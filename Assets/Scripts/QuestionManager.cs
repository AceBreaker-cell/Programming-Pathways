using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class QuestionManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject overlay;
    public GameObject paperPanel;
    public TextMeshProUGUI questionText;
    public Button buttonA;
    public Button buttonB;
    public TextMeshProUGUI buttonAText;
    public TextMeshProUGUI buttonBText;
    public TextMeshProUGUI resultText;

    [Header("Statue Questions")]
    public StatueQuestions[] allStatues;

    [Header("Settings")]
    public float detectionRadius = 1.5f;
    public KeyCode interactKey = KeyCode.E;

    [Header("Animation")]
    public float slideDuration = 0.4f;
    public float slideStartY = -600f;

    public StatueArrow statueArrow;

    private Transform player;
    private Transform[] statueTransforms;
    private bool isOpen = false;
    private Question currentQuestion;
    private bool answered = false;
    private RectTransform paperRect;
    private Vector2 paperTargetPos;
    private HashSet<int> completedStatues = new HashSet<int>();

    void Start()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;

        var posParent = GameObject.Find("Question Pos");
        if (posParent != null)
        {
            statueTransforms = new Transform[posParent.transform.childCount];
            for (int i = 0; i < posParent.transform.childCount; i++)
                statueTransforms[i] = posParent.transform.GetChild(i);
        }

        paperRect = paperPanel.GetComponent<RectTransform>();
        paperTargetPos = paperRect.anchoredPosition;

        overlay.SetActive(false);
        paperPanel.SetActive(false);
        resultText.gameObject.SetActive(false);

        buttonA.onClick.AddListener(() => OnAnswer(false));
        buttonB.onClick.AddListener(() => OnAnswer(true));
    }

    void Update()
    {
        if (isOpen) return;
        if (player == null) return;

        float closest = float.MaxValue;
        int closestIndex = -1;

        for (int i = 0; i < statueTransforms.Length; i++)
        {
            if (completedStatues.Contains(i)) continue;

            float d = Vector2.Distance(player.position, statueTransforms[i].position);
            if (d < closest)
            {
                closest = d;
                closestIndex = i;
            }
        }

        if (closest <= detectionRadius && closestIndex != -1 && Input.GetKeyDown(interactKey))
        {
            OpenQuestion(closestIndex);
        }
    }

    void OpenQuestion(int statueIndex)
    {
        if (statueIndex < 0 || statueIndex >= allStatues.Length) return;

        var questions = allStatues[statueIndex].questions;
        currentQuestion = questions[Random.Range(0, questions.Length)];
        currentQuestion.statueIndex = statueIndex;

        questionText.text = currentQuestion.questionText;
        buttonAText.text = currentQuestion.optionA;
        buttonBText.text = currentQuestion.optionB;

        ColorBlock defaultColors = buttonA.colors;
        defaultColors.normalColor = Color.white;
        defaultColors.highlightedColor = new Color(0.9f, 0.9f, 0.9f);
        defaultColors.selectedColor = Color.white;
        defaultColors.disabledColor = Color.white;
        buttonA.colors = defaultColors;
        buttonB.colors = defaultColors;
        buttonA.interactable = true;
        buttonB.interactable = true;

        resultText.gameObject.SetActive(false);
        answered = false;
        isOpen = true;

        overlay.SetActive(true);
        paperPanel.SetActive(true);

        Time.timeScale = 0f;

        StopAllCoroutines();
        StartCoroutine(SlideIn());
    }

    IEnumerator SlideIn()
    {
        paperRect.anchoredPosition = new Vector2(paperTargetPos.x, slideStartY);
        float elapsed = 0f;

        while (elapsed < slideDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / slideDuration;
            float easedT = 1f - Mathf.Pow(1f - t, 3f);

            paperRect.anchoredPosition = Vector2.Lerp(
                new Vector2(paperTargetPos.x, slideStartY),
                paperTargetPos,
                easedT
            );

            yield return null;
        }

        paperRect.anchoredPosition = paperTargetPos;
    }

    IEnumerator SlideOut()
    {
        float elapsed = 0f;
        Vector2 startPos = paperRect.anchoredPosition;

        while (elapsed < slideDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / slideDuration;
            float easedT = t * t * t;

            paperRect.anchoredPosition = Vector2.Lerp(
                startPos,
                new Vector2(paperTargetPos.x, slideStartY),
                easedT
            );

            yield return null;
        }

        overlay.SetActive(false);
        paperPanel.SetActive(false);
        isOpen = false;
        Time.timeScale = 1f;
    }

    void OnAnswer(bool choseB)
    {
        if (answered) return;
        answered = true;

        bool correct = (choseB == currentQuestion.correctAnswer);
        bool correctIsB = currentQuestion.correctAnswer;

        ColorBlock correctColors = buttonA.colors;
        correctColors.normalColor = Color.green;
        correctColors.highlightedColor = Color.green;
        correctColors.selectedColor = Color.green;
        correctColors.disabledColor = Color.green;

        ColorBlock wrongColors = buttonA.colors;
        wrongColors.normalColor = Color.red;
        wrongColors.highlightedColor = Color.red;
        wrongColors.selectedColor = Color.red;
        wrongColors.disabledColor = Color.red;

        if (correctIsB)
        {
            buttonB.colors = correctColors;
            buttonA.colors = wrongColors;
        }
        else
        {
            buttonA.colors = correctColors;
            buttonB.colors = wrongColors;
        }

        buttonA.interactable = false;
        buttonB.interactable = false;

        resultText.gameObject.SetActive(true);

        if (correct)
        {
            resultText.text = "Good Job!";
            resultText.color = Color.green;
            ScoreManager.Instance.AddScore(10);

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayCorrect();

            if (ParticleSpawner.Instance != null)
                ParticleSpawner.Instance.SpawnCorrect(
                    Camera.main.transform.position + new Vector3(0, 0, 5f)
                );
        }
        else
        {
            string correctLabel = correctIsB ? currentQuestion.optionB : currentQuestion.optionA;
            resultText.text = "Wrong! Jawaban yang benar: " + correctLabel;
            resultText.color = Color.red;

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayWrong();

            if (ParticleSpawner.Instance != null)
                ParticleSpawner.Instance.SpawnWrong(
                    Camera.main.transform.position + new Vector3(0, 0, 5f)
                );
        }

        completedStatues.Add(currentQuestion.statueIndex);

        if (statueArrow != null)
            statueArrow.AddCompletedStatue(currentQuestion.statueIndex);

        ScoreManager.Instance.CompletePos();

        StartCoroutine(CloseAfterDelay(2f));
    }

    IEnumerator CloseAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        yield return StartCoroutine(SlideOut());
    }
}