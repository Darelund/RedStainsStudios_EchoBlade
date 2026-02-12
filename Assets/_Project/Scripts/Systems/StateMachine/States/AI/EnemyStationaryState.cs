
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStationaryState : NonMonoState
{
    private NavMeshAgent agent;
    List<Quaternion> RotationList;
    private int RotationIndex = 0;
    private float timer = 0;
    private DetectionHelper detectionHelper;
    public EnemyStationaryState(NonMonoBehaviourStateMachine nonMonoStateMachine, DetectionHelper detectionHelper) : base(nonMonoStateMachine)
    {
        this.RotationList = new List<Quaternion>()
        {
            Quaternion.Euler(0, Random.Range(0, 361), 0),
            Quaternion.Euler(0, Random.Range(0, 361), 0),
            Quaternion.Euler(0, Random.Range(0, 361), 0),
            Quaternion.Euler(0, Random.Range(0, 361), 0),
            Quaternion.Euler(0, Random.Range(0, 361), 0),

        };
        agent = nonMonoStateMachine.GetComponent<NavMeshAgent>();
        this.detectionHelper = detectionHelper;
    }
    public override void EnterState()
    {
        RotationList[RotationIndex] = agent.transform.rotation;
        //sets the rotationlist at the index to the current transform.rotation
    }
    public override void UpdateState()
    {
        while (timer <= 10 && agent.transform.rotation != RotationList[RotationIndex])
        {
            timer += Time.deltaTime;
            agent.transform.rotation = Quaternion.Lerp(agent.transform.rotation, RotationList[RotationIndex], Time.deltaTime);
        }

        //var detectionState = detectionHelper.Detect();

        //switch (detectionState)
        //{
        //    case DetectionState.DetectNone:
        //        //Well do nothing
        //        break;
        //    case DetectionState.Chase:
        //        nonMonoStateMachine.GetComponent<Conversationable>().OverrideTalkDelay();
        //        nonMonoStateMachine.SwitchState<EnemyChaseState>();
        //        break;
        //    case DetectionState.Investigate:
        //        nonMonoStateMachine.SwitchState<EnemyInvestigateState>();
        //        break;
        //}

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
    public override void ExitState()
    {
        RotationIndex++;
        if (RotationIndex >= RotationList.Count)
        {
            RotationIndex = 0;
        }
    }
    

}
