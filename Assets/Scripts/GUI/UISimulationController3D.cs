/// Author: Samuel Arzt
/// Date: March 2017


#region Includes
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;
#endregion

/// <summary>
/// Class for controlling the various ui elements of the simulation
/// </summary>
public class UISimulationController3D : MonoBehaviour
{
    #region Members
    private CarController3D target;
    /// <summary>
    /// The Car to fill the GUI data with.
    /// </summary>
    public CarController3D Target
    {
        get { return target; }
        set
        {
            if (target != value)
            {
                target = value;

                if (target != null)
                    NeuralNetPanel.Display(target.Agent.FNN);
            }
        }
    }

    // GUI element references to be set in Unity Editor.
    [SerializeField]
    private TMP_Text[] InputTexts;
    [SerializeField]
    private TMP_Text Evaluation;
    [SerializeField]
    private TMP_Text GenerationCount;
    [SerializeField]
    private UINeuralNetworkPanel NeuralNetPanel;
    #endregion

    #region Constructors
    void Awake()
    {

    }
    #endregion

    #region Methods
    void Update()
    {
        if (Target != null)
        {
            //Display controls
            if (Target.CurrentControlInputs != null)
            {
                for (int i = 0; i < InputTexts.Length; i++)
                    InputTexts[i].text = Target.CurrentControlInputs[i].ToString("F3");
            }

            //Display evaluation and generation count
            Evaluation.text = Target.Agent.Genotype.Evaluation.ToString("F3");
            GenerationCount.text = EvolutionManager3D.Instance.GenerationCount.ToString();
            //Debug.Log("Generation count = " + EvolutionManager3D.Instance.GenerationCount);
        }
    }

    /// <summary>
    /// Starts to display the gui elements.
    /// </summary>
    public void Show()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Stops displaying the gui elements.
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    #endregion
}
