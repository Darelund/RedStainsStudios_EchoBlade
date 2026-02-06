using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrolState : NonMonoState
{
    private NavMeshAgent agent;
    private DetectionHelper detectionHelper;
    private List<Transform> waypoints = new List<Transform>();
    private Transform transform;

    private int PatrolIndex = -1;
    private Vector3 targetPosition = Vector3.zero;
    private Quaternion targetRotation;
    public bool HasReachedTargetPosition = true;
    public bool HasTargetPosition = true;


   
    int chanceOfTakingABreak = 5; //5% chance
    int randomPercentage;
    public EnemyPatrolState(NonMonoBehaviourStateMachine nonMonoStateMachine, List<Transform> waypoints, DetectionHelper detectionHelper) : base(nonMonoStateMachine)
    {
        this.waypoints = waypoints;
        this.detectionHelper = detectionHelper;

        agent = nonMonoStateMachine.GetComponent<NavMeshAgent>();
        transform = nonMonoStateMachine.transform;



     
    }

    public override void EnterState()
    {
      
    }
    public override void ExitState()
    {

    }
    public override void UpdateState()
    {

     

        DetectionState detectionState = detectionHelper.Detect();
        switch (detectionState)
        {
            case DetectionState.DetectNone:
                PatrolMoving();
                break;
            case DetectionState.Chase:
                //Debug.Log("Switching to chasing");
                nonMonoStateMachine.SwitchState<EnemyChaseState>();
                break;
            case DetectionState.Detect:
                //Be still, look at the thing you detected
                if (agent.hasPath)
                    agent.ResetPath();
                break;
            case DetectionState.Investigate:
                nonMonoStateMachine.GetComponent<EnemyController>().InvestigationType = InvestigationType.InvestigateSaw;
                nonMonoStateMachine.SwitchState<EnemyInvestigateState>();
                break;
        }

    }
    public override void FixedUpdateState()
    {

    }

    private void PatrolMoving()
    {
        if (HasReachedTargetPosition)
        {
            HasReachedTargetPosition = false;
            HasTargetPosition = true;
            NextWaypoint();
            ChangeTargetPosition(waypoints[PatrolIndex]);
        }
        else
        {
            if (agent.hasPath == false && HasTargetPosition)
            {
                agent.SetDestination(waypoints[PatrolIndex].position);
            }

            if (Vector3.Distance(transform.position, waypoints[PatrolIndex].position) <= 0.8f)
            {
                HasReachedTargetPosition = true;
                HasTargetPosition = false;
                TakeBreak();
            }

        }
    }
    private void TakeBreak()
    {
        randomPercentage = Random.Range(1, 101);

        if (randomPercentage > chanceOfTakingABreak) return;  //No break =( 
        nonMonoStateMachine.SwitchState<EnemyBreakState>();
    }
    private void NextWaypoint()
    {

        PatrolIndex++;
        if (PatrolIndex >= waypoints.Count)
        {
            PatrolIndex = 0;
        }

    }
    public void ChangeTargetPosition(Transform tar)
    {

        HasTargetPosition = true;
        agent.SetDestination(tar.position);
    }
}
