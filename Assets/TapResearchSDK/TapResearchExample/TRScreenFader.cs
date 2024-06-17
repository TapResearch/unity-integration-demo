using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class TRScreenFader : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 1.0f;

    public void FadeToBlack(Action onComplete)
    {
        Debug.Log("Unity C# TRScreenFader: FadeToBlack");
        StartCoroutine(Fade(0, 1, onComplete));
    }

    public void FadeFromBlack(Action onComplete)
    {
        Debug.Log("Unity C# TRScreenFader: FadeFromBlack");
        StartCoroutine(Fade(1, 0, onComplete));
    }

    private IEnumerator Fade(float startAlpha, float endAlpha, Action onComplete)
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            color.a = newAlpha;
            fadeImage.color = color;
            yield return null;
        }

        color.a = endAlpha;
        fadeImage.color = color;

        onComplete.Invoke();
    }

    public void SetAlpha(float alpha)
    {
        Debug.Log("Unity C# TRScreenFader: SetAlpha " + alpha);
        Color color = fadeImage.color;
        color.a = alpha;
        fadeImage.color = color;
    }

}
