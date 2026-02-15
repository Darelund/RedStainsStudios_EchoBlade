using UnityEngine;
using UnityEngine.AI;

public class EnemyAlertState : NonMonoState
{
    private readonly NavMeshAgent agent;
    private Vector3 soundlocation;

    public EnemyAlertState(NonMonoBehaviourStateMachine nonMonoStateMachine) : base(nonMonoStateMachine)
    {
        agent = nonMonoStateMachine.GetComponent<NavMeshAgent>();
    }
    public override void EnterState()
    {

    }
    public override void ExitState()
    {

    }
    public override void UpdateState()
    {
        if (soundlocation != null)
        {
          agent.transform.rotation = Quaternion.Lerp(agent.transform.rotation, Quaternion.LookRotation(soundlocation), Time.deltaTime);
        }
    }

    public void ChangeLocation(Vector3 loc)
    {
        soundlocation = loc;
    }
}
