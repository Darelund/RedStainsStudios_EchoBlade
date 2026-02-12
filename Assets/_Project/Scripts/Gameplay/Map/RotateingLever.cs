using System.Collections;
using Unity.AI.Navigation;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class RotateingLever : MonoBehaviour, ISavable
{
    private bool isPulled = false;
    private bool canPull = false;
    private AudioSource audioSource;
    private string Unique_ID;

    [SerializeField] private bool shouldProgressQuest;
    [SerializeField] private bool shouldCompleteMainObjective;
    [SerializeField] private GameObject objectiveCompleteScreen;
    [SerializeField] private bool shouldUpdateNavMesh;
    [SerializeField] private bool isDaggerblade;
    [SerializeField] private GameObject daggerblade;
    [SerializeField] private NavMeshSurface navMeshSurface;

    [SerializeField] private InputActionAsset actionMap;
    [SerializeField] private InputAction pullAction;

    [SerializeField] private GameObject player;
    [SerializeField] private CinemachineCamera camera;
    [SerializeField] private GameObject lever;
    [SerializeField] private GameObject door;
    [SerializeField] private AudioClip leverClip;
    [SerializeField] private AudioClip doorClip;

    [SerializeField] private float axisRotation;
    [SerializeField] private float rotationDuration;

    Quaternion targetRotation;


    private void Start()
    {
        Unique_ID = gameObject.name + "_" + SceneManager.GetActiveScene().name;

        targetRotation = Quaternion.Euler(0, axisRotation, 0);

        pullAction = actionMap.FindActionMap("Player").FindAction("Interact");

        pullAction.performed += PullAction_performed;

        audioSource = GetComponent<AudioSource>();

        navMeshSurface = FindAnyObjectByType<NavMeshSurface>();
    }

    private void PullAction_performed(InputAction.CallbackContext obj)
    {
        if (canPull && !isPulled)
        {
            StartCoroutine(PullLever(1f));
            StartCoroutine(RotateDoor());
            isPulled = true;
            if (shouldProgressQuest)
            {
                QuestLog.instance.ProgressQuest();
            }
            
        }
    }

    private IEnumerator PullLever(float duration) 
    {
        if (isDaggerblade)
        {
            daggerblade.SetActive(false);
            yield return null;
        }

        else
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
    }
    
    private IEnumerator RotateDoor()
    {
        GameManager.Instance.SwitchState<CutsceneState>();
        audioSource.PlayOneShot(leverClip);
        yield return new WaitForSeconds(1.5f);
        camera.Target.TrackingTarget = door.transform;
        yield return new WaitForSeconds(1f);
        door.gameObject.GetComponent<AudioSource>().PlayOneShot(doorClip);
        door.gameObject.GetComponent<Animator>().SetTrigger("OpenDoor");
        yield return new WaitForSeconds(2f);
        camera.Target.TrackingTarget = player.transform;
        if (shouldUpdateNavMesh)
        navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);

        if(shouldCompleteMainObjective)
        {
            objectiveCompleteScreen.SetActive(true);
            yield return new WaitForSeconds(7f);
        }

        yield return new WaitForSeconds(0.6f);
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

    public void Save(GameData gameData)
    {
        if (lever == null || door == null) return; //Don't care about doors not used
        foreach (RotatingLeverData lever in gameData.RotatingLeverData)
        {
            if (lever.ID == Unique_ID)
            {
                lever.IsPulled = isPulled;
                lever.LeverRotation = this.lever.transform.localEulerAngles;
                return;
            }
        }
        gameData.RotatingLeverData.Add(new RotatingLeverData() { ID = Unique_ID, IsPulled = isPulled, LeverRotation = lever.transform.localEulerAngles});
    }

    public void Load(GameData gameData)
    {
        if (gameData.RotatingLeverData == null) return;
        if (gameData.RotatingLeverData.Count <= 0) return;
        //if (lever == null || door == null) return; //Don't care about doors not used
        Debug.Log("Is here");
        foreach (RotatingLeverData lever in gameData.RotatingLeverData)
        {
            if (lever.ID == gameObject.name + "_" + SceneManager.GetActiveScene().name)
            {
                isPulled = lever.IsPulled;
                if (isPulled)
                    door.GetComponent<Animator>().SetTrigger("OpenDoor");
                this.lever.transform.localEulerAngles = lever.LeverRotation;
                return;
            }
        }
    }
}