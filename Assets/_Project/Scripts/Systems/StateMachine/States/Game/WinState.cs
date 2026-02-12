using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class WinState : State
{
    [SerializeField] private GameObject winScreenUI;
   
    public override void EnterState()
    {
        AIManager.Instance.StopAllAI();
        Time.timeScale = 0; //We might want to use timescale here, so enemies stop instantly and doesn't touch you
        winScreenUI.SetActive(true);
        GameObject.FindAnyObjectByType<Volume>().profile.TryGet(out DepthOfField d);
        d.active = true;
    }
    public override void UpdateState()
    {

    }
    public override void ExitState()
    {
        AIManager.Instance.ResumeAllAI();
        Time.timeScale = 1;
        winScreenUI.SetActive(false);
      
        GameObject.FindAnyObjectByType<Volume>().profile.TryGet(out DepthOfField d);
        d.active = false;
    }

    //public void CloseWinScreen()
    //{
    //    GameManager.Instance.SwitchState<PlayingState>();
    //}
}
