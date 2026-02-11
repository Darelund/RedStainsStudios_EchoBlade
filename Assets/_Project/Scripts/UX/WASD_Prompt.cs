using System.Collections;
using TMPro;
using UnityEngine;

public class WASD_Prompt : MonoBehaviour
{

    [SerializeField] private TMP_Text prompt;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(ShowPrompt());
        }
    }

    IEnumerator ShowPrompt()
    {
        float alpha = 0f;

        while(alpha < 1f)
        {
            alpha += Time.deltaTime;
            prompt.color = new Color(prompt.color.r, prompt.color.g, prompt.color.b, alpha);
            yield return null;
        }
        yield return null;
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            StartCoroutine(HidePrompt());
        }
    }

    IEnumerator HidePrompt()
    {
        float alpha = 1f;
        while (alpha > 0f)
        {
            alpha -= Time.deltaTime;
            prompt.color = new Color(prompt.color.r, prompt.color.g, prompt.color.b, alpha);
            yield return null;
        }
        yield return null;
    }

}
