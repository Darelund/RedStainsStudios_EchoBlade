using UnityEngine;
using UnityEngine.InputSystem.XR;

public class LightChanger : MonoBehaviour
{
    public float angle;
    public float range;

    Light spotLight;
    Gradient gradient;

    private void Awake()
    {
        spotLight = GetComponentInChildren<Light>();
        spotLight.color = Color.yellow;

        spotLight.spotAngle = angle;
        spotLight.innerSpotAngle = angle;
        //spotLight.enableSpotReflector = true;

        spotLight.range = range;

        gradient = new Gradient();

        // Blend color from red at 0% to blue at 100%
        var colors = new GradientColorKey[3];
        colors[0] = new GradientColorKey(Color.white, 0.0f);
        colors[1] = new GradientColorKey(Color.yellow, 0.4f);
        colors[2] = new GradientColorKey(Color.red, 0.8f);

        var alphas = new GradientAlphaKey[3];
        alphas[0] = new GradientAlphaKey(1.0f, 0.0f);
        alphas[1] = new GradientAlphaKey(1.0f, 1.0f);

        gradient.SetKeys(colors, alphas);

        // What's the color at the relative time 0.25 (25%) ?
       // Debug.Log(gradient.Evaluate(0.25f));
       // hehehe russian was here

        //norok. sunatate

        //namaste
    }


    public void ChangeVisibilityColor(float timeInSight)
    {
        //Debug.Log("Changing light");
        //if (timeInSight > 0f && timeInSight < 0.5f)
        //{
        //    spotLight.color = Color.Lerp(Color.white, Color.yellow, timeInSight); //0 - 0.5f
        //}
        //else if (timeInSight >= 0.5f && timeInSight <= 0.8f)
        //{
        //    spotLight.color = Color.Lerp(Color.yellow, Color.red, timeInSight - 1);// 0.5 - 0.8
        //}
        spotLight.color = gradient.Evaluate(timeInSight);
    }
}
