using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class RotateingLever : MonoBehaviour
{
    private bool isPulled = false;
    private bool canPull = false;

    [SerializeField] private InputActionAsset actionMap;
    [SerializeField] private InputAction pullAction;

    [SerializeField] private GameObject door;
    [SerializeField] private AudioClip doorClip;

    [SerializeField] private float axisRotation;
    [SerializeField] private float rotationDuration;

    Quaternion targetRotation;


    private void Start()
    {
        targetRotation = Quaternion.Euler(0, axisRotation, 0);

        pullAction = actionMap.FindActionMap("Player").FindAction("Interact");

        pullAction.performed += PullAction_performed;
    }

    private void PullAction_performed(InputAction.CallbackContext obj)
    {
        if (canPull && !isPulled)
        {
            StartCoroutine(RotateDoor());
            isPulled = true;
        }
    }

    private IEnumerator RotateDoor()
    {
        door.gameObject.GetComponent<AudioSource>().PlayOneShot(doorClip);
        while (rotationDuration >= 0)
        {
            rotationDuration -= Time.deltaTime;

            door.transform.rotation = Quaternion.Lerp(door.transform.rotation, targetRotation, rotationDuration);
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Movement>() != null && !isPulled) //Find player gameobject
        {
            canPull = true;
            Debug.Log("Can Pull Lever");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Movement>() != null && !isPulled) //Find player gameobject
        {
            canPull = false;
            Debug.Log("Can't Pull Lever Anymore");
        }
    }
}
