using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIManager : MonoBehaviour
{
    #region Singleton
    private static AIManager instance;
    public static AIManager Instance => instance;


    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
            return;
        }

        instance = this;
    }
    #endregion

    List<EnemyController> Ais = new List<EnemyController>();
    
    private void Start()
    {
        initialise();
    }
    private void initialise()
    {
        Ais.AddRange(FindObjectsByType<EnemyController>(FindObjectsSortMode.None));
        //finds all the enemycontrollers components in the scene and add them to the list
    }
    public void UpdateManager()
    {
        //TODO: Remove from list when dead
        List<EnemyController> deadList = new List<EnemyController>();
        foreach (EnemyController AI in Ais)
        {
            if (AI.gameObject.activeSelf != false)
            {
                AI.UpdateController();
            }
            if(AI.GetCurrentState().GetType() == typeof(EnemyDeathState))
            {
                deadList.Add(AI);
            }
                    
        }
        foreach (EnemyController AI in deadList)
        {
            Ais.Remove(AI);
        }
    }
    public void RemoveAI(EnemyController AiRemove)
    {
        for (int i = 0; i < Ais.Count; i++)
        {
            if (Ais[i] == AiRemove)
            {
                Ais.RemoveAt(i);
                Debug.Log("ai was removed");
                return;
            }
        }

    }
    public void StopAllAI()
    {
        foreach (var AI in Ais)
        {
            if (AI.GetCurrentState().GetType() == typeof(EnemyDeathState)) return;

            AI.GetComponent<NavMeshAgent>().Stop();
            //AI.GetComponent<NavMeshAgent>().isStopped = true;
        }
    }
    public void ResumeAllAI()
    {
        foreach (var AI in Ais)
        {
            if (AI.GetCurrentState().GetType() == typeof(EnemyDeathState)) return;

            AI.GetComponent<NavMeshAgent>().Resume();
            //AI.GetComponent<NavMeshAgent>().isStopped = false;
        }
    }
}
