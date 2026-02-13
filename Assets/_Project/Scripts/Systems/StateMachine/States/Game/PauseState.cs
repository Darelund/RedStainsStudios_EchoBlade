using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PauseState : State
{
    [SerializeField] private GameObject pauseScreenUI;
    [SerializeField] private GameObject abilityBarUI; //added refs to hide elements during pause - filip
    [SerializeField] private GameObject questScreenUI;
    public override void EnterState()
    {
        SavingManager.Instance.SaveData();
        AIManager.Instance.StopAllAI();
      //  Time.timeScale = 0; //Navmesh agents would keep going without this, I could make it work by stopping all navmesh agents, but to lazy - Vidar
        pauseScreenUI.SetActive(true);
        abilityBarUI.SetActive(false);
        questScreenUI.SetActive(false);
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
        AIManager.Instance.ResumeAllAI();
       // Time.timeScale = 1;
        pauseScreenUI.SetActive(false);
        abilityBarUI.SetActive(true);
        questScreenUI.SetActive(true);
        GameObject.FindAnyObjectByType<Volume>().profile.TryGet(out DepthOfField d);
        d.active = false;
    }

    public void ClosePauseScreen()
    {
        GameManager.Instance.SwitchState<PlayingState>();
    }
}
