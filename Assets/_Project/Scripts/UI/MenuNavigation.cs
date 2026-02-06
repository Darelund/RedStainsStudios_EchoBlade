using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class MenuNavigation : MonoBehaviour
{
    //camera variables
    [SerializeField] private Camera mainCamera;
    [SerializeField] private bool shouldMoveCamera;
    [SerializeField] private CinemachineCamera cameraFrom;
    [SerializeField] private CinemachineCamera cameraTo;

    //canvas variables
    [SerializeField] private GameObject menuToOpen;
    [SerializeField] private GameObject canvasToClose;

    public void OnButtonClick()
    {
        if(shouldMoveCamera)
        {
            cameraFrom.Priority = 0;
            cameraTo.Priority = 10;
        }

        menuToOpen.SetActive(true);
        canvasToClose.SetActive(false);
    }
}
