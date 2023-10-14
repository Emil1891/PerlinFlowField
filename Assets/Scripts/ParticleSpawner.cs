using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class ParticleSpawner : MonoBehaviour
{
    [SerializeField] private int spawnCount = 1000;

    [SerializeField] private GameObject Particle; 
    
    // Start is called before the first frame update
    void Start()
    {
        var cam = Camera.main;

        for (int i = 0; i < spawnCount; i++)
        {
            var spawnPos = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f));

            spawnPos = cam.ViewportToWorldPoint(spawnPos);

            Instantiate(Particle, spawnPos, quaternion.identity);
        }
    }
}
