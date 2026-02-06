using System;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

public class SpeechBubble : MonoBehaviour
{
    [SerializeField] private Transform lookTarget;
    private CinemachineBrain brain;

    private void Awake()
    {
        brain = GetComponent<CinemachineBrain>();
    }
    public void Init(Transform lookTarget)
    {
        this.lookTarget = lookTarget;
    }
    private void Update()
    {
        transform.forward = Vector3.right;
     //  transform.forward
       //transform.LookAt(Camera.main.transform);
    }
    //private void OnEnable()
    //{
    //    CinemachineCore.CameraActivatedEvent.AddListener(OnCameraChanged);
    //}
    //private void OnDisable()
    //{
    //    CinemachineCore.CameraActivatedEvent.RemoveListener(OnCameraChanged);
    //}

    //private void OnCameraChanged(ICinemachineCamera.ActivationEventParams arg0)
    //{
    //    lookTarget = arg0.IncomingCamera.
    //    //CinemachineCore.ins
    //}

}
