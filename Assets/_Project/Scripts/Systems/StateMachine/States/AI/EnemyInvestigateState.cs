
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.XR;

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
    private List<GameObject> enemies = new List<GameObject>();


    private float stopThreshold = 0.4f;
    public bool isGoingTowardsInvestigatingPoint;
    public bool isAtInvestigationPoint = false;

    private float investigationTime = 5f;
    [SerializeField] private float currentInvestigationTime;
    private Vector3 interestingpoint;

    private int TalkingDistance = 2;

    private int SearchAngle = 0;
    private float NextSearch = 0;
    private InvestigationType investigationType;

    //[], [][], [,]  [][,]  [,][]

    public bool CanTalk;

    public EnemyInvestigateState(NonMonoBehaviourStateMachine nonMonoStateMachine, DetectionHelper detectionHelper) : base(nonMonoStateMachine)
    {
        this.detectionHelper = detectionHelper;
        agent = nonMonoStateMachine.GetComponent<NavMeshAgent>();

       
    }
    public override void EnterState()
    {
        //Investigate(); //How to give it an interesting point
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

        switch (detectionState)
        {
            case DetectionState.DetectNone:

                if (nonMonoStateMachine.GetComponent<Conversationable>().IsConversing) return;

                if (Vector3.Distance(agent.transform.position, interestingpoint) < stopThreshold) //We want this threshold to be quit small, so the enemy "remembers" in what direction the player last went to. This will make it look in the last direction it saw the player and if the player isn't there then it will start looking around in confusion
                {

                    if(isAtInvestigationPoint is false && CanTalk)
                    {
                        isAtInvestigationPoint = true;
                        CanTalk = false;
                        SparkConversation();
                    }
                    NextSearch += Time.deltaTime;
                    Debug.Log($"NextSearch countdown: {NextSearch}");
                    if (NextSearch > 0.2f)
                    {
                        CircleSearch(interestingpoint);
                        NextSearch = 0;
                        return;
                    }
                }
                InvestigateArea();
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
        Debug.Log("SPARK");
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
    private void CircleSearch(Vector3 pos)
    {
        if (nonMonoStateMachine.GetComponent<EnemyController>().PointOfInterest.Direction.magnitude <= Mathf.Epsilon) return;
        //Vector3 newpos = new Vector3(Mathf.Sin(SearchAngle), 0, Mathf.Cos(SearchAngle)) * 2 + pos;
        //SearchAngle = (int)Random.Range(0, 360);
        Debug.Log("New look angle");
        agent.SetDestination(pos + nonMonoStateMachine.GetComponent<EnemyController>().PointOfInterest.Direction);     
    }
    //TODO: Make him look around on the investigation spot instead if just standing there
 
    private void InvestigateArea()
    {
        currentInvestigationTime += Time.deltaTime;
        Debug.Log($"currentInvestigationTime: {currentInvestigationTime}");
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
