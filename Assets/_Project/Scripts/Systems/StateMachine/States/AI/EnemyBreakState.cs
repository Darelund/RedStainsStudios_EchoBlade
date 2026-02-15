using System.Collections.Generic;
using UnityEngine;

public class EnemyBreakState : NonMonoState
{
    //private bool isAllowedBreaks = true; //Maybe add later that some enemies are not allowed to take a break
    private bool isOnBreak = false;
    private float breakTime;
    private float currentBreakTime;
    private readonly List<(string, float)> breakActivities;
    private readonly DetectionHelper detectionHelper;

    public EnemyBreakState(NonMonoBehaviourStateMachine nonMonoStateMachine, DetectionHelper detectionHelper) : base(nonMonoStateMachine)
    {
        //TODO: Do something like this: The less waypoints you have, the less breaks you have
        //if (waypoints.Count <= 2)
        //{
        //    isAllowedBreaks = false;
        //}
        this.detectionHelper = detectionHelper;
        breakActivities = new List<(string, float)> 
        { ("Looking Around", Random.Range(2, 5)), 
          ("Taking a piss", Random.Range(7, 11)), 
          ("Smoking", Random.Range(10, 16)), 
          ("Singing", Random.Range(4, 10)) 
        }; //The number is how long the break will be
    }

    public override void EnterState()
    {
        WaitPatrol();
    }
    public override void ExitState()
    {

    }
    public override void UpdateState()
    {
        if (isOnBreak is true)
        {
            BreakTickingDown();
            //return;
        }
        else
        {
            if (nonMonoStateMachine.transform.GetComponent<EnemyController>().ShouldPatrol)
                nonMonoStateMachine.SwitchState<EnemyPatrolState>();
            else
                nonMonoStateMachine.SwitchState<EnemyStationaryState>();
           
        }

        var detectionState = detectionHelper.Detect();

        switch (detectionState)
        {
            case DetectionState.DetectNone:
                //Well do nothing
                break;
            case DetectionState.Chase:
                nonMonoStateMachine.GetComponent<Conversationable>().OverrideTalkDelay();
                nonMonoStateMachine.SwitchState<EnemyChaseState>();
                break;
            case DetectionState.Investigate:
                nonMonoStateMachine.GetComponent<EnemyController>().PointOfInterest.Position = nonMonoStateMachine.GetComponent<EnemyController>().Player.transform.position;
                nonMonoStateMachine.GetComponent<EnemyController>().InvestigationType = InvestigationType.InvestigateSaw;
                nonMonoStateMachine.SwitchState<EnemyInvestigateState>();
                break;
        }
    }
    public override void FixedUpdateState()
    {

    }
    private void BreakTickingDown()
    {
        currentBreakTime += Time.deltaTime;
        if (currentBreakTime > breakTime)
        {
            isOnBreak = false;
            currentBreakTime = 0f;
        }
    }
    private void WaitPatrol()
    {
        //TODO: Switch to break animation instead of just printing a string



        (string Activity, float BreakTime) breakActivityChoosen = breakActivities[Random.Range(0, breakActivities.Count)];
        isOnBreak = true;
        breakTime = breakActivityChoosen.BreakTime;
        Debug.Log($"{nonMonoStateMachine.gameObject.name} is on a break : {breakActivityChoosen.Activity}");
    }
}
