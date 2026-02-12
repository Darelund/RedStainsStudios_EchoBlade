using System.Collections.Generic;
using UnityEngine;

public class AlarmSensor : MonoBehaviour
{
    [SerializeField] private float alarmArea = 20; //Area to alarm enemies in
    public bool isDisabled = false;
    [SerializeField] private GameObject alarmIcon;
    private Vector3 offset = new Vector3(0, 0, 0);
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Movement>() != null && !isDisabled) //Find player gameobject
        {
            GetNearbyEnemies(other.gameObject.transform);
        }
    }
    private void GetNearbyEnemies(Transform investigationPoint)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, alarmArea, 1 << 15);
        //TODO: Add a tag for enemies so I don't need to do this
        List<EnemyController> nearbyEnemies = new List<EnemyController>();
        foreach (var collider in colliders)
        {
            if (collider.GetComponent<EnemyController>() != null)
            {
                nearbyEnemies.Add(collider.GetComponent<EnemyController>());
            }
        }

        foreach (var enemy in nearbyEnemies)
        {
            Debug.Log(enemy);
            PingEnemies(enemy.GetComponent<EnemyController>(), investigationPoint);
        }
        offset = Vector3.zero;
    }

    private void PingEnemies(EnemyController controller, Transform investigationPoint)
    {

        //If an enemy is chasing or attacking a player then it shouldn't care about an alarm
        if(controller.GetCurrentState().GetType() == typeof(EnemyAttackState) ||
            controller.GetCurrentState().GetType() == typeof(EnemyChaseState))
        {
            return;
        }



        Instantiate(alarmIcon, controller.transform);
        controller.PointOfInterest.Position = investigationPoint.position - offset;
        offset += new Vector3(0.5f, 0, 0.5f);
        controller.InvestigationType = InvestigationType.InvestigateAlarm;
        controller.SwitchState<EnemyAlertedState>();
    }
}

