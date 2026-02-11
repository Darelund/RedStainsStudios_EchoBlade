using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class EnemyController : NonMonoBehaviourStateMachine
{
    [SerializeField] private GameObject eyes; //(Eye), singular, maybe more eyes in the future?
    [SerializeField] private LightChanger lightChanger;
    [SerializeField] List<Transform> waypoints = new List<Transform>();
    [SerializeField] GameObject weapon;

    public InterestPoint PointOfInterest = new InterestPoint();
    public bool ShouldPatrol = true;
    public float hearingRange;
    public bool ShowGizmos = false;
    public bool ShowHearingDebugs = false;

    public DetectionHelper detectionHelper;
    public HearingTarget hearingTarget;
    public InvestigationType InvestigationType;

   public bool IsDead()
    {
        return GetCurrentState().GetType() == typeof(EnemyDeathState);
    }

    private void Start()
    {
        detectionHelper = new DetectionHelper(transform, eyes.transform, lightChanger);



        states.Add(new EnemyPatrolState(this, waypoints, detectionHelper));
        states.Add(new EnemyChaseState(this, detectionHelper));
        states.Add(new EnemyInvestigateState(this, detectionHelper));
        states.Add(new EnemyAlertedState(this));
        states.Add(new EnemyAttackState(this, detectionHelper));
        states.Add(new EnemyStationaryState(this, detectionHelper));
        states.Add(new EnemyDeathState(this, weapon));
        states.Add(new EnemyTalkState(this));
        states.Add(new EnemyBreakState(this, detectionHelper));

        hearingTarget = new HearingTarget(transform, hearingRange, this, ShowHearingDebugs);

        if (ShouldPatrol)
            SwitchState<EnemyPatrolState>();
        else
            SwitchState<EnemyStationaryState>();
    }

    public void UpdateController()
    {
        UpdateStateMachine();
    }

    public void FixedUpdateController()
    {

    }

    //private void OnDrawGizmosSelected()
    //{
       
    //}
    private void OnDrawGizmos()
    {
        if (ShowGizmos is false) return;


        Gizmos.color = Color.yellow;

        //var circleMesh = new Mesh();
        //circleMesh.
        Gizmos.DrawWireSphere(transform.position, hearingRange);

      //  transform.draw

    }
}
