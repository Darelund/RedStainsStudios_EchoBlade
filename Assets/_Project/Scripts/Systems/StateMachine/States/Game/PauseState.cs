using UnityEngine;
using UnityEngine.InputSystem;

public class PauseState : State
{
    [SerializeField] private GameObject pauseScreenUI;
    public override void EnterState()
    {
        AIManager.Instance.StopAllAI();
      //  Time.timeScale = 0; //Navmesh agents would keep going without this, I could make it work by stopping all navmesh agents, but to lazy - Vidar
        pauseScreenUI.SetActive(true);
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
    }

    public void ClosePauseScreen()
    {
        GameManager.Instance.SwitchState<PlayingState>();
    }
}
