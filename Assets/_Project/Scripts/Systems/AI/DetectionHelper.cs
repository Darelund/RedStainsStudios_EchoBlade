using UnityEngine;
using UnityEngine.AI;


public enum DetectionState
{
    DetectNone,
    Chase,
    Detect,
    Investigate
}

public class DetectionHelper
{
    private Transform detectionGameObject;
    private Transform eyes;
    private GameObject target;

    private float timeInSight;
    private const float CHASE_THRESHOLD = 0.8f;
    private const float INVESTIGATE_THRESHOLD = 0.5f;


    private float eyeSightAngle = 28f;
    private float detectionRange = 10;
    private readonly LightChanger lightChanger;
    public DetectionHelper(Transform transform, Transform eyes, LightChanger lightChanger)
    {
        detectionGameObject = transform;
        this.eyes = eyes;
        this.lightChanger = lightChanger;
        eyeSightAngle = lightChanger.angle / 2;
        detectionRange = lightChanger.range;
    }
    public GameObject GetTarget() => target;
    Vector3 lastposition;
    public DetectionState Detect()
    {
        //First find player in area
        var colliders = Physics.OverlapSphere(detectionGameObject.position, detectionRange, 1 << 0); //TODO: Right now it finds all default layers, dumb, fix later
        Vector3 forwardDir = detectionGameObject.forward;
        RaycastHit rayHit;
        foreach (var c in colliders)
        {
            //If the object isn't a player move on
            if (c.gameObject.GetComponent<Movement>() == null)
            {
                continue;
            }

            Vector3 TargetDir = (c.transform.position - detectionGameObject.position).normalized;

            //Check if the player is inside FOV
            float angle = Vector3.Angle(TargetDir, forwardDir);
            if (angle <= eyeSightAngle)
            {
                //Check if we can see the player
                //Send away 10 - 30 rays to see if player is hiding or not
                //Look range
                //Vector3 velocity = (c.transform.position - lastposition) / Time.deltaTime;
                //lastposition = c.transform.position;

                //float distance = Vector3.Distance(detectionGameObject.position, c.transform.position);
                //float predictionTime = distance / detectionGameObject.GetComponent<NavMeshAgent>().speed;

                //Vector3 futurePosition =
                //c.transform.position + velocity * predictionTime;
                Vector3 predictedDir = PredictFutureDirection(c.gameObject);


                //Physics.Raycast(eyes.transform.position, predictedDir, out rayHit, lookRange);
                //Debug.DrawRay(eyes.transform.position, predictedDir * lookRange, Color.red, 1);

                int numberOfRaycasts = 10;
                float degreesBetweenRays = 5f;

                for (int i = -numberOfRaycasts; i < numberOfRaycasts; i++)
                {
                    var angleBetweenRays = i * degreesBetweenRays;
                    Vector3 dir = Quaternion.AngleAxis(angleBetweenRays, Vector3.up) * TargetDir;


                    //Physics.Raycast(eyes.transform.position, TargetDir + dir, out rayHit, lookRange);

                    Physics.Raycast(eyes.transform.position, predictedDir + dir, out rayHit, detectionRange);
                    Debug.DrawRay(eyes.transform.position, (predictedDir + dir) * detectionRange, Color.darkOliveGreen, 1);

                    if (rayHit.collider != null && rayHit.transform == c.transform)
                    {
                        //TODO: It shouldn't only be about time insight
                        //it should also be how close you are. If you are in the face of the
                        //enemy then obviously it should instantly chase/do something
                        float closeness = detectionRange - Vector3.Distance(detectionGameObject.position, rayHit.collider.transform.position);
                        //So you use lookRange somehow



                        //Found a player in sight
                        target = rayHit.collider.gameObject;
                        timeInSight += closeness * Time.deltaTime;
                        //agent.ResetPath();// = false; //Resetting path when looking at an object, does it work? I don't really know
                        //HasReachedTargetPosition = true;
                        if (timeInSight > CHASE_THRESHOLD)
                        {
                            //timeInSight = 0;
                            //agent.ResetPath();// = false;
                            //HasReachedTargetPosition = true;
                            //currentEnemyState = EnemyState.Chase;
                            return DetectionState.Chase;
                        }
                        //isInvestigating = false;

                        //Should be called from the object using this script
                        //Not this scripts responsibility to change color
                        lightChanger.ChangeVisibilityColor(timeInSight);

                        return DetectionState.Detect;
                    }
                }
            }
            //else //Out of sight before chaseTime was reached
            //{
            //    //isInvestigating = false;

            //    target = null;
            //    //timeInSight = 0;
            //}
        }
        target = null;
        if (timeInSight > 0)
        {
            timeInSight -= Time.deltaTime;
        }
        else
        {
            timeInSight = 0;
        }
        lightChanger.ChangeVisibilityColor(timeInSight);
        if (timeInSight > INVESTIGATE_THRESHOLD)
        {
            //currentEnemyState = EnemyState.Investigate;

            return DetectionState.Investigate;
        }
        return DetectionState.DetectNone;
    }

    private Vector3 PredictFutureDirection(GameObject target)
    {
        Vector3 upOnY = new Vector3(0, 0.5f, 0);
        float distance = Vector3.Distance(detectionGameObject.position, target.transform.position + upOnY);

        Vector3 velocity = (((target.transform.position + upOnY) - lastposition) / Time.deltaTime) * 0.35f;
        lastposition = target.transform.position + upOnY;

        float predictionTime = distance / detectionGameObject.GetComponent<NavMeshAgent>().speed;

        Vector3 futurePosition =
        (target.transform.position + upOnY) + velocity * predictionTime;
        Vector3 predictedDir = (futurePosition - eyes.position).normalized;
        return predictedDir;
    }


    //TODO: Right now it finds all default layers, dumb, fix later. Make one layer for things that the enemy should interact with when they see it
    public bool PlayerAround()
    {
        var colliders = Physics.OverlapSphere(detectionGameObject.position, 10, 1 << 0);

        foreach (var c in colliders)
        {
            if (c.gameObject.GetComponent<Movement>() == null)
            {
                continue;
            }
            float rangeToKnowIfRaycastShouldBeUsed = 2;
            float distance = Vector3.Distance(c.transform.position, detectionGameObject.position);
            if (distance <= rangeToKnowIfRaycastShouldBeUsed) return true;

            Vector3 predictedDir = PredictFutureDirection(c.gameObject);

            RaycastHit rayHit;

            int numberOfRaycasts = 15;
            float degreesBetweenRays = 5;

            for (int i = -numberOfRaycasts; i < numberOfRaycasts; i++)
            {
                var angle = i * degreesBetweenRays;
                Vector3 dir = Quaternion.AngleAxis(angle, Vector3.up) * predictedDir;


                Physics.Raycast(eyes.transform.position, predictedDir + dir, out rayHit, detectionRange, 1 << 0);
                Debug.DrawRay(eyes.transform.position, (predictedDir + dir) * detectionRange, Color.darkOrange, 1);
                if (rayHit.collider != null && rayHit.transform.CompareTag("Player"))
                    return true;
            }
            //Debug.Log($"Cant see - distance {distance}");
            return false;
        }
        //Debug.Log("Couldn't find");
        return false;
    }
    //public bool PlayerAroundSphere()
    //{
    //    var colliders = Physics.OverlapSphere(detectionGameObject.position, 10, 1 << 0);
    //    foreach (var c in colliders)
    //    {
    //        if (c.gameObject.GetComponent<Movement>() == null)
    //        {
    //            continue;
    //        }
    //        float rangeToKnowIfRaycastShouldBeUsed = 2;
    //        float distance = Vector3.Distance(c.transform.position, detectionGameObject.position);
    //        if (distance <= rangeToKnowIfRaycastShouldBeUsed) return true;

    //        Vector3 predictedDir = PredictFutureDirection(c.gameObject);

    //        RaycastHit rayHit;
    //        var ray = new Ray(detectionGameObject.position, predictedDir);

    //       RaycastHit[] colliders2 = Physics.SphereCastAll(ray, 5, 10);

    //        int numberOfRaycasts = 15;
    //        float degreesBetweenRays = 5;

    //        for (int i = -numberOfRaycasts; i < numberOfRaycasts; i++)
    //        {
    //            var angle = i * degreesBetweenRays;
    //            Vector3 dir = Quaternion.AngleAxis(angle, Vector3.up) * predictedDir;


    //            Physics.Raycast(eyes.transform.position, predictedDir + dir, out rayHit, detectionRange, 1 << 0);
    //            Debug.DrawRay(eyes.transform.position, (predictedDir + dir) * detectionRange, Color.darkOrange, 1);
    //            if (rayHit.collider != null && rayHit.transform.CompareTag("Player"))
    //                return true;
    //        }
    //        Debug.Log($"Cant see - distance {distance}");
    //        return false;
    //    }
    //    Debug.Log("Couldn't find");
    //    return false;
    //}
}
