using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScreenFaderTest : MonoBehaviour
{
    public Image fadeImage; // Reference to the Image component
    public float fadeDuration = 2f; // Duration of the fade effect
    public bool startWithFadeIn; // Set in the Inspector to control fade behavior

    private void Start()
    {
        if (startWithFadeIn)
        {
            FadeIn();
        }
        else
        {
            FadeOut(null);
        }
    }

    public void FadeIn()
    {
        StartCoroutine(Fade(1, 0, null)); // Fade from opaque to transparent
    }

    public void FadeOut(string nextScene)
    {
        StartCoroutine(Fade(0, 1, nextScene)); // Fade from transparent to opaque
    }

    private IEnumerator Fade(float startAlpha, float endAlpha, string nextScene)
    {
        float elapsedTime = 0f;
        Color fadeColor = fadeImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            fadeColor.a = newAlpha;
            fadeImage.color = fadeColor; // Only modify the fade image

            yield return null;
        }

        // Ensure the final alpha is set
        fadeColor.a = endAlpha;
        fadeImage.color = fadeColor;

        // Load the next scene if specified
        if (!string.IsNullOrEmpty(nextScene))
        {
            SceneManager.LoadScene(nextScene);
        }
    }
}
