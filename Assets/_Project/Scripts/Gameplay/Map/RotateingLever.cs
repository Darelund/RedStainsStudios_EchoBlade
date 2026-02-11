using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class RotateingLever : MonoBehaviour
{
    private bool isPulled = false;
    private bool canPull = false;

    [SerializeField] private InputActionAsset actionMap;
    [SerializeField] private InputAction pullAction;

    [SerializeField] private GameObject player;
    [SerializeField] private CinemachineCamera camera;
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
        GameManager.Instance.SwitchState<CutsceneState>();
        yield return new WaitForSeconds(0.5f);
        camera.Target.TrackingTarget = door.transform;
        yield return new WaitForSeconds(1f);
        door.gameObject.GetComponent<AudioSource>().PlayOneShot(doorClip);
        door.gameObject.GetComponent<Animator>().SetTrigger("OpenDoor");
        yield return new WaitForSeconds(2f);
        camera.Target.TrackingTarget = player.transform;
        GameManager.Instance.SwitchState<PlayingState>();
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
