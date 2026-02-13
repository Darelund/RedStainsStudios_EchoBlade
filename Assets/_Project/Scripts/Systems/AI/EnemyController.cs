using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.EventSystems.EventTrigger;

public class EnemyController : NonMonoBehaviourStateMachine, ISavable
{
    [SerializeField] private GameObject eyes; //(Eye), singular, maybe more eyes in the future?
    [SerializeField] private LightChanger lightChanger;
    [SerializeField] List<Transform> waypoints = new List<Transform>();
    [SerializeField] GameObject weapon;
    [SerializeField] ParticleSystem blood;
    [NonSerialized] public GameObject Player;
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
    private Vector3 startPos; //Only used for getting the startposition of stationary enemies

   public bool IsDead()
    {
        return GetCurrentState().GetType() == typeof(EnemyDeathState);
    }

    private void Start()
    {
        Player = FindAnyObjectByType<Movement>().gameObject;
        detectionHelper = new DetectionHelper(transform, eyes.transform, lightChanger);



        states.Add(new EnemyPatrolState(this, waypoints, detectionHelper));
        states.Add(new EnemyChaseState(this, detectionHelper));
        states.Add(new EnemyInvestigateState(this, detectionHelper));
        states.Add(new EnemyAlertedState(this));
        states.Add(new EnemyAttackState(this, detectionHelper));
        states.Add(new EnemyStationaryState(this, detectionHelper));
        states.Add(new EnemyDeathState(this, weapon, blood));
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
        if (ShouldPatrol is false && startPos != Vector3.zero)
        {
            var stat = states.Find(s => s.GetType() == typeof(EnemyStationaryState)) as EnemyStationaryState;
            stat.startPosition = startPos;
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
        var startPosition = Vector3.zero;
        if (ShouldPatrol is false)
        {
            var stat = states.Find(s => s.GetType() == typeof(EnemyStationaryState)) as EnemyStationaryState;
            startPosition = stat.startPosition;
        }
        gameData.EnemiesData.Add(new EnemyData() { ID = enemyID, IsDead = IsDead(), Position = transform.position, Rotation = transform.rotation, StartPosition = startPosition });
    }

    public void Load(GameData gameData)
    {
        if(gameData.EnemiesData == null) return;
        if (gameData.EnemiesData.Count == 0) return;

        foreach (var enemy in gameData.EnemiesData)
        {
            if (enemy.ID == gameObject.name + "_" + SceneManager.GetActiveScene().name)
            {
                isDead = enemy.IsDead;
                if(isDead is true)
                {
                    Debug.Log($"{gameObject.name} is dead");
                }
                if (ShouldPatrol is false)
                {
                    //var stat = states.Find(s => s.GetType() == typeof(EnemyStationaryState)) as EnemyStationaryState;
                   startPos = enemy.StartPosition;
                }
                transform.position = enemy.Position;
                transform.rotation = enemy.Rotation;
                return;
            }
        }
    }
}
