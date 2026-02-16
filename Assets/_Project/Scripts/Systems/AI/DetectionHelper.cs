using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


public enum DetectionState
{
    DetectNone,
    Chase,
    Detect,
    Investigate
}


[System.Serializable]
public class InterestPoint
{
    public Vector3 Position;
    public Vector3 Direction;
    public InterestPoint(Vector3 position, Vector3 direction)
    {
        Position = position;
        Direction = direction;
    }
    public InterestPoint() { }
}
public class DetectionHelper
{
    private Transform detectorObject;
    private Transform eyes;
    private GameObject target;
    private List<Transform> extremityPoints;

    private float timeInSight;
    //private float CHASE_THRESHOLD = 2.5f;
    //private float INVESTIGATE_THRESHOLD = 1f;

    //private float DetectionSpeed = 4f;
    private float eyeSightAngle = 28f;
    private float detectionRange = 10;
    private readonly LightChanger lightChanger;
    public DetectionHelper(Transform transform, Transform eyes, LightChanger lightChanger)
    {
        detectorObject = transform;
        target = GameObject.FindAnyObjectByType<PlayerController>().gameObject;
        this.eyes = eyes;
        this.lightChanger = lightChanger;
        eyeSightAngle = lightChanger.angle / 2;
        detectionRange = lightChanger.range;

        //extremityPoints = target.GetComponent<Movement>().ExtremityPoints;
        extremityPoints = new List<Transform>();
        extremityPoints.Add(target.GetComponentInChildren<Animator>().GetBoneTransform(HumanBodyBones.Head).transform);
        extremityPoints.Add(target.GetComponentInChildren<Animator>().GetBoneTransform(HumanBodyBones.Chest).transform);
        extremityPoints.Add(target.GetComponentInChildren<Animator>().GetBoneTransform(HumanBodyBones.Neck).transform);
        extremityPoints.Add(target.GetComponentInChildren<Animator>().GetBoneTransform(HumanBodyBones.Spine).transform);
        extremityPoints.Add(target.GetComponentInChildren<Animator>().GetBoneTransform(HumanBodyBones.Hips).transform);
        extremityPoints.Add(target.GetComponentInChildren<Animator>().GetBoneTransform(HumanBodyBones.LeftFoot).transform);
        extremityPoints.Add(target.GetComponentInChildren<Animator>().GetBoneTransform(HumanBodyBones.RightFoot).transform);
        extremityPoints.Add(target.GetComponentInChildren<Animator>().GetBoneTransform(HumanBodyBones.LeftHand).transform);
        extremityPoints.Add(target.GetComponentInChildren<Animator>().GetBoneTransform(HumanBodyBones.RightHand).transform);
        extremityPoints.Add(target.GetComponentInChildren<Animator>().GetBoneTransform(HumanBodyBones.LeftShoulder).transform);
        extremityPoints.Add(target.GetComponentInChildren<Animator>().GetBoneTransform(HumanBodyBones.RightShoulder).transform);
        Debug.Log(extremityPoints.Count);
    }
    public GameObject GetTarget() => target;
    //Collider[] colliders;
    //int maxColliders = 10;
    public DetectionState Detect(float CHASE_THRESHOLD = 1.8f, float INVESTIGATE_THRESHOLD = 0.8f)
    {
        UpdateLight();

        Vector3 forwardDir = detectorObject.forward;
        Vector3 targetDir = (target.transform.position - detectorObject.position);
        var distanceToPlayer = targetDir.magnitude;

        //Player too far away
        if(distanceToPlayer >= detectionRange) return DetectionState.DetectNone;
       // Debug.Log("Inside detection range");

        //Check if the player is inside FOV
        float angle = Vector3.Angle(targetDir, forwardDir);
        if (angle <= eyeSightAngle)
        {
            //Debug.Log("Inside eye sight");
          //  Vector3 predictedDir = PredictFutureDirection(target.gameObject);
            var detectResult = ShootOutRays(CHASE_THRESHOLD);
            if (detectResult == DetectionState.Detect || detectResult == DetectionState.Chase) return detectResult;
        }
       

       
        if (timeInSight > INVESTIGATE_THRESHOLD)
        {
            return DetectionState.Investigate;
        }
        //Debug.Log(timeInSight);
        return DetectionState.DetectNone;
    }

    private DetectionState ShootOutRays(float CHASE_THRESHOLD = 2.5f)
    {
        RaycastHit rayHit;

        for (int i = 0; i < extremityPoints.Count; i++)
        {
            Vector3 predictedDir = (eyes.position - extremityPoints[i].transform.position).normalized;
            //Vector3 predictedDir = PredictFutureDirection(extremityPoints[i].transform);
            Physics.Raycast(extremityPoints[i].transform.position, predictedDir, out rayHit, detectionRange, (1 << 0) | (1 << 6) | (1 << 13) | (1 << 15));
            
            //Debug.Log($"Extremity position: {extremityPoints[9].GetComponentInParent<Animator>().GetBoneTransform(HumanBodyBones.Head).transform.position}");
            //if (rayHit.collider != null)
            //    Debug.Log($"Name of hit object - {rayHit.collider.name} and target transform is {detectorObject.transform.name}");
            if (rayHit.collider != null && rayHit.transform == detectorObject.transform && target.transform.gameObject.layer != 11)
            {
                //TODO: It shouldn't only be about time insight
                //it should also be how close you are. If you are in the face of the
                //enemy then obviously it should instantly chase/do something
                float distance = Vector3.Distance(target.transform.position, rayHit.collider.transform.position);
                float closeness = 1 - (distance / detectionRange); //1 to make closer higher
                var clampedClosness = Mathf.Clamp01(closeness); //We don't want negative values
                var detectionSpeed = Mathf.Lerp(2f, 10f, clampedClosness);

                timeInSight += detectionSpeed * Time.deltaTime;
                Debug.Log(timeInSight);
                Debug.DrawRay(extremityPoints[i].transform.position, (predictedDir) * detectionRange, Color.green, 1);
                //Chase after being in sight for x seconds
                if (timeInSight > CHASE_THRESHOLD)
                {
                    //Debug.Log("Time to chase");
                    return DetectionState.Chase;
                }
                UpdateLight();
                //lightChanger.ChangeVisibilityColor(timeInSight);
                //Debug.Log("Detecting");
                //Stand still and look at the thing you detect
                return DetectionState.Detect;
            }
            else //Missed
            {
                if (rayHit.collider != null && rayHit.transform.gameObject != null)
                    Debug.Log(rayHit.transform.gameObject.name);
                Debug.DrawRay(extremityPoints[i].transform.position, (predictedDir) * detectionRange, Color.darkRed, 1);
            }
        }
        //Debug.Log("Nothing detected");
        return DetectionState.DetectNone;
    }
    private void UpdateLight()
    {
        if (timeInSight > 0)
        {
            timeInSight -= Time.deltaTime;
        }
        else
        {
            timeInSight = 0;
        }
        lightChanger.ChangeVisibilityColor(timeInSight);
    }
    Vector3 lastPosition;
    private Vector3 PredictFutureDirection(Transform transformTarget)
    {
        float distance = Vector3.Distance(eyes.position, transformTarget.position);

        Vector3 velocity = (((target.transform.position) - lastPosition) / Time.deltaTime) * 0.35f;
        lastPosition = target.transform.position;

        float predictionTime = distance / detectorObject.GetComponent<NavMeshAgent>().speed;

        Vector3 futurePosition =
        (target.transform.position) + velocity * predictionTime;
        Vector3 predictedDir = (eyes.position - futurePosition).normalized;
        return predictedDir;
    }


    //TODO: Right now it finds all default layers, dumb, fix later. Make one layer for things that the enemy should interact with when they see it
    public bool PlayerAround()
    {

        float autoDetectionRange = 5; //Target is close enough so we know the target isn't hiding
        float distance = Vector3.Distance(target.transform.position, detectorObject.position);
        //Debug.Log($"Distance to player {distance}");
        //No need to check, player can't hide
        if (distance <= autoDetectionRange)
        {
            Debug.Log("Auto Detection");
            return true;
        }


        //Well player is to far away
        if (distance > detectionRange)
        {
            Debug.Log("Too far away");
            return false;
        }

   


        //Vector3 predictedDir = PredictFutureDirection(target.gameObject);
        //GameObject.FindAnyObjectByType<Volume>().profile.TryGet<DepthOfField>(out DepthOfField d);
        //d.active = true;

        RaycastHit rayHit;

        for (int i = 0; i < extremityPoints.Count; i++)
        {
            Vector3 predictedDir = (eyes.position - extremityPoints[i].position).normalized;
            //Vector3 predictedDir = PredictFutureDirection(extremityPoints[i].transform);
            // Vector3 predictedDir = PredictFutureDirection();
            Physics.Raycast(extremityPoints[i].transform.position, predictedDir, out rayHit, detectionRange, (1 << 0) | (1 << 6) | (1 << 13) | (1 << 15));
            if (rayHit.collider != null && rayHit.transform == detectorObject.transform && target.transform.gameObject.layer != 11)
            {
                Debug.DrawRay(extremityPoints[i].transform.position, (predictedDir) * detectionRange, Color.darkGreen, 1);
                return true;
            }
            else
            {
                Debug.DrawRay(extremityPoints[i].transform.position, (predictedDir) * detectionRange, Color.darkOrange, 1);
            }
        }
        Debug.Log("Missed everything");
        //int numberOfRaycasts = 15;
        //float degreesBetweenRays = 5;

        //for (int i = -numberOfRaycasts; i < numberOfRaycasts; i++)
        //{
        //    var angle = i * degreesBetweenRays;
        //    Vector3 dir = Quaternion.AngleAxis(angle, Vector3.up) * predictedDir;


        //    Physics.Raycast(eyes.transform.position, predictedDir + dir, out rayHit, detectionRange, 1 << 0 | 1 << 6 | 1 << 13);
        //    Debug.DrawRay(eyes.transform.position, (predictedDir + dir) * detectionRange, Color.darkOrange, 1);
        //    if (rayHit.collider != null && rayHit.transform.CompareTag("Player"))
        //        return true;
        //}
        //Debug.Log($"Cant see - distance {distance}");
        return false;

    }
    public bool PlayerAround2()
    {
        float distance = Vector3.Distance(target.transform.position, detectorObject.position);
        if (distance > detectionRange) return false;

        RaycastHit rayHit;

        for (int i = 0; i < extremityPoints.Count; i++)
        {
            Vector3 predictedDir = (eyes.position - extremityPoints[i].position).normalized;
            // Vector3 predictedDir = PredictFutureDirection();
            Physics.Raycast(extremityPoints[i].transform.position, predictedDir, out rayHit, detectionRange, (1 << 0) | (1 << 6) | (1 << 13) | (1 << 15));
            Debug.DrawRay(extremityPoints[i].transform.position, (predictedDir) * detectionRange, Color.darkOrange, 1);
            if (rayHit.collider != null && rayHit.transform == detectorObject.transform && target.transform.gameObject.layer != 11)
                return true;
        }
        
        return false;

    }
}
