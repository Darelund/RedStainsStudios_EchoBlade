using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class IteractionPopUp : MonoBehaviour
{
    [SerializeField] private CanvasGroup blackScreen;
    [SerializeField] private GameObject[] texts;
    [SerializeField] private float fadeDuration = 1f;

    public bool isDone;


    void Start()
    {
        StartCoroutine(Intermission());
    }

    IEnumerator Intermission()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            blackScreen.alpha = t / fadeDuration;
            yield return null;
        }
        blackScreen.alpha = 1f;

        foreach (var text in texts)
        {
            StartCoroutine(FadeIn(text.GetComponent<CanvasGroup>()));
            yield return new WaitForSeconds(3f);
        }

        yield return new WaitForSeconds(2f);

        foreach (var text in texts)
        {
            StartCoroutine(FadeOut(text.GetComponent<CanvasGroup>()));
        }

        yield return new WaitForSeconds(2f);


    }
    IEnumerator FadeIn(CanvasGroup canvasGroup)
    {
        float t = 0f;

        GameObject.FindAnyObjectByType<Volume>().profile.TryGet(out DepthOfField d);
        d.active = true;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = t / fadeDuration;
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

    IEnumerator FadeOut(CanvasGroup canvasGroup)
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = 1 - (t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0f;
        GameObject.FindAnyObjectByType<Volume>().profile.TryGet(out DepthOfField d);
        d.active = false;
    }
}
