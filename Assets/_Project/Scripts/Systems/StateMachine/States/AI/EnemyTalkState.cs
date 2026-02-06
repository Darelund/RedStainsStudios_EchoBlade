
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyTalkState : NonMonoState
{
  [SerializeField] public string[] conversation = new string[5];
  [SerializeField] public string[] responses = new string[5];
    int conversatioIndex = 0;
    int responseIndex = 0;

    private NavMeshAgent agent;

    EnemyTalkState otherenemy = null;
   public bool talking = false;
    float timerBetweentalks = 0;
    bool stopTalking = false;
    public EnemyTalkState(NonMonoBehaviourStateMachine nonMonoStateMachine) : base(nonMonoStateMachine)
    {
        agent = nonMonoStateMachine.GetComponent<NavMeshAgent>();
    }
    public override void EnterState()
    {
       otherenemy = GetClosestEnemy().GetComponent<EnemyTalkState>();
    }
    public override void ExitState()
    {

    }
    public override void UpdateState()
    {
        timerBetweentalks += Time.deltaTime;
        if (otherenemy != null && otherenemy.talking == false)
        {
            //if otherenemy is not null and the enemytalking is false, this agent/enemy will talk
            talking = true;
            TalkWithOther();
        }else if (otherenemy.talking == true)
        {

            //if enemy is talking, this enemy will not talk
            talking = false;
        }
    }
    public override void FixedUpdateState()
    {

    }

    private GameObject GetClosestEnemy()
    {
        List<GameObject> list = new List<GameObject>();
        list.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        GameObject nearestenemy = null;
        float distance = 1000;

        foreach(GameObject go in list)
        {
            if (go.GetComponent<EnemyTalkState>() == this)
            {
                continue;
            }else if (go != null)
            {
                float dis = Vector3.Distance(agent.transform.position, go.transform.position);
                if (dis < distance)
                {
                    nearestenemy = go;
                    distance = dis;
                }
            }
        }
       
        return nearestenemy;

    }

    private void TalkWithOther()
    {
        if (otherenemy != null)
        {
           if (!talking) return;
           //if talking is false, return otherwise continue

            if (timerBetweentalks > 1)
            {
                Talk();
                //runs the talk function
            }
            if (timerBetweentalks > 3)
            {
                otherenemy.Respond();
                //gets the enemy respons
                timerBetweentalks = 0;
                //repeats
            }

            if (stopTalking)
            {
                //when it stops talking, resets conversation
                //todo, make enemy walk away
                conversatioIndex = 0;
                responseIndex = 0;
                otherenemy.responseIndex = 0;
            }
        }
    }

    private void Talk()
    {
        //do something with ui, like adding textbox above enemy
        //conversation[conversationIndex]
        if (conversatioIndex > conversation.Length)
        {
            conversatioIndex = 0;
        }
        else
        {
            Debug.Log(conversation[conversatioIndex]);
            conversatioIndex++;
        }
    }
    public void Respond()
    {
        //add textbox above enemy with respons
        //responses[responseIndex]
        Debug.Log(responses[responseIndex]);
        if (responseIndex > responses.Length)
        {
            responseIndex = 0;
        }
        else
        {
            responseIndex++;
        }
    }
}
