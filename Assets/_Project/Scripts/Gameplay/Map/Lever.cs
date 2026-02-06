using UnityEngine;
using UnityEngine.InputSystem;

public class Lever : MonoBehaviour
{
    private bool isPulled = false;
    private bool canPull = false;

    [SerializeField] private InputActionAsset actionMap;
    [SerializeField] private InputAction pullAction;

    [SerializeField] private GameObject door;

    [SerializeField] private CapsuleCollider exitCollider;


    private void Start()
    {
        pullAction = actionMap.FindActionMap("Player").FindAction("Interact");

        pullAction.performed += PullAction_performed;
    }

    private void PullAction_performed(InputAction.CallbackContext context)
    {
        Debug.Log("Tried to pull");
        if (canPull)
        {
            //QuestLog.instance.ProgressQuest();
            isPulled = true;
            door.SetActive(false); //Open the door by disabling it
            exitCollider.enabled = true; //Enable exit collider
            Debug.Log("Lever Pulled, Door Opened");
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
