using System.Collections;
using UnityEngine;

public class AngelLight : MonoBehaviour
{
    [SerializeField] private Light Spotlight;
    [SerializeField] private float Duration = 1.5f;
    [SerializeField] private float T_Intensity = 200f;
    [SerializeField] private float targetVolume = 0.3f;

    AudioSource src;
    Coroutine running;

    void Awake()
    {
        src = GetComponent<AudioSource>();
        if (src != null) { src.playOnAwake = false; src.volume = 0f; }
        if (Spotlight != null) Spotlight.intensity = 0f;
    }

    void OnTriggerEnter(Collider other)
    {
        // detect player by Movement component (works regardless of tags)
        if (other.GetComponentInParent<Movement>() == null) return;

        if (running != null) StopCoroutine(running);
        if (src != null) { src.Play(); }
        running = StartCoroutine(LightAndAudioOn());
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<Movement>() == null) return;

        if (running != null) StopCoroutine(running);
        running = StartCoroutine(LightAndAudioOff());
    }

    IEnumerator LightAndAudioOn()
    {
        float t = 0f;
        float fromI = Spotlight != null ? Spotlight.intensity : 0f;
        float fromV = src != null ? src.volume : 0f;

        while (t < Duration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / Duration);
            if (Spotlight != null) Spotlight.intensity = Mathf.Lerp(fromI, T_Intensity, k);
            if (src != null) src.volume = Mathf.Lerp(fromV, targetVolume, k);
            yield return null;
        }

        if (Spotlight != null) Spotlight.intensity = T_Intensity;
        if (src != null) src.volume = targetVolume;
        running = null;
    }

    IEnumerator LightAndAudioOff()
    {
        float t = 0f;
        float fromI = Spotlight != null ? Spotlight.intensity : T_Intensity;
        float fromV = src != null ? src.volume : targetVolume;

        while (t < Duration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / Duration);
            if (Spotlight != null) Spotlight.intensity = Mathf.Lerp(fromI, 0f, k);
            if (src != null) src.volume = Mathf.Lerp(fromV, 0f, k);
            yield return null;
        }

        if (Spotlight != null) Spotlight.intensity = 0f;
        if (src != null) { src.volume = 0f; src.Stop(); }
        running = null;
    }
}
