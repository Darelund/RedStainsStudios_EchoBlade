using UnityEngine;
using UnityEngine.AI;

public enum InvestigationType
{
    InvestigateNone,
    InvestigateSound,
    InvestigateSaw,
    InvestigateAlarm,
    InvestigateLostTrackOf
}
public class EnemyInvestigateState : NonMonoState
{
    private readonly DetectionHelper detectionHelper;
    private readonly NavMeshAgent agent;


    private readonly float stopThreshold = 2.5f;
    public bool isGoingTowardsInvestigatingPoint;
    public bool isAtInvestigationPoint = false;

    private readonly float investigationTime = 5f;
    [SerializeField] private float currentInvestigationTime;
    private Vector3 interestingpoint;


    private int SearchAngle = 0;
    private float NextSearch = 0;
    private InvestigationType investigationType;
    public int investigationState = 0;

    public bool CanTalk;

    public EnemyInvestigateState(NonMonoBehaviourStateMachine nonMonoStateMachine, DetectionHelper detectionHelper) : base(nonMonoStateMachine)
    {
        this.detectionHelper = detectionHelper;
        agent = nonMonoStateMachine.GetComponent<NavMeshAgent>();

       
    }
    public override void EnterState()
    {
        interestingpoint = nonMonoStateMachine.GetComponent<EnemyController>().PointOfInterest.Position;
        SearchAngle = (int)Random.Range(0, 360);
        investigationType = nonMonoStateMachine.GetComponent<EnemyController>().InvestigationType;
        CanTalk = true;
    }
    public override void ExitState()
    {
        isGoingTowardsInvestigatingPoint = false;
        investigationType = InvestigationType.InvestigateNone;
        isAtInvestigationPoint = false;
        CanTalk = true;
    }
    public override void UpdateState()
    {
        Investigate();
    }
    public void Investigate()
    {
        if (!isGoingTowardsInvestigatingPoint)
        {
            agent.SetDestination(interestingpoint);
            isGoingTowardsInvestigatingPoint = true;
        }
       
        var detectionState = detectionHelper.Detect(1, 0.5f);

        switch (detectionState)
        {
            case DetectionState.DetectNone:

                if (nonMonoStateMachine.GetComponent<Conversationable>().IsConversing)
                {
                    return;
                }
                if (Vector3.Distance(agent.transform.position, interestingpoint) < stopThreshold) //We want this threshold to be quit small, so the enemy "remembers" in what direction the player last went to. This will make it look in the last direction it saw the player and if the player isn't there then it will start looking around in confusion
                {
                    if (isAtInvestigationPoint is false && CanTalk)
                    {
                        isAtInvestigationPoint = true;
                        CanTalk = false;
                        SparkConversation();
                    }
                    if(investigationState == 0)
                    {
                        LookAtLastPlayerPoint(interestingpoint);
                        investigationState = 1;
                    }
                    else if (investigationState == 1)
                    {
                        NextSearch += Time.deltaTime;
                        if (NextSearch > 0.2f)
                        {
                            CircleSearch(interestingpoint);
                            NextSearch = 0;
                            return;
                        }
                    }
                    InvestigateArea();
                }
                break;
            case DetectionState.Chase:
                nonMonoStateMachine.GetComponent<Conversationable>().OverrideTalkDelay();
                nonMonoStateMachine.SwitchState<EnemyChaseState>();
                isGoingTowardsInvestigatingPoint = false;
                break;
            case DetectionState.Investigate:
                nonMonoStateMachine.SwitchState<EnemyInvestigateState>();
                isGoingTowardsInvestigatingPoint = false;
                break;
        }
    }
   private void SparkConversation()
    {
        switch (nonMonoStateMachine.GetComponent<EnemyController>().InvestigationType)
        {
            case InvestigationType.InvestigateNone:
                break;
            case InvestigationType.InvestigateSound:
                ConversationManager.Instance.TryAConversation(nonMonoStateMachine.transform, ConversationTopic.Heard);
                break;
            case InvestigationType.InvestigateSaw:
                ConversationManager.Instance.TryAConversation(nonMonoStateMachine.transform, ConversationTopic.Saw);
                break;
            case InvestigationType.InvestigateAlarm:
                ConversationManager.Instance.TryAConversation(nonMonoStateMachine.transform, ConversationTopic.Alarm);
                break;
            case InvestigationType.InvestigateLostTrackOf:
                ConversationManager.Instance.TryAConversation(nonMonoStateMachine.transform, ConversationTopic.Disappeared);
                break;
        }
    }

  
    //Should be like investigation stages
    //First one is stage 1, which is look at last known position of the player
    private void LookAtLastPlayerPoint(Vector3 pos)
    {
        if (nonMonoStateMachine.GetComponent<EnemyController>().PointOfInterest.Direction.magnitude <= Mathf.Epsilon) return;
        
        agent.SetDestination(pos + nonMonoStateMachine.GetComponent<EnemyController>().PointOfInterest.Direction);
    }
    //Second stage is to look around for the player
    private void CircleSearch(Vector3 pos)
    {
        //if (nonMonoStateMachine.GetComponent<EnemyController>().PointOfInterest.Direction.magnitude <= Mathf.Epsilon) return;
        Vector3 newpos = new Vector3(Mathf.Sin(SearchAngle), 0, Mathf.Cos(SearchAngle)) * 2 + pos;
        SearchAngle = (int)Random.Range(0, 360);
        NavMeshPath targetPath = new NavMeshPath();
        if (agent.CalculatePath(newpos, targetPath) is false) return; //Why not out parameter?
        if (targetPath.status == NavMeshPathStatus.PathPartial || targetPath.status == NavMeshPathStatus.PathInvalid) return;
        agent.SetDestination(newpos);     
    }
    private void InvestigateArea()
    {
        currentInvestigationTime += Time.deltaTime;
      
        if (currentInvestigationTime >= investigationTime)
        {
            //if the ai has been investigating for 5 seconds and have not found anything, return to patrol
            if (nonMonoStateMachine.transform.GetComponent<EnemyController>().ShouldPatrol)
                nonMonoStateMachine.SwitchState<EnemyPatrolState>();
            else
                nonMonoStateMachine.SwitchState<EnemyStationaryState>();

            currentInvestigationTime = 0f;
            isGoingTowardsInvestigatingPoint = false;
            if (agent.isActiveAndEnabled == false) agent.enabled = true;
        }
    }

}
