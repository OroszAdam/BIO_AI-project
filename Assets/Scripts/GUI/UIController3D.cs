/// Author: Samuel Arzt
/// Date: March 2017

#region Includes
using UnityEngine;
using UnityEngine.UI;
#endregion

/// <summary>
/// Class for controlling the overall GUI.
/// </summary>
public class UIController3D : MonoBehaviour
{
    #region Members
    /// <summary>
    /// The parent canvas of all UI elements.
    /// </summary>
    public Canvas Canvas
    {
        get;
        private set;
    }

    public Button trainButton; 
    public Button stopButton; 
    public Button runButton;

    public Toggle raceMode;

    public bool runMode = false;

    private UISimulationController3D simulationUI;
    private UIStartMenuController3D startMenuUI;
    #endregion

    #region Constructors
    void Awake()
    {
        if (GameStateManager3D.Instance != null)
            GameStateManager3D.Instance.UIController = this;

        Canvas = GetComponent<Canvas>();
        simulationUI = GetComponentInChildren<UISimulationController3D>(true);
        startMenuUI = GetComponentInChildren<UIStartMenuController3D>(true);

        simulationUI.Show();
    }
    #endregion

    private void Start() {
        Button trainBtn = trainButton.GetComponent<Button>();
		trainBtn.onClick.AddListener(StartTraining);

        Button stopBtn = stopButton.GetComponent<Button>();
		stopBtn.onClick.AddListener(StopTraining);

        Button runBtn = runButton.GetComponent<Button>();
		runBtn.onClick.AddListener(RunAgentUI);

    }
    #region Methods
    /// <summary>
    /// Sets the CarController from which to get the data from to be displayed.
    /// </summary>
    /// <param name="target">The CarController to display the data of.</param>
    public void SetDisplayTarget(CarController3D target)
    {
        simulationUI.Target = target;
    }

    void StartTraining(){
		EvolutionManager3D.Instance.StartEvolution();
	}

    void StopTraining(){
		EvolutionManager3D.Instance.StopEvolution();
	}

    void RunAgentUI(){
        runMode = true;
        if (raceMode.isOn)
        {
            TrackManager3D.Instance.InstantiateRaceCar();
            Debug.Log("RACING AGAINST THE AI");
        }
		EvolutionManager3D.Instance.RunAgent();
        Debug.Log("Agent is running");

	}
    #endregion
}
