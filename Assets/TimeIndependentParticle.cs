using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeIndependentParticle : MonoBehaviour
{
    ParticleSystem particleSystem;

    private void Start()
    {
        particleSystem = this.GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (Time.timeScale < 0.01f)
        {
            particleSystem.Simulate(Time.unscaledDeltaTime, true, false);
        }
    }
}
