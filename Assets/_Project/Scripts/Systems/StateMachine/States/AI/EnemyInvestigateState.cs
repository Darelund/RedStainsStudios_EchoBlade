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
    private DetectionHelper detectionHelper;
    private NavMeshAgent agent;


    private float stopThreshold = 0.4f;
    public bool isGoingTowardsInvestigatingPoint;
    public bool isAtInvestigationPoint = false;

    private float investigationTime = 5f;
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
        //Investigate(); //How to give it an interesting point
        interestingpoint = nonMonoStateMachine.GetComponent<EnemyController>().PointOfInterest.Position /*+ new Vector3(0, -0.3f, 0)*/;
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
    public override void FixedUpdateState()
    {

    }


    //TODO: One problem, enemies will run to the same spot and the first enemy will stop on that spot. Which will block all other enemies to reach that spot. Fix it later
    public void Investigate()
    {
        //ugly but hopefully functional code
        if (!isGoingTowardsInvestigatingPoint)
        {
            agent.SetDestination(interestingpoint);
            isGoingTowardsInvestigatingPoint = true;
            //already sets the intersting point with offset in other scripts
        }
       
        var detectionState = detectionHelper.Detect(1, 0.5f);
        Debug.Log(detectionState);

        switch (detectionState)
        {
            case DetectionState.DetectNone:

                if (nonMonoStateMachine.GetComponent<Conversationable>().IsConversing)
                {
                    Debug.Log($"Conversering makes it stuck");
                    return;
                }

                Debug.Log($"Distance is to far away from {interestingpoint} it is {Vector3.Distance(agent.transform.position, interestingpoint)}m away");
                if (Vector3.Distance(agent.transform.position, interestingpoint) < stopThreshold) //We want this threshold to be quit small, so the enemy "remembers" in what direction the player last went to. This will make it look in the last direction it saw the player and if the player isn't there then it will start looking around in confusion
                {
                    Debug.Log("Close enough");
                    if (isAtInvestigationPoint is false && CanTalk)
                    {
                        isAtInvestigationPoint = true;
                        CanTalk = false;
                        Debug.Log("Stuck 1");
                        SparkConversation();
                    }
                    if(investigationState == 0)
                    {
                        LookAtLastPlayerPoint(interestingpoint);
                        Debug.Log("Stuck 2");
                        investigationState = 1;
                    }
                    else if (investigationState == 1)
                    {
                        Debug.Log("Stuck 3");
                        NextSearch += Time.deltaTime;
                        Debug.Log($"Next Search in: {NextSearch} / 0.2 ");
                        if (NextSearch > 0.2f)
                        {
                            Debug.Log("Do a circle");
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
            case DetectionState.Detect:
                Debug.Log("Detect???");
                break;
        }
    }
   private void SparkConversation()
    {
        //Debug.Log("SPARK");
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
        //Debug.Log("New look angle");
        NavMeshPath targetPath = new NavMeshPath();
        if (agent.CalculatePath(newpos, targetPath) is false) return; //Why not out parameter?
        //if(Vector3.Distance(targetPath.corners[targetPath.corners.Length - 1], soundLocation) > 0.05f) return;
        if (targetPath.status == NavMeshPathStatus.PathPartial || targetPath.status == NavMeshPathStatus.PathInvalid) return;
        agent.SetDestination(newpos);     
    }
    //TODO: Make him look around on the investigation spot instead if just standing there
 
    private void InvestigateArea()
    {
        currentInvestigationTime += Time.deltaTime;
        Debug.Log($"Investigate time: {currentInvestigationTime}");
        //Debug.Log($"currentInvestigationTime: {currentInvestigationTime}");
        //  Debug.Log("currentinvestigation time" + currentInvestigationTime + " " + " investigation time" + investigationTime);
        if (currentInvestigationTime >= investigationTime)
        {
            //if the ai has been investigating for 5 seconds and have not found anything, return to patrol
            //currentEnemyState = EnemyState.Patrol;
            if (nonMonoStateMachine.transform.GetComponent<EnemyController>().ShouldPatrol)
                nonMonoStateMachine.SwitchState<EnemyPatrolState>();
            else
                nonMonoStateMachine.SwitchState<EnemyStationaryState>();
            //nonMonoStateMachine.SwitchState<EnemyPatrolState>();
            currentInvestigationTime = 0f;
            isGoingTowardsInvestigatingPoint = false;
            isLooking = false;
            if (agent.isActiveAndEnabled == false) agent.enabled = true;
        }
    }
    bool isLooking = false;

}
