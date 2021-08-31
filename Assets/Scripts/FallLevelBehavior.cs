using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallLevelBehavior : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.transform.parent.tag == "Player")
        {
            GameEvents.Instance.FallEventEnter();
        }
    }
}
