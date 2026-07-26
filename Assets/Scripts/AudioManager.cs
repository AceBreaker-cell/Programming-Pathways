using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Music Sources")]
    public AudioSource musicSource;        // untuk background music
    public AudioSource sfxSource;          // untuk sound effects

    [Header("Music Clips")]
    public AudioClip mainMenuMusic;        // musik main menu
    public AudioClip gameplayMusic;        // musik gameplay

    [Header("SFX Clips")]
    public AudioClip buttonClickSFX;       // klik button
    public AudioClip jumpSFX;             // lompat
    public AudioClip correctSFX;          // jawab benar
    public AudioClip wrongSFX;            // jawab salah
    public AudioClip deathSFX;            // mati
    public AudioClip victorySFX;          // menang / result screen

    [Header("Music Settings")]
    public float musicVolume = 0.5f;
    public float sfxVolume = 1f;
    public float fadeDuration = 1f;

    void Awake()
    {
        // Singleton — jangan destroy saat ganti scene
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        musicSource.volume = musicVolume;
        sfxSource.volume = sfxVolume;
        musicSource.loop = true;

        // Deteksi scene sekarang dan play musik yang sesuai
        PlayMusicForCurrentScene();

        // Listen untuk scene change
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForCurrentScene();
    }

    void PlayMusicForCurrentScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "MainMenu" || sceneName == "Main Menu")
            PlayMusic(mainMenuMusic);
        else
            PlayMusic(gameplayMusic);
    }

    // ==================== MUSIC ====================

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        if (musicSource.clip == clip) return; // sudah playing

        StartCoroutine(FadeMusic(clip));
    }

    IEnumerator FadeMusic(AudioClip newClip)
    {
        // Fade out musik lama
        if (musicSource.isPlaying)
        {
            float startVolume = musicSource.volume;
            float elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                musicSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / fadeDuration);
                yield return null;
            }
        }

        // Ganti clip dan fade in
        musicSource.Stop();
        musicSource.clip = newClip;
        musicSource.Play();

        float elapsed2 = 0f;
        while (elapsed2 < fadeDuration)
        {
            elapsed2 += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0f, musicVolume, elapsed2 / fadeDuration);
            yield return null;
        }

        musicSource.volume = musicVolume;
    }

    // ==================== SFX ====================

    public void PlayButtonClick()
    {
        if (buttonClickSFX != null)
            sfxSource.PlayOneShot(buttonClickSFX, sfxVolume);
    }

    public void PlayJump()
    {
        if (jumpSFX != null)
            sfxSource.PlayOneShot(jumpSFX, sfxVolume);
    }

    public void PlayCorrect()
    {
        if (correctSFX != null)
            sfxSource.PlayOneShot(correctSFX, sfxVolume);
    }

    public void PlayWrong()
    {
        if (wrongSFX != null)
            sfxSource.PlayOneShot(wrongSFX, sfxVolume);
    }

    public void PlayDeath()
    {
        if (deathSFX != null)
            sfxSource.PlayOneShot(deathSFX, sfxVolume);
    }

    public void PlayVictory()
    {
        if (victorySFX != null)
        {
            // Stop musik gameplay dulu, play victory sfx
            musicSource.Stop();
            sfxSource.PlayOneShot(victorySFX, sfxVolume);
        }
    }
}