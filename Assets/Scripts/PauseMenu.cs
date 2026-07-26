using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class PauseMenu : MonoBehaviour
{
    [Header("=== PAUSE MENU REFERENCES ===")]
    [SerializeField] private GameObject pauseMenuPanel;       // Panel utama pause menu
    [SerializeField] private CanvasGroup overlayBackground;   // Dark overlay (80% opacity)
    [SerializeField] private GameObject pauseButton;          // Tombol pause di HUD

    [Header("=== BUTTONS ===")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button quitToMainMenuButton;

    [Header("=== VOLUME SLIDER ===")]
    [SerializeField] private Slider volumeSlider;

    [Header("=== SCENE SETTINGS ===")]
    [SerializeField] private string mainMenuSceneName = "MainMenu"; // Ganti sesuai nama scene kamu

    [Header("=== ANIMATION SETTINGS ===")]
    [SerializeField] private float fadeInDuration = 0.25f;
    [SerializeField] private float fadeOutDuration = 0.2f;

    // -------------------------------------------------------
    private bool isPaused = false;
    private RectTransform pauseMenuRect;

    // Singleton (opsional, hapus jika tidak perlu)
    public static PauseMenu Instance { get; private set; }

    // -------------------------------------------------------
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        pauseMenuRect = pauseMenuPanel != null ? pauseMenuPanel.GetComponent<RectTransform>() : null;
    }

    private void Start()
    {
        // Pastikan pause menu tertutup di awal
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (overlayBackground != null)
        {
            overlayBackground.alpha = 0f;
            overlayBackground.gameObject.SetActive(false);
        }

        // Daftarkan button events
        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);

        if (quitToMainMenuButton != null)
            quitToMainMenuButton.onClick.AddListener(QuitToMainMenu);

        // Volume slider — load nilai tersimpan
        if (volumeSlider != null)
        {
            float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
            volumeSlider.value = savedVolume;
            AudioListener.volume = savedVolume;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }
    }

    private void Update()
    {
        // Tekan ESC atau tombol Start (gamepad) untuk pause/resume
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    // -------------------------------------------------------
    //  PUBLIC METHODS
    // -------------------------------------------------------

    /// <summary>
    /// Dipanggil dari tombol Pause di HUD
    /// </summary>
    public void PauseGame()
    {
        if (isPaused) return;
        isPaused = true;

        Time.timeScale = 0f;  // Hentikan waktu game

        if (pauseButton != null) pauseButton.SetActive(false);

        // Aktifkan overlay + panel lalu animasikan
        if (overlayBackground != null) overlayBackground.gameObject.SetActive(true);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);

        StopAllCoroutines();
        StartCoroutine(FadeIn());
    }

    /// <summary>
    /// Dipanggil dari tombol Resume
    /// </summary>
    public void ResumeGame()
    {
        if (!isPaused) return;

        StopAllCoroutines();
        StartCoroutine(FadeOutAndResume());
    }

    /// <summary>
    /// Kembali ke Main Menu
    /// </summary>
    public void QuitToMainMenu()
    {
        Time.timeScale = 1f; // Reset time scale sebelum pindah scene
        isPaused = false;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // -------------------------------------------------------
    //  VOLUME
    // -------------------------------------------------------
    private void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("MasterVolume", value);
        PlayerPrefs.Save();
    }

    // -------------------------------------------------------
    //  COROUTINES — Fade animasi
    // -------------------------------------------------------
    private IEnumerator FadeIn()
    {
        float elapsed = 0f;
        if (overlayBackground != null) overlayBackground.alpha = 0f;

        // Scale animasi panel (pop-in)
        if (pauseMenuRect != null)
            pauseMenuRect.localScale = Vector3.one * 0.85f;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.unscaledDeltaTime; // Pakai unscaled karena Time.timeScale = 0
            float t = elapsed / fadeInDuration;
            float eased = 1f - Mathf.Pow(1f - t, 3f); // Ease-out cubic

            if (overlayBackground != null)
                overlayBackground.alpha = Mathf.Lerp(0f, 0.85f, eased); // 80–85% opacity

            if (pauseMenuRect != null)
                pauseMenuRect.localScale = Vector3.Lerp(Vector3.one * 0.85f, Vector3.one, eased);

            yield return null;
        }

        if (overlayBackground != null) overlayBackground.alpha = 0.85f;
        if (pauseMenuRect != null) pauseMenuRect.localScale = Vector3.one;
    }

    private IEnumerator FadeOutAndResume()
    {
        float elapsed = 0f;
        float startAlpha = overlayBackground != null ? overlayBackground.alpha : 0.85f;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / fadeOutDuration;
            float eased = t * t; // Ease-in

            if (overlayBackground != null)
                overlayBackground.alpha = Mathf.Lerp(startAlpha, 0f, eased);

            if (pauseMenuRect != null)
                pauseMenuRect.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.85f, eased);

            yield return null;
        }

        // Selesai fade — sembunyikan dan resume
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (overlayBackground != null)
        {
            overlayBackground.alpha = 0f;
            overlayBackground.gameObject.SetActive(false);
        }
        if (pauseButton != null) pauseButton.SetActive(true);

        isPaused = false;
        Time.timeScale = 1f;
    }

    // -------------------------------------------------------
    private void OnDestroy()
    {
        if (resumeButton != null) resumeButton.onClick.RemoveListener(ResumeGame);
        if (quitToMainMenuButton != null) quitToMainMenuButton.onClick.RemoveListener(QuitToMainMenu);
        if (volumeSlider != null) volumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);
    }
}
