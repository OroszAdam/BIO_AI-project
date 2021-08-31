using UnityEngine;

public class TriggerArea : MonoBehaviour
{
    private int id;

    private void Awake()
    {
        id = transform.parent.GetComponent<MovingCubeController>().Id;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.transform.parent.tag == "Player")
        {
            other.gameObject.transform.parent.transform.parent = transform.parent;
        }

        GameEvents.Instance.MovingBlockTriggerEnter(id);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.transform.parent.tag == "Player")
        {
            other.gameObject.transform.parent.transform.parent = null;
        }

        GameEvents.Instance.MovingBlockTriggerExit(id);
    }
}
