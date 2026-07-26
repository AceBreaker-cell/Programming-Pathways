using UnityEngine;

public class ParticleSpawner : MonoBehaviour
{
    public static ParticleSpawner Instance;

    [Header("Particle Prefabs")]
    public ParticleSystem correctParticle;  // particle hijau/bintang
    public ParticleSystem wrongParticle;    // particle merah/asap

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SpawnCorrect(Vector3 position)
    {
        if (correctParticle != null)
        {
            var p = Instantiate(correctParticle, position, Quaternion.identity);
            p.Play();
            Destroy(p.gameObject, 2f);
        }
    }

    public void SpawnWrong(Vector3 position)
    {
        if (wrongParticle != null)
        {
            var p = Instantiate(wrongParticle, position, Quaternion.identity);
            p.Play();
            Destroy(p.gameObject, 2f);
        }
    }
}