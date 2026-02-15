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
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private Material LightsOutOutlineShaderMaterial;

    //[SerializeField] public Image coolDownImage;//All images should be seperate with an event. Now I have to do a dumb solution in the AbilityBar to make this work. We need to come up with a less dumb solution later - Vidar
    public static event Action<float> OnLightOutCoolDown;

    [SerializeField] private float slowMoFactor = 0.5f;
    [SerializeField] private float disableDuration = 5f;

    [SerializeField] private float range;

    [SerializeField] private float cooldown;

    private float timer = 0;
    private float useTimer = 3f; //Seconds you have to use it before its stops
    private float useTimerTick; //BAd name, fix later



    List<(MeshRenderer MeshRenderer, Material[] OldMaterial)> renderers = new();

    private bool throwRay;
    [SerializeField] public bool ShowDebugs;

    private GameObject[] objects;

    [SerializeField] private int HowHighToSearchForParent;
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
         if (PlayerAbilities.Instance.GetAbilityState(PlayerAbility.LightsOut) is false) return;
        if (timer <= 0)
        {
            if (ShowDebugs)
                Debug.Log("Lights Out Activated!");
            Time.timeScale = slowMoFactor;

            objects = FindObjectsWithLayer();
            
            if (ShowDebugs)
                Debug.Log("Found " + objects.Length + " lights to disable.");

            throwRay = true;

            renderers.Clear();

            foreach (GameObject light in objects)
            {
                //_Outline_Scale

                MeshRenderer renderer = light.GetComponent<MeshRenderer>();
                Material[] oldMaterials = renderer.materials;
                Material[] newMaterials = renderer.materials;

                for (int i = 0; i < newMaterials.Length; i++)
                {
                    newMaterials[i] = LightsOutOutlineShaderMaterial;
                    newMaterials[i].SetFloat("_Outline_Scale", 1.05f);
                }

                renderer.materials = newMaterials;
                renderers.Add((renderer, oldMaterials));
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

            if (Input.GetMouseButton(0))
            {
                if (ShowDebugs)
                    Debug.Log("Disable Lights");
                StartCoroutine(DisableLight(objects));

                foreach (var renderer in renderers)
                {
                    renderer.MeshRenderer.materials = renderer.OldMaterial;
                    //renderer.materials[1].SetFloat("_Outline_Scale", 0f);
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
                foreach (var renderer in renderers)
                {
                    renderer.MeshRenderer.materials = renderer.OldMaterial;
                    //renderer.materials[1].SetFloat("_Outline_Scale", 0f);
                }

                throwRay = false;
                Time.timeScale = 1f;
            }

            useTimerTick += Time.deltaTime;
            if(useTimer <= useTimerTick)
            {
                foreach (var renderer in renderers)
                {
                    renderer.MeshRenderer.materials = renderer.OldMaterial;
                    //renderer.materials[1].SetFloat("_Outline_Scale", 0f);
                }
                useTimerTick = 0;

                throwRay = false;
                Time.timeScale = 1f;
                timer = 2f;

                OnLightOutCoolDown?.Invoke(0);

            }
        }
    }

    private GameObject[] FindObjectsWithLayer()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Light");
        List<GameObject> found = new List<GameObject>();

        foreach (GameObject obj in objects)
        {
            float distance = Vector3.Distance(transform.position, obj.transform.position);

            if (distance < range) found.Add(obj);


        }

        return found.ToArray();
    }

    //private GameObject[] FindLights()
    //{
    //    GameObject[] objects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);

    //    List<GameObject> lights = new List<GameObject>();

    //    foreach (GameObject light in objects)
    //    {
    //        if (light.layer == 8)
    //        {
    //            lights.Add(light);
    //        }
    //    }

    //    return lights.ToArray();
    //}

    IEnumerator DisableLight(GameObject[] light)
    {
       
        foreach (GameObject obj in light)
        {
            Light lightComponent = GetHighestParent(obj.transform, HowHighToSearchForParent).GetComponentInChildren<Light>();
            AlarmSensor sensor = GetHighestParent(obj.transform, HowHighToSearchForParent).GetComponentInChildren<AlarmSensor>();

            lightComponent.enabled = false;
            sensor.isDisabled = true;
        }

        yield return new WaitForSeconds(disableDuration);

        foreach (GameObject obj in light)
        {
            Light lightComponent = GetHighestParent(obj.transform, HowHighToSearchForParent).GetComponentInChildren<Light>();
            AlarmSensor sensor = GetHighestParent(obj.transform, HowHighToSearchForParent).GetComponentInChildren<AlarmSensor>();

            lightComponent.enabled = true;
            sensor.isDisabled = false;
        }
    }

    

    private Component FindComponent(Transform startTransform, Type targetType)
    {
        Transform highestTransform = GetHighestParent(startTransform, 2);
        return SearchAfterComponent(highestTransform, targetType);
    }
    private Transform GetHighestParent(Transform startTransform, int maxSearches)
    {
        int currentSearches = 0;
        Transform highestParent = startTransform;
        while(highestParent.parent != null && maxSearches > currentSearches)
        {
            highestParent = highestParent.parent;
            currentSearches++;
        }
        return highestParent;
    }
    private Component SearchAfterComponent(Transform highestTransform, Type targetType)
    {
        List<Transform> AllChildren = GetAllNestedChildren(highestTransform);
        AllChildren.ForEach(c => Debug.Log(c.gameObject.name));
        foreach (var child in AllChildren)
        {
            var comp = child.GetComponent(targetType);
            if(comp != null)
            {
                return child;
            }
        }
        return null;
    }
    public List<Transform> GetAllNestedChildren(Transform parent)
    {
        List<Transform> childrenTransforms = new();

        Queue<Transform> parents = new Queue<Transform>();
        parents.Enqueue(parent);

        while (parents.Count > 0)
        {
            parent = parents.Dequeue();
            childrenTransforms.Add(parent);

            for (int i = 0; i < parent.childCount; i++)
            {
                parents.Enqueue(parent.GetChild(i));
            }
        }
        return childrenTransforms;
    }
}
