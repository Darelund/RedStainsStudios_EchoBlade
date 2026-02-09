using UnityEngine;
using UnityEngine.AI;

public class HearingTarget
{
    //TODO: Add hearing
    private Transform hearingGameObject;
    private float hearingRange = 30f;
    private NavMeshAgent agent;
    private NonMonoBehaviourStateMachine nonMonoBehaviourStateMachine;
    private int travelDistance = 10; //How far the AI is willing to travel to reach the soundLocation
    private bool showHearingDebugs = true;
    public HearingTarget(Transform hearingGameObject, float hearingRange, NonMonoBehaviourStateMachine agent, bool showHearingDebugs)
    {
        this.hearingGameObject = hearingGameObject;
        this.hearingRange = hearingRange;
        this.agent = agent.GetComponent<NavMeshAgent>();
        this.nonMonoBehaviourStateMachine = agent;

        this.showHearingDebugs = showHearingDebugs;

        HearingManager.Instance.OnHearing += Instance_OnHearing;
    }

    private void Instance_OnHearing(HearingManager.HeardSound obj)
    {
        OnHeardSound(obj.soundLocation, obj.soundType);
    }

    public void StopHearing()
    {
        HearingManager.Instance.OnHearing -= Instance_OnHearing;
    }

    public void OnHeardSound(Vector3 soundLocation, SoundType soundType)
    {
        if (nonMonoBehaviourStateMachine.GetCurrentState().GetType() == typeof(EnemyChaseState)) return;
        if (nonMonoBehaviourStateMachine.GetCurrentState().GetType() == typeof(EnemyAttackState)) return;

        if (Vector3.Distance(hearingGameObject.position, soundLocation) > hearingRange) return; //To far away
        if (showHearingDebugs)
            Debug.Log("Sound");



        //No way for the navmesh to reach it
        NavMeshPath targetPath = new NavMeshPath();
        if (agent.CalculatePath(soundLocation, targetPath) is false) return; //Why not out parameter?
        //if(Vector3.Distance(targetPath.corners[targetPath.corners.Length - 1], soundLocation) > 0.05f) return;
        if (targetPath.status == NavMeshPathStatus.PathPartial || targetPath.status == NavMeshPathStatus.PathInvalid) return;


            //TODO: Implement it
            //Too far away
            //  if(targetPath.corners)
            //float pathDistance = 0;
            //for (int i = 0; i < targetPath.corners.Length; i++)
            //{
            //    pathDistance += Vector3.Distance(targetPath.corners[i], targetPath.corners[i + 1]);
            //}




            var distanceFromSound = Vector3.Distance(hearingGameObject.position, soundLocation);
        float soundIntensity = ( distanceFromSound / hearingRange ) >= 0.50f ? 
                               1 - (distanceFromSound / hearingRange) : 
                               1 + (distanceFromSound / hearingRange);
        // 30 - 0
        // 0  - 1
        /*
         * 45 10
         * 40 20
         * 30 40
         * 20 60
         * 10 80
         * 0  100
         */
        if (showHearingDebugs)
            Debug.Log($"{soundIntensity}");


        ////TODO: Make sounds louder when they are closer to the enemy
        nonMonoBehaviourStateMachine.GetComponent<EnemyController>().InvestigationType = InvestigationType.InvestigateAlarm;
        switch (soundType)
        {
            case SoundType.Footstep:
                //if (soundIntensity > 0.3f && soundIntensity < 0.5f)
                //{
                //    // nonMonoBehaviourStateMachine.SwitchState<EnemyAlertedState>();
                //}
                if (showHearingDebugs)
                    Debug.Log("Footstep");
                if (soundIntensity >= 0.1f)
                {
                    nonMonoBehaviourStateMachine.GetComponent<EnemyController>().PointOfInterest.Position = soundLocation;
                    nonMonoBehaviourStateMachine.SwitchState<EnemyInvestigateState>();
                }
                break;
            case SoundType.TakeDown:
                nonMonoBehaviourStateMachine.GetComponent<EnemyController>().PointOfInterest.Position = soundLocation;
                nonMonoBehaviourStateMachine.SwitchState<EnemyInvestigateState>();
                break;
        }
    }
}
