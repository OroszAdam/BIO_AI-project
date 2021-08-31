using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floater : MonoBehaviour
{
    private float angle = 0;
    private float sinValue;
    private float initialYPosition;

    [Header("Z-floating")]
    public float FrequencyModifier = 60.0f;
    public float StartOffset = 0.0f;
    public float AmplitudeModifier = 0.2f;

    [Header("Rotation")]
    public bool Rotate = false;
    public Vector3 RotationSpeed = new Vector3(1.0f, 0.0f, 0.0f);

    void Start()
    {
        initialYPosition = transform.position.y;
    }

    void Update()
    {
        // Move the object up and down in a sine wave
        angle = (float)(Mathf.PI * (StartOffset + Time.time * FrequencyModifier) / 180.0);
        sinValue = Mathf.Sin(angle) * AmplitudeModifier;
        transform.position = new Vector3(transform.position.x, initialYPosition + sinValue, transform.position.z);

        // Rotate if asked and not paused
        if (Rotate && Time.timeScale > 0f)
        {
            transform.Rotate(RotationSpeed);
        }
    }
}
