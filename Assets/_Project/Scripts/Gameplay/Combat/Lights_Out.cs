using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class Lights_Out : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private InputAction lightsOut;
    [SerializeField] private Material material;

    //[SerializeField] public Image coolDownImage;//All images should be seperate with an event. Now I have to do a dumb solution in the AbilityBar to make this work. We need to come up with a less dumb solution later - Vidar
    public static event Action<float> OnLightOutCoolDown;

    [SerializeField] private float slowMoFactor = 0.5f;
    [SerializeField] private float disableDuration = 5f;

    [SerializeField] private float cooldown;

    private float timer = 0;

    List<MeshRenderer> renderers = new List<MeshRenderer>();

    private bool throwRay;
    [SerializeField] public bool ShowDebugs;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //if(coolDownImage != null)
        //coolDownImage.fillAmount = 1;

        lightsOut = inputActions.FindActionMap("Player").FindAction("LightsOut");

        lightsOut.performed += LightsOut_performed;
    }

    private void LightsOut_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
       // if (PlayerAbilities.Instance.GetAbilityState(PlayerAbility.LightsOut) is false) return;
        if (timer <= 0)
        {
            if(ShowDebugs)
            Debug.Log("Lights Out Activated!");
            Time.timeScale = slowMoFactor;

            GameObject[] objects = FindLights();
            if (ShowDebugs)
                Debug.Log("Found " + objects.Length + " lights to disable.");

            throwRay = true;

            renderers.Clear();

            foreach (GameObject light in objects)
            {
                //_Outline_Scale

                MeshRenderer renderer = light.GetComponent<MeshRenderer>();

                renderer.materials[1].SetFloat("_Outline_Scale", 1.05f);
                renderers.Add(renderer);
            }

            
            
        }
        
    }
    private void OnDisable()
    {
        lightsOut.performed -= LightsOut_performed;
    }
    private void Update()
    {
        if (timer >= 0)
        {
            timer -= Time.deltaTime;
            OnLightOutCoolDown?.Invoke(Mathf.Lerp(0, 1, timer));
            //if (coolDownImage != null)
            //    coolDownImage.fillAmount = Mathf.Lerp(0, 1, timer);
        }

        if (throwRay)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 999, 1 << 8) && Input.GetMouseButton(0))
            {
                if (ShowDebugs)
                    Debug.Log("Selected Light");
                StartCoroutine(DisableLight(hitInfo.collider.gameObject,
                    hitInfo.collider.gameObject.GetComponentInChildren<AlarmSensor>()));

                foreach (MeshRenderer renderer in renderers)
                {
                    renderer.materials[1].SetFloat("_Outline_Scale", 0f);
                }

                throwRay = false;
                Time.timeScale = 1f;

                if (PlayerAbilities.Instance.GetAbilityState(PlayerAbility.AbilityHaste))
                {
                    timer = cooldown * 0.8f;
                }
                else
                {
                    timer = cooldown;
                }
                OnLightOutCoolDown?.Invoke(0);
                //if (coolDownImage != null)
                //    coolDownImage.fillAmount = 0;


            }
            else if (Input.GetMouseButton(1))
            {
                foreach (MeshRenderer renderer in renderers)
                {
                    renderer.materials[1].SetFloat("_Outline_Scale", 0f);
                }

                throwRay = false;
                Time.timeScale = 1f;
            }
        }
    }

    private GameObject[] FindLights()
    {
        GameObject[] objects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        List<GameObject> lights = new List<GameObject>();

        foreach (GameObject light in objects)
        {
            if (light.layer == 8)
            {
                lights.Add(light);
            }
        }

        return lights.ToArray();
    }

    IEnumerator DisableLight(GameObject light, AlarmSensor sensor)
    {
        Light lightComponent = light.GetComponentInChildren<Light>();

        lightComponent.enabled = false;
        sensor.isDisabled = true;

        yield return new WaitForSeconds(disableDuration);

        lightComponent.enabled = true;
        sensor.isDisabled = false;
    }
}
