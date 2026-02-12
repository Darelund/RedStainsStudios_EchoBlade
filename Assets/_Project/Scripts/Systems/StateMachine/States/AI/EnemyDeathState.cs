using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemyDeathState : NonMonoState
{
    private float deathTime = 1f;
    private float currentDeathTime;
    GameObject Weapon;
    ParticleSystem blood1;
    public EnemyDeathState(NonMonoBehaviourStateMachine nonMonoStateMachine, GameObject weapon, ParticleSystem blood) : base(nonMonoStateMachine)
    {
        Weapon = weapon;
       blood1 = blood;
    }

    public override void EnterState()
    {
        //TODO: Switch to death animation
        //TODO: Play Death Sound
        Debug.Log("Enemy Died");
        blood1.Play();
        nonMonoStateMachine.GetComponentInChildren<Animator>().Play("Dying");
       
        nonMonoStateMachine.GetComponent<NavMeshAgent>().enabled = false;
        nonMonoStateMachine.GetComponent<EnemyController>().enabled = false;
        nonMonoStateMachine.GetComponent<EnemyController>().hearingTarget.StopHearing();
        //nonMonoStateMachine.transform.GetChild(3).rotation = Quaternion.Euler(90, 0, 0);
        Weapon.GetComponent<BoxCollider>().enabled = false;
        //gets the boxcollider of the weapon and disables it
        //nonMonoStateMachine.StartCoroutine(DyingCoroutine());
        nonMonoStateMachine.GetComponentsInChildren<Light>().ToList().ForEach(x => x.gameObject.SetActive(false));
    }
  
    public override void UpdateState()
    {
       

    }

    private IEnumerator DyingCoroutine()
    {
        var startRotation = nonMonoStateMachine.transform.GetChild(3).localRotation;
        var endRotation = Quaternion.Euler(90, 0, 0);
        while(currentDeathTime < deathTime)
        {
            currentDeathTime += Time.deltaTime;
            nonMonoStateMachine.transform.GetChild(3).localRotation = Quaternion.Lerp(startRotation, endRotation, currentDeathTime);
            yield return null;
        }
        /*
         * agent.transform.rotation = Quaternion.Lerp(agent.transform.rotation, RotationList[RotationIndex], Time.deltaTime);
           this code is in stationary,
           agent.transform.rotation = Quaternion.Lerp(agent.transform.rotation, Quaternion.LookRotation(soundlocation), Time.deltaTime);
         */
    }
}
