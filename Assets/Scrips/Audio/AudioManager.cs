using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("----------Audio Source----------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;
    [Header("----------Audio Clip----------")]
    public AudioClip background;
    public AudioClip talking;
    [Header("----------Fade Settings----------")]
    [SerializeField] float fadeDuration = 1.0f;

    private void Start()
    {
        StartCoroutine(FadeIn(musicSource, background));
    }
    public void PlaySFX(AudioClip clip)
    {
        StartCoroutine(FadeIn(SFXSource, clip));
    }
    public void StopSFX()
    {
        StartCoroutine(FadeOut(SFXSource));
    }
    private IEnumerator FadeIn(AudioSource audioSource, AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
        audioSource.volume = 0f; // Start from silence

        // Gradually increase volume to 1 over the fadeDuration
        float timeElapsed = 0f;
        while (timeElapsed < fadeDuration)
        {
            audioSource.volume = Mathf.Lerp(0f, 1f, timeElapsed / fadeDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        audioSource.volume = 1f; // Ensure volume is set to max at the end
    }
    private IEnumerator FadeOut(AudioSource audioSource)
    {
        float timeElapsed = 0f;
        float startVolume = audioSource.volume;

        // Gradually decrease volume to 0 over the fadeDuration
        while (timeElapsed < fadeDuration)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0f, timeElapsed / fadeDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        audioSource.volume = 0f; // Ensure volume is set to 0 at the end
        audioSource.Stop();
    }
}
