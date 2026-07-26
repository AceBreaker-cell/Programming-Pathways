using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class DeathSystem : MonoBehaviour
{
    public static DeathSystem Instance;

    [Header("Player")]
    public Transform player;
    public float deathY = -5f;

    [Header("Death UI")]
    public GameObject deathPanel;
    public Image deathOverlay;      // background hitam
    public GameObject deathImage;   // tulisan "You Are Dead"
    public GameObject retryButton;  // tombol retry

    [Header("Animation Timing")]
    public float fadeDuration = 1f;     // durasi fade in hitam
    public float delayBeforeText = 0.5f; // jeda sebelum tulisan muncul
    public float delayBeforeButton = 1f; // jeda sebelum tombol muncul
    public float textFadeDuration = 0.8f;

    private bool isDead = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        deathPanel.SetActive(false);

        // Sembunyikan semua elemen
        SetImageAlpha(deathOverlay, 0f);
        SetCanvasGroupAlpha(deathImage, 0f);
        SetCanvasGroupAlpha(retryButton, 0f);

        // Tambah Button listener
        var btn = retryButton.GetComponent<Button>();
        if (btn != null) btn.onClick.AddListener(RetryGame);
    }

    void Update()
    {
        if (isDead) return;
        if (player == null) return;

        if (player.position.y < deathY)
            TriggerDeath();
    }

    public void TriggerDeath()
    {
        if (isDead) return;
        isDead = true;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayDeath();

        StartCoroutine(PlayDeathSequence());
    }

    IEnumerator PlayDeathSequence()
    {
        // Aktifkan panel dulu
        deathPanel.SetActive(true);
        deathImage.SetActive(true);
        retryButton.SetActive(true);

        // Pastikan semua mulai invisible
        SetImageAlpha(deathOverlay, 0f);
        SetCanvasGroupAlpha(deathImage, 0f);
        SetCanvasGroupAlpha(retryButton, 0f);

        // Step 1 — Fade in overlay hitam
        yield return StartCoroutine(FadeImage(deathOverlay, 0f, 0.8f, fadeDuration));

        // Step 2 — Jeda sebentar
        yield return new WaitForSeconds(delayBeforeText);

        // Step 3 — Fade in tulisan "You Are Dead"
        yield return StartCoroutine(FadeCanvasGroup(deathImage, 0f, 1f, textFadeDuration));

        // Step 4 — Jeda sebentar
        yield return new WaitForSeconds(delayBeforeButton);

        // Step 5 — Fade in tombol retry
        yield return StartCoroutine(FadeCanvasGroup(retryButton, 0f, 1f, textFadeDuration));
    }

    IEnumerator FadeImage(Image img, float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float easedT = 1f - Mathf.Pow(1f - t, 3f);
            SetImageAlpha(img, Mathf.Lerp(from, to, easedT));
            yield return null;
        }
        SetImageAlpha(img, to);
    }

    IEnumerator FadeCanvasGroup(GameObject obj, float from, float to, float duration)
    {
        var cg = obj.GetComponent<CanvasGroup>();
        if (cg == null) cg = obj.AddComponent<CanvasGroup>();

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float easedT = 1f - Mathf.Pow(1f - t, 3f);
            cg.alpha = Mathf.Lerp(from, to, easedT);
            yield return null;
        }
        cg.alpha = to;
    }

    void SetImageAlpha(Image img, float alpha)
    {
        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }

    void SetCanvasGroupAlpha(GameObject obj, float alpha)
    {
        var cg = obj.GetComponent<CanvasGroup>();
        if (cg == null) cg = obj.AddComponent<CanvasGroup>();
        cg.alpha = alpha;
    }

    public void RetryGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}