using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Image fadePanel;
    public float fadeDuration = 1f;

    void Start()
    {
        fadePanel.raycastTarget = false;
        StartCoroutine(FadeIn());
    }

    public void PlayGame()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayButtonClick();

        StartCoroutine(FadeAndLoad("Gameplay"));
    }

    IEnumerator LoadGameWithDelay()
    {
        yield return new WaitForSeconds(0.3f); // tunggu suara klik selesai
        SceneManager.LoadScene("Gameplay");    // ganti dengan nama scene kamu
    }

    IEnumerator FadeAndLoad(string Gameplay)
    {
        fadePanel.raycastTarget = true;
        yield return StartCoroutine(FadeOut());
        SceneManager.LoadScene(Gameplay);
    }

    IEnumerator FadeIn()
    {
        float elapsed = 0f;
        Color c = Color.black;
        c.a = 1f;
        fadePanel.color = c;
        fadePanel.gameObject.SetActive(true);

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = 1f - Mathf.Clamp01(elapsed / fadeDuration);
            fadePanel.color = c;
            yield return null;
        }

        c.a = 0f;
        fadePanel.color = c;
        fadePanel.gameObject.SetActive(false);
    }

    IEnumerator FadeOut()
    {
        float elapsed = 0f;
        Color c = Color.black;
        c.a = 0f;
        fadePanel.color = c;
        fadePanel.gameObject.SetActive(true);

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Clamp01(elapsed / fadeDuration);
            fadePanel.color = c;
            yield return null;
        }

        c.a = 1f;
        fadePanel.color = c;
    }
}