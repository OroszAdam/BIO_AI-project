using UnityEngine;

public class CrystalBehavior : MonoBehaviour
{
    public GameObject ParticlePrefab;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.transform.parent.tag == "Player")
        {
            GameEvents.Instance.CrystalTriggerEnter();
            var particle = Instantiate(ParticlePrefab, transform.position, Quaternion.identity);
        }
    }
}
