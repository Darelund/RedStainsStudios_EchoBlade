using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAttackState : NonMonoState
{
    private GameObject targetTransform; //How do we get the target
    private Transform transform;
    private float attackRange = 2.3f;
    private NavMeshAgent agent;

    private bool isAttacking;
    private float attackCooldown = 1f;
    //private 
    public EnemyAttackState(NonMonoBehaviourStateMachine nonMonoStateMachine) : base(nonMonoStateMachine)
    {
        transform = nonMonoStateMachine.transform;
        agent = nonMonoStateMachine.GetComponent<NavMeshAgent>();
    }
    public override void EnterState()
    {
        targetTransform = nonMonoStateMachine.GetComponent<EnemyController>().detectionHelper.GetTarget();

        agent.stoppingDistance = 3.2f;
        //agent.ResetPath();
    }
    public override void ExitState()
    {
        agent.stoppingDistance = 0f;
    }
    public override void UpdateState()
    {
        AttackTarget();
    }
    public override void FixedUpdateState()
    {

    }
    private void AttackTarget()
    {

       
        //Debug.Log("Attacking target");
        //TODO: Attack player
        agent.transform.rotation = Quaternion.Lerp(agent.transform.rotation, Quaternion.LookRotation((targetTransform.transform.position - agent.transform.position).normalized), Time.deltaTime);
     
        SwingSword();
     
        if (Vector3.Distance(transform.position, targetTransform.transform.position) > attackRange)
        {
            Debug.Log(Vector3.Distance(transform.position, targetTransform.transform.position));
            nonMonoStateMachine.GetComponent<Conversationable>().OverrideTalkDelay();
            nonMonoStateMachine.SwitchState<EnemyChaseState>();
            //currentEnemyState = EnemyState.Chase;
        }
    }
    private void SwingSword()
    {
        if (isAttacking is true) return;

        isAttacking = true;
        nonMonoStateMachine.GetComponentInChildren<Animator>().Play("Attack");
        nonMonoStateMachine.GetComponentInChildren<Animator>().SetTrigger("AnimAttack");
        nonMonoStateMachine.StartCoroutine(AttackCooldown());
    }
    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }
}
