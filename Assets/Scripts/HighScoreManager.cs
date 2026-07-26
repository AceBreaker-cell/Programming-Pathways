using UnityEngine;
using TMPro;

public class HighScoreManager : MonoBehaviour
{
    public static HighScoreManager Instance;

    [Header("UI References")]
    public TextMeshProUGUI highScoreText;

    private const string HIGH_SCORE_KEY = "HighScore";
    private int highScore = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        highScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
        UpdateHighScoreUI();
    }

    public void TrySetHighScore(int newScore)
    {
        if (newScore > highScore)
        {
            highScore = newScore;
            PlayerPrefs.SetInt(HIGH_SCORE_KEY, highScore);
            PlayerPrefs.Save();
        }
        UpdateHighScoreUI();
    }

    void UpdateHighScoreUI()
    {
        if (highScoreText != null)
            highScoreText.text = "Best Score: " + highScore;
    }
}