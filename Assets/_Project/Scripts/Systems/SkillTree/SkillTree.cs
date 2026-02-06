using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillTree : MonoBehaviour
{
    [SerializeField] private List<Tree> trees;
    [SerializeField] private GameObject skilltreeUI;
    [SerializeField] private TMP_Text skillPointsText;
   

    private void Start()
    {
        UpdateSkillTree();

        skilltreeUI.SetActive(false);
    }


    public void UpdateSkillTree()
    {
        skillPointsText.text = $"Skill Points: {GameManager.Instance.GetCurrentSkillPoints().ToString()}";

        //foreach (var tree in trees)
        //{
        //    tree.ShowSkillTree(tree.Root);
        //}
    }
}
