using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    public Image fadeImage; // Reference to the Image component
    public CanvasGroup canvasGroup; //Refểnce to Canvas Group
    public float fadeDuration = 2f; // Duration of the fade effect
    public bool fadeOnStart = true;
    private void Start()
    {
        // Optional: Start with a fade-in
        if (fadeOnStart)
        {
            FadeOut();
        }
    }

    // Fade-in effect
    public void FadeIn()
    {
        StartCoroutine(Fade(1, 0)); // Fade from opaque to transparent
    }

    // Fade-out effect
    public void FadeOut()
    {
        StartCoroutine(Fade(0, 1)); // Fade from transparent to opaque
    }

    // Coroutine for fading
    private IEnumerator Fade(float startAlpha, float endAlpha)
    { 
        float elapsedTime = 0f;
        Color fadeColor = fadeImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            fadeColor.a = newAlpha;
            fadeImage.color = fadeColor;
            yield return null;
        }

        // Ensure the final alpha is set
        fadeColor.a = endAlpha;
        fadeImage.color = fadeColor;
    }
}
