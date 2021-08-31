using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tilter : MonoBehaviour
{
    private float angle = 0;
    private float sinValue;

    // With these values the block will rotate 35°
    public float FrequencyModifier = 60.0f;
    public float AmplitudeModifier = 0.3f;

    public bool TiltX = true;
    public bool TiltY = false;
    public bool TiltZ = false;


    void Start()
    {
    }

    void Update()
    {
        // Rotate the object back and forth
        angle = (float)(Mathf.PI * Time.time * FrequencyModifier / 180.0);
        sinValue = Mathf.Sin(angle) * AmplitudeModifier;

        if (!PauseMenu.GameIsPaused)
            transform.Rotate((TiltX) ? sinValue : 0.0f,
                        (TiltY) ? sinValue : 0.0f,
                        (TiltZ) ? sinValue : 0.0f);
    }
}
