using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyController : NonMonoBehaviourStateMachine, ISavable
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

    private bool isDead = false;
    public string enemyID;

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
        enemyID = gameObject.name + "_" + SceneManager.GetActiveScene().name;

        Debug.Log($"isdead: {isDead}");
        if (isDead is true)
        {
            SwitchState<EnemyDeathState>();
            return;
        }

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

    public void Save(GameData gameData)
    {
        foreach (var enemy in gameData.EnemiesData)
        {
            if(enemy.ID == enemyID)
            {
                enemy.IsDead = IsDead();
                enemy.Position = transform.position;
                enemy.Rotation = transform.rotation;
                return;
            }
        }
        gameData.EnemiesData.Add(new EnemyData() { ID = enemyID, IsDead = IsDead(), Position = transform.position, Rotation = transform.rotation });
    }

    public void Load(GameData gameData)
    {
        if (gameData.EnemiesData.Count == 0) return;
        if(gameData.EnemiesData == null) return;

        foreach (var enemy in gameData.EnemiesData)
        {
            if (enemy.ID == gameObject.name + "_" + SceneManager.GetActiveScene().name)
            {
                isDead = enemy.IsDead;
                if(isDead is true)
                {
                    Debug.Log($"{gameObject.name} is dead");
                }
                transform.position = enemy.Position;
                transform.rotation = enemy.Rotation;
                return;
            }
        }
    }
}
