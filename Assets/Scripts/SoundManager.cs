using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Clips")]
    public AudioClip correctSound;
    public AudioClip wrongSound;
    public AudioClip deathSound;

    private AudioSource audioSource;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlayCorrect() => audioSource.PlayOneShot(correctSound);
    public void PlayWrong()   => audioSource.PlayOneShot(wrongSound);
    public void PlayDeath()   => audioSource.PlayOneShot(deathSound);
}