using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class S_AsyncLoadingManager : MonoBehaviour
{
    [Header("Camera")]
    [Space(4)]
    [SerializeField] private Camera MainCamera;
    [Space(10)]
    [Header("Loading")]
    [Space(4)]
    public GameObject LoadingScreen;
    public Image LoadingBarFillImage;

    public void LoadScene(int SceneID)
    {
        StartCoroutine(LoadSceneAsync(SceneID));
    }

    IEnumerator LoadSceneAsync(int SceneID)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(SceneID);
        
        LoadingScreen.SetActive(true);
        
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            
            LoadingBarFillImage.fillAmount = progress;
            
            
            yield return null;
        }
    }
}
