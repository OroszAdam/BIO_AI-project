/// Author: Samuel Arzt
/// Date: March 2017


#region Includes
using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
#endregion

/// <summary>
/// Singleton class for managing the evolutionary processes.
/// </summary>
public class EvolutionManager3D : MonoBehaviour
{
    #region Members
    private static System.Random randomizer = new System.Random();

    public static EvolutionManager3D Instance
    {
        get;
        private set;
    }

    // Whether or not the results of each generation shall be written to file, to be set in Unity Editor
    [SerializeField]
    private bool SaveStatistics = false;
    private string statisticsFileName;

    // How many of the first to finish the course should be saved to file, to be set in Unity Editor
    [SerializeField]
    private uint SaveFirstNGenotype = 0;
    private uint genotypesSaved = 0;

    // Population size, to be set in Unity Editor
    [SerializeField]
    private int PopulationSize = 30;

    // After how many generations should the genetic algorithm be restart (0 for never), to be set in Unity Editor
    [SerializeField]
    private int RestartAfter = 100;

    // Whether to use elitist selection or remainder stochastic sampling, to be set in Unity Editor
    [SerializeField]
    private bool ElitistSelection = false;

    // Topology of the agent's FNN, to be set in Unity Editor
    [SerializeField]
    private uint[] FNNTopology;

    // The current population of agents.
    private List<Agent> agents = new List<Agent>();
    /// <summary>
    /// The amount of agents that are currently alive.
    /// </summary>
    public int AgentsAliveCount
    {
        get;
        private set;
    }

    /// <summary>
    /// Event for when all agents have died.
    /// </summary>
    public event System.Action AllAgentsDied;

    private GeneticAlgorithm geneticAlgorithm;

    /// <summary>
    /// The age of the current generation.
    /// </summary>
    public uint GenerationCount
    {
        get { return geneticAlgorithm.GenerationCount; }
    }
    #endregion

    public float timeAlive;

    #region Constructors
    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("More than one EvolutionManager in the Scene.");
            return;
        }
        Instance = this;
    }
    #endregion

    #region Methods
    /// <summary>
    /// Starts the evolutionary process.
    /// </summary>
    public void StartEvolution()
    {
        //Create neural network to determine parameter count
        NeuralNetwork nn = new NeuralNetwork(FNNTopology);

        //Setup genetic algorithm
        geneticAlgorithm = new GeneticAlgorithm((uint) nn.WeightCount, (uint) PopulationSize);
        genotypesSaved = 0;

        geneticAlgorithm.Evaluation = StartEvaluation;

        if (ElitistSelection)
        {
            //Second configuration
            geneticAlgorithm.Selection = GeneticAlgorithm.DefaultSelectionOperator;
            geneticAlgorithm.Recombination = RandomRecombination;
            geneticAlgorithm.Mutation = MutateAllButBestTwo;
        }
        else
        {   
            //First configuration
            geneticAlgorithm.Selection = RemainderStochasticSampling;
            geneticAlgorithm.Recombination = RandomRecombination;
            geneticAlgorithm.Mutation = MutateAllButBestTwo;
        }
        
        AllAgentsDied += geneticAlgorithm.EvaluationFinished;

        //Statistics
        if (SaveStatistics)
        {
            statisticsFileName = "Evaluation - " + GameStateManager3D.Instance.TrackName + " " + DateTime.Now.ToString("yyyy_MM_dd_HH-mm-ss");
            WriteStatisticsFileStart();
            geneticAlgorithm.FitnessCalculationFinished += WriteStatisticsToFile;
            geneticAlgorithm.FitnessCalculationFinished += WriteBestCarTimeToCSV;
        }
        geneticAlgorithm.FitnessCalculationFinished += CheckForTrackFinished;

        //Restart logic
        if (RestartAfter > 0)
        {
            geneticAlgorithm.TerminationCriterion += CheckGenerationTermination;
            geneticAlgorithm.AlgorithmTerminated += OnGATermination;
        }

        geneticAlgorithm.Start();
    }

    public void StopEvolution()
    {
        geneticAlgorithm.Stop();
    }

    // Writes the starting line to the statistics file, stating all genetic algorithm parameters.
    private void WriteStatisticsFileStart()
    {
        File.WriteAllText(statisticsFileName + ".txt", "Evaluation of a Population with size " + PopulationSize + 
                ", on Track \"" + GameStateManager3D.Instance.TrackName + "\", using the following GA operators: " + Environment.NewLine +
                "Selection: " + geneticAlgorithm.Selection.Method.Name + Environment.NewLine +
                "Recombination: " + geneticAlgorithm.Recombination.Method.Name + Environment.NewLine +
                "Mutation: " + geneticAlgorithm.Mutation.Method.Name + Environment.NewLine + 
                "FitnessCalculation: " + geneticAlgorithm.FitnessCalculationMethod.Method.Name + Environment.NewLine + Environment.NewLine);
    }

    // Appends the current generation count and the evaluation of the best genotype to the statistics file.
    private void WriteStatisticsToFile(IEnumerable<Genotype> currentPopulation)
    {
        foreach (Genotype genotype in currentPopulation)
        {
            File.AppendAllText(statisticsFileName + ".txt", geneticAlgorithm.GenerationCount + "\t" + genotype.Evaluation + Environment.NewLine);
            break; //Only write first
        }
    }
    private void WriteStatisticsToCSV(IEnumerable<Genotype> currentPopulation)
    {
        foreach (Genotype genotype in currentPopulation)
        {
            File.WriteAllText(statisticsFileName + ".csv", geneticAlgorithm.GenerationCount + ";" + genotype.Evaluation + Environment.NewLine);
            break; //Only write first
        }
    }

    private void WriteBestCarTimeToCSV(IEnumerable<Genotype> currentPopulation)
    {
        File.WriteAllText(statisticsFileName + ".csv", "GENERATION;TOTAL RACE TIME OF BEST AGENT;SCORE");
        File.AppendAllText(statisticsFileName + "---BESTCARTIME.csv",  geneticAlgorithm.GenerationCount + ";" + TrackManager3D.Instance.BestCar.raceTime +  Environment.NewLine);
    }

    // Checks the current population and saves genotypes to a file if their evaluation is greater than or equal to 0.35
    private void CheckForTrackFinished(IEnumerable<Genotype> currentPopulation)
    {
        if (genotypesSaved >= SaveFirstNGenotype) 
        {   
            Debug.Log("genotypesSaved >= SaveFirstNGenotype");
            return;
        }

        string saveFolder = statisticsFileName + "/";

        foreach (Genotype genotype in currentPopulation)
        {
            if (genotype.Evaluation >= 0.35)
            {
                if (!Directory.Exists(saveFolder))
                    Directory.CreateDirectory(saveFolder);

                genotype.SaveToFile(saveFolder + "Genotype - Finished as " + (++genotypesSaved) + " - Generation " + geneticAlgorithm.GenerationCount + ".txt");

                if (genotypesSaved >= SaveFirstNGenotype) return;
            }
            else
                return; //List should be sorted, so we can exit here
        }
    }

    // Checks whether the termination criterion of generation count was met.
    private bool CheckGenerationTermination(IEnumerable<Genotype> currentPopulation)
    {
        return geneticAlgorithm.GenerationCount >= RestartAfter;
    }

    // To be called when the genetic algorithm was terminated
    private void OnGATermination(GeneticAlgorithm ga)
    {
        AllAgentsDied -= ga.EvaluationFinished;

        RestartAlgorithm(5.0f);
    }

    // Restarts the algorithm after a specific wait time second wait
    private void RestartAlgorithm(float wait)
    {
        Invoke("StartEvolution", wait);
    }

    public void RunAgent()
    {
        //Create new agents from currentPopulation
        agents.Clear();
        AgentsAliveCount = 0;


        Genotype genotype;

        // So far this is hardcoded, the genotypes of certain trained agents can be found in the project folder
        // name starting with "Genotype - finished as..." 
        string loadFolder = "Genotype - Finished as 497_4layer_best_topology_5432.txt";
        genotype = Genotype.LoadFromFile(loadFolder);
        Debug.Log("genotype: " + genotype);

        agents.Add(new Agent(genotype, MathHelper.SoftSignFunction, FNNTopology));
        TrackManager3D.Instance.SetCarAmount(1);

        IEnumerator<CarController3D> carsEnum = TrackManager3D.Instance.GetCarEnumerator();
        for (int i = 0; i < agents.Count; i++)
        {
            if (!carsEnum.MoveNext())
            {
                Debug.LogError("Cars enum ended before agents.");
                break;
            }

            carsEnum.Current.Agent = agents[i];
            AgentsAliveCount++;
            agents[i].AgentDied += OnAgentDied;
        }
        TrackManager3D.Instance.Restart();

    }

    // Starts the evaluation by first creating new agents from the current population and then restarting the track manager.
    private void StartEvaluation(IEnumerable<Genotype> currentPopulation)
    {
        //Create new agents from currentPopulation
        agents.Clear();
        AgentsAliveCount = 0;

        foreach (Genotype genotype in currentPopulation)
            agents.Add(new Agent(genotype, MathHelper.SoftSignFunction, FNNTopology));

        TrackManager3D.Instance.SetCarAmount(agents.Count);
        IEnumerator<CarController3D> carsEnum = TrackManager3D.Instance.GetCarEnumerator();
        for (int i = 0; i < agents.Count; i++)
        {
            if (!carsEnum.MoveNext())
            {
                Debug.LogError("Cars enum ended before agents.");
                break;
            }

            carsEnum.Current.Agent = agents[i];
            AgentsAliveCount++;
            agents[i].AgentDied += OnAgentDied;
        }

        TrackManager3D.Instance.Restart();
    }

    // Callback for when an agent died.
    private void OnAgentDied(Agent agent)
    {
        AgentsAliveCount--;

        if (AgentsAliveCount == 0 && AllAgentsDied != null)
            AllAgentsDied();
    }

    #region GA Operators
    // Selection operator for the genetic algorithm, using a method called remainder stochastic sampling.
    private List<Genotype> RemainderStochasticSampling(List<Genotype> currentPopulation)
    {
        List<Genotype> intermediatePopulation = new List<Genotype>();
        //Put integer portion of genotypes into intermediatePopulation
        //Assumes that currentPopulation is already sorted
        foreach (Genotype genotype in currentPopulation)
        {
            if (genotype.Fitness < 1)
            {
                Debug.Log("genotype.Fitness <1" + genotype.Fitness);
                break;
            }
            else
            {
                for (int i = 0; i < (int) genotype.Fitness; i++)
                {
                    Debug.Log("Integer of genot: " + genotype.GetParameterCopy()[0] + " fitness: " + genotype.Fitness);
                    intermediatePopulation.Add(new Genotype(genotype.GetParameterCopy()));
                }
                    
            }
        }

        //Put remainder portion of genotypes into intermediatePopulation
        foreach (Genotype genotype in currentPopulation)
        {
            float remainder = genotype.Fitness - (int)genotype.Fitness;
            if (randomizer.NextDouble() < remainder)
            {
                Debug.Log("remainder of genot: " + genotype.GetParameterCopy()[0] + " fitness: " + genotype.Fitness);
                intermediatePopulation.Add(new Genotype(genotype.GetParameterCopy()));
            }
        }
        Debug.Log("intermed pop: " + intermediatePopulation.Count);
        return intermediatePopulation;
    }

    // Recombination operator for the genetic algorithm, recombining random genotypes of the intermediate population
    private List<Genotype> RandomRecombination(List<Genotype> intermediatePopulation, uint newPopulationSize)
    {
        //Check arguments
        if (intermediatePopulation.Count < 2)
            throw new System.ArgumentException("The intermediate population has to be at least of size 2 for this operator.");

        List<Genotype> newPopulation = new List<Genotype>();
        //Always add best two (unmodified)
        newPopulation.Add(intermediatePopulation[0]);
        newPopulation.Add(intermediatePopulation[1]);


        while (newPopulation.Count < newPopulationSize)
        {
            //Get two random indices that are not the same
            int randomIndex1 = randomizer.Next(0, intermediatePopulation.Count), randomIndex2;
            do
            {
                randomIndex2 = randomizer.Next(0, intermediatePopulation.Count);
            } while (randomIndex2 == randomIndex1);

            Genotype offspring1, offspring2;
            GeneticAlgorithm.CompleteCrossover(intermediatePopulation[randomIndex1], intermediatePopulation[randomIndex2], 
                GeneticAlgorithm.DefCrossSwapProb, out offspring1, out offspring2);

            newPopulation.Add(offspring1);
            if (newPopulation.Count < newPopulationSize)
                newPopulation.Add(offspring2);
        }

        return newPopulation;
    }

    // Mutates all members of the new population with the default probability, while leaving the first 2 genotypes in the list untouched.
    private void MutateAllButBestTwo(List<Genotype> newPopulation)
    {
        for (int i = 2; i < newPopulation.Count; i++)
        {
            if (randomizer.NextDouble() < GeneticAlgorithm.DefMutationPerc)
                GeneticAlgorithm.MutateGenotype(newPopulation[i], GeneticAlgorithm.DefMutationProb, GeneticAlgorithm.DefMutationAmount);
        }
    }

    // Mutates all members of the new population with the default parameters
    private void MutateAll(List<Genotype> newPopulation)
    {
        foreach (Genotype genotype in newPopulation)
        {
            if (randomizer.NextDouble() < GeneticAlgorithm.DefMutationPerc)
                GeneticAlgorithm.MutateGenotype(genotype, GeneticAlgorithm.DefMutationProb, GeneticAlgorithm.DefMutationAmount);
        }
    }
    #endregion
    #endregion

    }
