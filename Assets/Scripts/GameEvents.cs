using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    #region Singleton

    public static GameEvents Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    #endregion

    #region Moving block trigger events

    public event Action<int> onMovingBlockTriggerEnter;
    public event Action<int> onMovingBlockTriggerExit;
    public void MovingBlockTriggerEnter(int id)
    {
        if (onMovingBlockTriggerEnter != null)
        {
            onMovingBlockTriggerEnter.Invoke(id);
        }
    }

    public void MovingBlockTriggerExit(int id)
    {
        if (onMovingBlockTriggerExit != null)
        {
            onMovingBlockTriggerExit.Invoke(id);
        }
    }

    #endregion

    #region Crystal trigger events

    public event Action onCrystalTriggerEnter;
    public void CrystalTriggerEnter()
    {
        if (onCrystalTriggerEnter != null)
        {
            onCrystalTriggerEnter.Invoke();
        }
    }

    #endregion
    
    #region Fall events

    public event Action onFallEvent;
    public void FallEventEnter()
    {
        if (onFallEvent != null)
        {
            onFallEvent.Invoke();
        }
    }

    #endregion
}
