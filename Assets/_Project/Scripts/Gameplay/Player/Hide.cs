using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Hide : MonoBehaviour
{
    [SerializeField] private float range;
    private bool isHiding = false;
    private Vector3 playerStartPoint;

    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private InputAction hide;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hide = inputActions.FindActionMap("Player").FindAction("Interact");
        hide.performed += Hide_Performed;
    }
    private void Hide_Performed(InputAction.CallbackContext obj)
    {
        if (isHiding)
        {
            RevealFromObject();
            return;
        }


        Collider[] colliders = Physics.OverlapSphere(transform.position, range);

        List<GameObject> thingsToHideIn = new List<GameObject>();

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer("Hideable"))
            {
                thingsToHideIn.Add(collider.gameObject);
            }
        }

        if (thingsToHideIn.Count > 0)
        {
            HideInObject(thingsToHideIn[0]);
        }

    }
    private void OnDisable()
    {
        hide.performed -= Hide_Performed;
    }
    private void HideInObject(GameObject thingsToHideIn)
    {
        isHiding = true;
        gameObject.layer = 9;
        StartCoroutine(HideCoroutine(thingsToHideIn));
    }
    private void RevealFromObject()
    {
        isHiding = false;
        gameObject.layer = 0;
        StartCoroutine(RevealCoroutine());
    }


    IEnumerator HideCoroutine(GameObject thingsToHideIn)
    {
        playerStartPoint = transform.position;

        var startPoint = transform.position;
        var endPoint = thingsToHideIn.transform.position;
        float start = 0;
        float end = 1f;

        while (start < end)
        {
            start += Time.deltaTime;
            transform.position = Vector3.Lerp(startPoint, endPoint, start);
            yield return null;
        }
        transform.position = endPoint;
    }
    IEnumerator RevealCoroutine()
    {
        var startPoint = transform.position;
        var endPoint = playerStartPoint;
        float start = 0;
        float end = 1f;

        while (start < end)
        {
            start += Time.deltaTime;
            transform.position = Vector3.Lerp(startPoint, endPoint, start);
            yield return null;
        }
        transform.position = endPoint;
        playerStartPoint = Vector3.zero;
    }
}
