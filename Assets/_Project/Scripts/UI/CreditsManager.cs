using Unity.Cinemachine;
using UnityEngine;

public class CreditsManager : MonoBehaviour
{

    [SerializeField] private CinemachineCamera[] cameraPoints;
    [SerializeField] private CinemachineCamera currentCameraPoint;
    [SerializeField] private CinemachineCamera mainMenuCameraPoint;
    [SerializeField] private Camera mainCamera;


    private void OnEnable()
    {
        mainMenuCameraPoint.Priority = 0;
        cameraPoints[0].Priority = 10;
        currentCameraPoint = cameraPoints[0];
    }

    public void ChangeGravestone()
    {
        currentCameraPoint.Priority = 0;

        if(System.Array.IndexOf(cameraPoints, currentCameraPoint) == 10)
        {
            cameraPoints[0].Priority = 10;
            currentCameraPoint = cameraPoints[0];
        }

        else
        {
            cameraPoints[System.Array.IndexOf(cameraPoints, currentCameraPoint) + 1].Priority = 10;
            currentCameraPoint = cameraPoints[System.Array.IndexOf(cameraPoints, currentCameraPoint) + 1];
        }

    }

    public void ExitCredits()
    {
        currentCameraPoint.Priority = 0;
        mainMenuCameraPoint.Priority = 10;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            currentCameraPoint.Priority = 0;
            mainMenuCameraPoint.Priority = 10;
            gameObject.SetActive(false);
        }
    }


}
