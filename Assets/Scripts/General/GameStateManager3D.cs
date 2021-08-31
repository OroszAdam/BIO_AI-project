/// Author: Samuel Arzt
/// Date: March 2017

#region Includes
using UnityEngine;
using UnityEngine.SceneManagement;
#endregion

/// <summary>
/// Singleton class managing the overall simulation.
/// </summary>
public class GameStateManager3D : MonoBehaviour
{
    #region Members
    // The camera object, to be referenced in Unity Editor.
    [SerializeField]
    private CameraMovement Camera;

    // The name of the track to be loaded
    [SerializeField]
    public string TrackName;

    /// <summary>
    /// The UIController object.
    /// </summary>
    public UIController3D UIController
    {
        get;
        set;
    }

    public static GameStateManager3D Instance
    {
        get;
        private set;
    }

    private CarController3D prevBest, prevSecondBest;

    public GameObject carPlayerControlled;
    #endregion

    #region Constructors
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Multiple GameStateManagers in the Scene.");
            return;
        }
        Instance = this;

        //Load gui scene
        SceneManager.LoadScene("GUI3D", LoadSceneMode.Additive);

        //Load track
        SceneManager.LoadScene(TrackName, LoadSceneMode.Additive);
    }

    void Start ()
    {     
        if(!UIController.raceMode.isOn)
            TrackManager3D.Instance.BestCarChanged += OnBestCarChanged;

        Debug.Log("Check line 149 of the script 'CarController3D'");
        Debug.Log("For Run Mode check line 241 of the script 'EvolutionManager3D'");
	}
    private void Update() {
        //Debug.Log("ison: " + UIController.raceMode.isOn);
        //Debug.Log("racecar: " + carPlayerControlled);
        if(UIController.raceMode.isOn)
        Camera.SetTarget(carPlayerControlled);
    }
    #endregion

    #region Methods
    // Callback method for when the best car has changed.
    private void OnBestCarChanged(CarController3D bestCar)
    {
        if(!UIController.raceMode.isOn)
        {
            if (bestCar == null)
            Camera.SetTarget(null);
            else
            Camera.SetTarget(bestCar.gameObject);
            
            if (UIController != null)
            UIController.SetDisplayTarget(bestCar);
        }
    }
    #endregion
}
