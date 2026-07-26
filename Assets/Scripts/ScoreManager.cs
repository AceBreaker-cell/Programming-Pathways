using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("HUD References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI posText;

    [Header("Result Panel References")]
    public GameObject resultPanel;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI ratingText;
    public Button retryButton;

    [Header("Animation")]
    public float slideDuration = 0.5f;
    public float slideStartY = -800f;

    private int score = 0;
    private int posCompleted = 0;
    private int totalPos = 10;
    private RectTransform resultRect;
    private Vector2 resultTargetPos;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        resultPanel.SetActive(false);

        resultRect = resultPanel.GetComponent<RectTransform>();
        resultTargetPos = resultRect.anchoredPosition;

        UpdateHUD();
        retryButton.onClick.AddListener(RestartGame);
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateHUD();
    }

    public void CompletePos()
    {
        posCompleted++;
        UpdateHUD();

        if (posCompleted >= totalPos)
        {
            Invoke(nameof(OpenResultPanel), 2.5f);
        }
    }

    void UpdateHUD()
    {
        scoreText.text = "Score: " + score;
        posText.text = "Pos (" + posCompleted + "/" + totalPos + ")";
    }

    void OpenResultPanel()
    {
        resultPanel.SetActive(true);
        Time.timeScale = 0f;

        finalScoreText.text = "Score: " + score + " / " + (totalPos * 10);
        ratingText.text = "Rating: " + GetRating(score);
        ratingText.color = GetRatingColor(score);

        if (HighScoreManager.Instance != null)
            HighScoreManager.Instance.TrySetHighScore(score);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayVictory();

        StartCoroutine(SlideUpResult());
    }

    IEnumerator SlideUpResult()
    {
        resultRect.anchoredPosition = new Vector2(resultTargetPos.x, slideStartY);

        float elapsed = 0f;

        while (elapsed < slideDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / slideDuration;
            float easedT = 1f - Mathf.Pow(1f - t, 3f);

            resultRect.anchoredPosition = Vector2.Lerp(
                new Vector2(resultTargetPos.x, slideStartY),
                resultTargetPos,
                easedT
            );

            yield return null;
        }

        resultRect.anchoredPosition = resultTargetPos;
        StartCoroutine(PopRating());
    }

    IEnumerator PopRating()
    {
        ratingText.transform.localScale = Vector3.zero;
        float elapsed = 0f;
        float duration = 0.3f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            float scale = 1f + 0.3f * Mathf.Sin(t * Mathf.PI);
            ratingText.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t) * (t < 1f ? scale : 1f);

            yield return null;
        }

        ratingText.transform.localScale = Vector3.one;
    }

    string GetRating(int s)
    {
        if (s < 50) return "E";
        if (s < 60) return "D-";
        if (s < 65) return "D";
        if (s < 70) return "C";
        if (s < 75) return "C+";
        if (s < 80) return "B";
        if (s < 85) return "B+";
        if (s < 95) return "A";
        return "A+";
    }

    Color GetRatingColor(int s)
    {
        if (s < 50) return new Color(1f, 0.2f, 0.2f);
        if (s < 70) return new Color(1f, 0.85f, 0f);
        if (s < 80) return new Color(1f, 0.65f, 0f);
        return new Color(0.2f, 0.9f, 0.3f);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}