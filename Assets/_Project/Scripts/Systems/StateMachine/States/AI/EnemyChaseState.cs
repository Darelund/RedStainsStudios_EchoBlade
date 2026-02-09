using UnityEngine;
using UnityEngine.AI;

public class EnemyChaseState : NonMonoState
{
    private NavMeshAgent agent;
    private Transform transform;
    private DetectionHelper detectionHelper;
    private GameObject target;
    private float chaseSpeed;
    private float attackRange = 2f;
    //private bool isChasing = false;

    public EnemyChaseState(NonMonoBehaviourStateMachine nonMonoStateMachine, DetectionHelper detectionHelper) : base(nonMonoStateMachine)
    {
        transform = nonMonoStateMachine.transform;
        this.agent = nonMonoStateMachine.GetComponent<NavMeshAgent>();
        this.detectionHelper = detectionHelper;
    }
    public override void EnterState()
    {
        target = nonMonoStateMachine.GetComponent<EnemyController>().detectionHelper.GetTarget();
    }
    public override void ExitState()
    {

    }
    public override void UpdateState()
    {
        ChaseTarget();
        extraRotation();
    }
    public override void FixedUpdateState()
    {
        //if (detectionHelper.PlayerAround() is false)
        //{
        //    nonMonoStateMachine.GetComponent<EnemyController>().PointOfInterest = target.transform.position;
        //    nonMonoStateMachine.SwitchState<EnemyInvestigateState>();
        //}
    }
    private Vector3 lastPos;
    private void ChaseTarget()
    {
        if (target != null)
        {
            //ConversationManager.Instance.TryAConversation(transform, ConversationTopic.Chase);
            agent.SetDestination(target.transform.position);
            agent.updateRotation = true;

            if (Vector3.Distance(transform.position, target.transform.position) <= attackRange)
            {
                nonMonoStateMachine.SwitchState<EnemyAttackState>();
            }

            //Stop chasing either when the player is to far away , maybe raise an alarm when that happens? For other guards to pay attention. Or just go back to your spot
            //The other way is to investigate the last spot the player was scene in
            if (detectionHelper.PlayerAround() is false)
            {
                nonMonoStateMachine.GetComponent<EnemyController>().PointOfInterest.Position = target.transform.position;
                nonMonoStateMachine.GetComponent<EnemyController>().InvestigationType = InvestigationType.InvestigateLostTrackOf;
                nonMonoStateMachine.SwitchState<EnemyInvestigateState>();
            }
            nonMonoStateMachine.GetComponent<EnemyController>().PointOfInterest.Direction = (target.transform.position - lastPos).normalized;
            lastPos = target.transform.position;
        }
        else
        {
            Debug.Log("Target is null");
        }
    }
    float extraRotationSpeed = 50f;

    void extraRotation()
    {
        if (agent.hasPath is false) return;

        Vector3 lookrotation = agent.steeringTarget - transform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(lookrotation.x, 0, lookrotation.z)), extraRotationSpeed * Time.deltaTime);

    }
}
