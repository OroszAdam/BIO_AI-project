using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public GameObject Target;
    public bool RotateCamera { get; set; }

    void Start()
    {
        QualitySettings.vSyncCount = 0;
        QualitySettings.antiAliasing = 0;
        Application.targetFrameRate = 60;

        transform.LookAt(Target.transform);
    }

    void Update()
    {
        if (Target != null)
        {
            transform.LookAt(Target.transform);
        }

        if (RotateCamera && !PauseMenu.GameIsPaused)
        {
            Vector3 newRotation = new Vector3(transform.rotation.eulerAngles.x,
                                               transform.rotation.eulerAngles.y,
                                               transform.rotation.eulerAngles.z + (MobileInput.GyroRotation.eulerAngles.z - MobileInput.InitialGyroRotation.eulerAngles.z));
            transform.rotation = Quaternion.Euler(newRotation);
        }

    }
}
