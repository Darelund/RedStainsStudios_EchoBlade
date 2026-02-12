using System.Collections;
using UnityEngine;

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
        StartCoroutine(ResumeState());
    }

    private IEnumerator ResumeState()
    {
        abilityBarUI.SetActive(true);
        questScreenUI.SetActive(true);
        yield return new WaitForSeconds(.6f);
        AIManager.Instance.ResumeAllAI();
    }
}