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

    public void LoadScene()
    {
        var selectedLevel = GameObject.FindAnyObjectByType<MainMenu>().selectedLevel;
        //Debug.Log("SelectedLevel : " + selectedLevel);
        //Debug.Log(Application.dataPath + "/_Project/Scenes/Levels/" + selectedLevel + ".unity");
        //var sceneIndex = SceneUtility.GetBuildIndexByScenePath(Application.dataPath + "/_Project/Scenes/Levels/" + selectedLevel + ".unity");
        //Debug.Log(sceneIndex);
        Debug.Log("Switching scene");
        StartCoroutine(LoadSceneAsync(selectedLevel));
    }
    public void LoadDefaultScene()
    {
        var sceneIndex = SceneManager.GetSceneByName("IntroScene").buildIndex;
        //Debug.Log(sceneIndex);
        StartCoroutine(LoadSceneAsync("IntroScene"));
    }

    IEnumerator LoadSceneAsync(string SceneID)
    {
        yield return new WaitForSeconds(5f);
        AsyncOperation operation = SceneManager.LoadSceneAsync(SceneID);
        
        LoadingScreen.SetActive(true);
        
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            
            LoadingBarFillImage.fillAmount = progress;
            
            
            yield return null;
        }
        Debug.Log("Switched scene");
    }
}
