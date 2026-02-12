using System.Collections;
using UnityEngine;

public class AngelLight : MonoBehaviour
{
    [Header("Light Settings")]
    [SerializeField] private Light Spotlight;
    [SerializeField] private float Duration = 1.5f;
    [SerializeField] private float T_Intensity = 200f;
    [Space(10)]
    [Header("Sound Settings")]
    [SerializeField] private 
        
    
    private Coroutine running;

    private void OnTriggerEnter(Collider other)
    {
        if (running != null) StopCoroutine(running);
        running = StartCoroutine(LightOn());
    }

    private void OnTriggerExit(Collider other)
    {
        if (running != null) StopCoroutine(running);
        running = StartCoroutine(LightOff());
    }
    
    IEnumerator LightOn()
    {
        float t = 0f;
        while (t < Duration)
        {
            t += Time.deltaTime;
            Spotlight.intensity = Mathf.Lerp(0f, T_Intensity, t);
            yield return null;
        }
        Spotlight.intensity = T_Intensity;
        running = null;
    }

    IEnumerator LightOff()
    {
        float t = 0f;
        while (t < Duration)
        {
            t += Time.deltaTime;
            Spotlight.intensity = Mathf.Lerp(T_Intensity, 0f, t);
            yield return null;
        }
        Spotlight.intensity = 0f;
        running = null;
    }

}

