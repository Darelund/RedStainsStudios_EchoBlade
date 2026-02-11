using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CutsceneState : State
{
    [SerializeField] private GameObject abilityBarUI; //Repurposed Pause State for Cutscenes - Filip
    [SerializeField] private GameObject questScreenUI;
    public override void EnterState()
    {
        AIManager.Instance.StopAllAI();
        abilityBarUI.SetActive(false);
        questScreenUI.SetActive(false);
    }
    public override void UpdateState()
    {
        return;
    }
    public override void ExitState()
    {
        AIManager.Instance.ResumeAllAI();
        abilityBarUI.SetActive(true);
        questScreenUI.SetActive(true);
    }
}
