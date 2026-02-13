using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SkillTreeState : State
{
    [SerializeField] private GameObject skillTreeUI;
    [SerializeField] private SkillTree skiltree;
    public override void EnterState()
    {
        AIManager.Instance.StopAllAI();
        skillTreeUI.GetComponent<CanvasGroup>().alpha = 1;
        skillTreeUI.GetComponent<CanvasGroup>().interactable = true;
        skillTreeUI.GetComponent<CanvasGroup>().blocksRaycasts = true;
        skiltree.UpdateSkillTreesText();
        GameObject.FindAnyObjectByType<Volume>().profile.TryGet(out DepthOfField d);
        d.active = true;
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
        skillTreeUI.GetComponent<CanvasGroup>().alpha = 0;
        skillTreeUI.GetComponent<CanvasGroup>().interactable = false;
        skillTreeUI.GetComponent<CanvasGroup>().blocksRaycasts = false;
        AIManager.Instance.ResumeAllAI();
      //  skillTreeUI.SetActive(false);
        GameObject.FindAnyObjectByType<Volume>().profile.TryGet(out DepthOfField d);
        d.active = false;
    }

    public void CloseSkillTree()
    {
        GameManager.Instance.SwitchState<PlayingState>();
    }
}
