using UnityEngine;
using UnityEngine.InputSystem;

public class SkillTreeState : State
{
    [SerializeField] private GameObject skillTreeUI;
    [SerializeField] private SkillTree skiltree;
    public override void EnterState()
    {
        AIManager.Instance.StopAllAI();
        skillTreeUI.SetActive(true);
        skiltree.UpdateSkillTree();
    }
    public override void UpdateState()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            GameManager.Instance.SwitchState<PlayingState>();
            //SkillPointManager.AddSkillPoints(skillPoints); Or maybe we will have the skillpoints in the GameManager? I don't know
        }

        //Maybe update skill tree animations or whatever
    }
    public override void ExitState()
    {
        AIManager.Instance.ResumeAllAI();
        skillTreeUI.SetActive(false);
    }

    public void CloseSkillTree()
    {
        GameManager.Instance.SwitchState<PlayingState>();
    }
}
