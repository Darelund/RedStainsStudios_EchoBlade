using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Tombstone : MonoBehaviour, ISavable
{
    [SerializeField] private int skillPoints;
    [SerializeField] private SkillTree skillTree;
    //[SerializeField] private Material nonActivatedMat;
    [SerializeField] private Material activatedMat;
    [SerializeField] private MeshRenderer textMeshRenderer;
    [SerializeField] private bool hasBeenActivated = false;
    private GameObject player;

    [SerializeField] private InputActionAsset inputActions;
    private InputAction interact;

    public int Unique_ID;

    private void Start()
    {
        interact = inputActions.FindAction("interact");

        interact.performed += Interact_performed;
    }

    private void Interact_performed(InputAction.CallbackContext obj)
    {
        //if (hasBeenActivated) return; //Nothing to do here, already used

        if (player != null)
        {
            if (hasBeenActivated is false)
                GameManager.Instance.IncreaseSkillPoints(skillPoints); //Or maybe we will have the skillpoints in the GameManager? I don't know
            StartCoroutine(ActivatingCoroutine());

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player = other.gameObject;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            player = null;
        }
    }

    private void Update()
    {
        //if (hasBeenActivated) return; //Nothing to do here, already used

        //if (player != null)
        //{
        //    if (Keyboard.current.spaceKey.wasPressedThisFrame)
        //    {
        //        GameManager.Instance.IncreaseSkillPoints(skillPoints); //Or maybe we will have the skillpoints in the GameManager? I don't know
        //        StartCoroutine(ActivatingCoroutine());
        //    }
        //}
    }
    private IEnumerator ActivatingCoroutine()
    {
        hasBeenActivated = true;
        textMeshRenderer.material = activatedMat;
        yield return new WaitForSeconds(1.0f);
        GameManager.Instance.SwitchState<SkillTreeState>();
        //Destroy(gameObject); //maybe make it fade away later. So as you leave the SkillTreeUI you see it fade away
        //skillTree.transform.GetChild(0).gameObject.SetActive(true);
    }

    public void Save(GameData gameData)
    {
        //Check for altar save
        foreach (AltarData altar in gameData.altarData)
        {
            if(altar.ID == Unique_ID)
            {
                altar.IsUsed = hasBeenActivated;
                return;
            }
        }
        //No altar save
        gameData.altarData.Add(new AltarData() { ID = Unique_ID, IsUsed = hasBeenActivated });
       
    }

    public void Load(GameData gameData)
    {
        if (gameData.altarData == null) return;
        if (gameData.altarData.Count <= 0) return;


        foreach (AltarData altar in gameData.altarData)
        {
            if (altar.ID == Unique_ID)
            {
                hasBeenActivated = altar.IsUsed;
                return;
            }
        }
    }
    //private IEnumerator LightUpCoroutine()
    //{
    //    float start = 0;
    //    float end = 10;

    //    textMeshRenderer.material = activatedMat;

    //    while (start < end)
    //    {
    //        start += Time.deltaTime * 2;
    //        textMeshRenderer.material.color = new Color(textMeshRenderer.material.color.r, textMeshRenderer.material.color.g, textMeshRenderer.material.color.b, 255) * start;
    //        yield return null;
    //    }
    //    Debug.Log("Finished");
    //}
}
