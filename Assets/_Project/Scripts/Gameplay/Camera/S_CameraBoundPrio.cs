using System;
using Unity.Cinemachine;
using UnityEngine;

public class S_CameraBoundPrio : MonoBehaviour
{
    public CinemachineCamera VCam;
    public int OverridePriority = 10;
    private int OriginalPriority;
    
    void Start()
    {
        OriginalPriority = VCam.Priority;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            VCam.Priority = OverridePriority;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            VCam.Priority = OriginalPriority;
    }
}
