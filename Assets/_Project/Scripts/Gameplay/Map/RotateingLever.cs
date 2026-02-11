using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private GameObject lever;
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
            StartCoroutine(PullLever(1f));
            StartCoroutine(RotateDoor());
            isPulled = true;
        }
    }

    private IEnumerator PullLever(float duration) 
    {
        if (duration <= 0f) duration = 0.01f;
        
        float timeElapsed = 0f;
        float StartAngle = -75;
        float EndAngle = 75;

        while (timeElapsed < rotationDuration)
        {
            timeElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(timeElapsed / duration);
            float angle = Mathf.Lerp(StartAngle, EndAngle, t);
            
            Vector3 Euler = lever.transform.localEulerAngles;
            Euler.x = angle;
            lever.transform.localEulerAngles = Euler;
            
            yield return null;
        }
        
        Vector3 finalEuler = lever.transform.localEulerAngles;
        finalEuler.x = EndAngle;
        lever.transform.localEulerAngles = finalEuler;
    }
    
    private IEnumerator RotateDoor()
    {
        GameManager.Instance.SwitchState<CutsceneState>();
        yield return new WaitForSeconds(1.5f);
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
