using UnityEngine;

public class CoinBehavior : MonoBehaviour
{
    public GameObject[] ParticlePrefabs;
    public int NumberOfCoinsToAdd = 1;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.transform.parent.tag == "Player")
        {
            GameManager.Instance.AddCoin(transform.position, NumberOfCoinsToAdd);

            InstantiateParticles();

            Destroy(gameObject);
        }
    }

    private void InstantiateParticles()
    {
        foreach (var particlePrefab in ParticlePrefabs)
        {
            Instantiate(particlePrefab, transform.position, Quaternion.identity);
        }
    }
}
