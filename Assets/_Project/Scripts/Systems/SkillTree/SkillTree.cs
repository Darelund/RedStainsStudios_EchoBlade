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
        UpdateSkillTreesText();

        skilltreeUI.SetActive(false);
    }


    public void UpdateSkillTreesText()
    {
        skillPointsText.text = $"Skill Points: {GameManager.Instance.GetCurrentSkillPoints().ToString()}";

        //foreach (var tree in trees)
        //{
        //    tree.ShowSkillTree(tree.Root);
        //}
    }
    public void ResetSkillTrees()
    {
        foreach (var tree in trees)
        {
            tree.ResetSkillTree(tree.Root);
        }
    }
}
