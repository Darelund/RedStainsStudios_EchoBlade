using UnityEngine;
using System.Collections;

public class PullLever : MonoBehaviour
{
    [SerializeField] private Transform lever;
    [SerializeField] private float rotationDuration = 1f;
    
    public void StartPull()
    {
        StartCoroutine(PullLeverAction(1f));
    }
    
    private IEnumerator PullLeverAction(float duration)
    {
        float elapsed = 0f;
        float startAngle = -75f;
        float endAngle = 75f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float angle = Mathf.Lerp(startAngle, endAngle, t);

            Vector3 e = lever.localEulerAngles;
            e.x = angle;
            lever.localEulerAngles = e;

            yield return null;
        }

        Vector3 end = lever.localEulerAngles;
        end.x = endAngle;
        lever.localEulerAngles = end;
    }
}