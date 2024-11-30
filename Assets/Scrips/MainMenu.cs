using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public ScreenFader screenFader;

    public void PLayGame()
    {
        StartCoroutine(FadeOutAndChangeScene());
    }
    private IEnumerator FadeOutAndChangeScene()
    {
        screenFader.FadeIn();

        yield return new WaitForSeconds(screenFader.fadeDuration);

        SceneManager.LoadSceneAsync(1);
    }
}
