using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Node : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISavable
{
    public SkillTree skillTree;
    public PlayerAbility Value; //Type should be ability later
    public Node[] Children; //Should we use Tree or BinaryTree?
    public bool IsUnlocked;
    public int skillPointsNeeded;
    public bool IsUsed;
    public Image OuterBorder;
    public Button button;
    //public TMP_Text skillpointsNeededText;
    [SerializeField] private List<Image> ConnectingPaths;
    public Image skillIcon;
    public Sprite unlockedSprite;
    public Sprite selectedSprite;
    public Sprite lockedSprite;
    public Sprite skillbarSprite;
    public Sprite skillbarCooldownSprite;
    public bool haveSkillbarIcon;
    public int Unique_ID;
    private string skillName;

    [TextArea()]
    public string description;
    public void Awake()
    {
        skillTree = GameObject.Find("Canvas").GetComponent<SkillTree>();
        button.onClick.AddListener(() => Use());
        //gameObject.SetActive(false);
        skillName = gameObject.name;
        // skillpointsNeededText.text = skillPointsNeeded.ToString();
        skillIcon.sprite = lockedSprite;
    }
    public void ResetNode()
    {
        skillIcon.sprite = lockedSprite;
        IsUsed = false;
        IsUnlocked = false;
        ResetConnectingPaths();
    }
    public void Unlock()
    {
        //Debug.Log("Unlocked");
        IsUnlocked = true;
       // skillIcon.sprite
    }
    public void Use()
    {
        if (IsUnlocked is false) return;

        if (IsUsed) return; //Don't use something that has already been used
        //Check for skillpoints available
        if (skillPointsNeeded > GameManager.Instance.GetCurrentSkillPoints()) return;

        IsUsed = true;

        //Update GameManager and SkillTree UI and Update PlayerAbility
        GameManager.Instance.DecreaseSkillPoints(skillPointsNeeded);
        skillTree.UpdateSkillTreesText();
        GameObject.FindAnyObjectByType<PlayerAbilities>().ActivateAbility(Value);

        skillIcon.sprite = selectedSprite;
        StartCoroutine(UsingCoroutine());
        foreach (var child in Children)
        {
            if (child != null)
                child.Unlock();
        }

        if(haveSkillbarIcon)
        {
            AbilityBarManager.Instance.SetNewIcon(skillbarSprite, skillbarCooldownSprite, Unique_ID);
        }
      


        skillTree.UpdateSkillTreesText();
    }
    private IEnumerator UsingCoroutine()
    {
        var startColor = OuterBorder.color;
        var endColor = Color.darkRed;
        float start = 0f;
        float end = 1f;
        float changeMultiplier = 2f;
        //while (start < end)
        //{
        //    start += Time.deltaTime * changeMultiplier;
        //    OuterBorder.color = Color.Lerp(startColor, endColor, start);
        //    yield return null;
        //}
        ConnectingPaths.ForEach(c => c.gameObject.SetActive(true));
        start = 0;
        while (start < end)
        {
            start += Time.deltaTime * changeMultiplier;
           
            foreach (var child in ConnectingPaths)
            {
                child.color = Color.Lerp(startColor, endColor, start);
                yield return null;
            }
        }
    }
    public void ResetConnectingPaths()
    {
        ConnectingPaths.ForEach(c => { 
            c.gameObject.SetActive(false);
            c.color = Color.white;
        });
    }
    //TODO: Fix the code here, I just made it quickly to make it work
    public void OnPointerEnter(PointerEventData eventData)
    {
        GameObject DescriptionBoxUI = GameObject.Find("DescriptionBoxUI");

        DescriptionBoxUI.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = gameObject.name;
        DescriptionBoxUI.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = description;

        DescriptionBoxUI.transform.GetChild(0).gameObject.SetActive(true);
        DescriptionBoxUI.transform.GetChild(1).gameObject.SetActive(true);

        //Ignore skills that are already bought
        if (skillIcon.sprite != selectedSprite)
        {
            skillIcon.sprite = unlockedSprite;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GameObject DescriptionBoxUI = GameObject.Find("DescriptionBoxUI");

        DescriptionBoxUI.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = "";
        DescriptionBoxUI.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = "";

        DescriptionBoxUI.transform.GetChild(0).gameObject.SetActive(false);
        DescriptionBoxUI.transform.GetChild(1).gameObject.SetActive(false);

        //Ignore skills that are already bought
        if (skillIcon.sprite != selectedSprite)
        {
            skillIcon.sprite = IsUsed ? selectedSprite : lockedSprite;
        }
    }

    public void Save(GameData gameData)
    {
        //Debug.Log("What about now?");
        foreach (SkillsNodeData node in gameData.SkillsNodeData)
        {
            if (node.ID == Unique_ID)
            {
                node.IsUsed = IsUsed;
                node.IsUnlocked = IsUnlocked;
                node.SkillName = skillName;
                if (ConnectingPaths == null) continue;
                if (node.ConnectingPaths == null) continue;
                for (int i = 0; i < node.ConnectingPaths.Count; i++)
                {
                    if (node.ConnectingPaths == null) return;
                    node.ConnectingPaths[i] = IsUsed is true ? true : false;
                }
                return;
            }
        }
       // Debug.Log("Came here");
        var newBoolList = ConnectingPaths.ConvertAll(i => IsUsed is true);
        gameData.SkillsNodeData.Add(new SkillsNodeData() { ID = Unique_ID, SkillName = skillName, IsUsed = IsUsed, IsUnlocked = IsUnlocked, ConnectingPaths = newBoolList });
    }

    public void Load(GameData gameData)
    {
        if (gameData.SkillsNodeData == null) return;
        if (gameData.SkillsNodeData.Count <= 0) return;


        foreach (SkillsNodeData node in gameData.SkillsNodeData)
        {
            if (node.ID == Unique_ID)
            {
                IsUsed = node.IsUsed;
                IsUnlocked = node.IsUnlocked;
                if(IsUsed)
                {
                    skillIcon.sprite = selectedSprite;
                }
                if (ConnectingPaths == null) continue;
                for (int i = 0; i < node.ConnectingPaths.Count; i++)
                {
                    if (node.ConnectingPaths[i] is true)
                    {
                        ConnectingPaths[i].color = Color.darkRed;
                        ConnectingPaths[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        ConnectingPaths[i].color = Color.white;
                    }
                }
                return;
            }
        }
    }
}

