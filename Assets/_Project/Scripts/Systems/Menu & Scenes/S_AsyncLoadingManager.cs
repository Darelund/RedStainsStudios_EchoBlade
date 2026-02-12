using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class S_AsyncLoadingManager : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera MainCamera;

    [Header("Loading")]
    public GameObject LoadingScreen;
    public TextMeshProUGUI LoadingPercentText;
    public TextMeshProUGUI LoadingDotsText;

    [Tooltip("% Lerp Speed")]
    public float smoothingSpeed = 200f;

    [Tooltip("Seconds between dot update")]
    public float dotInterval = 0.4f;

    [Tooltip("Minimum time (seconds) the loading screen should be visible.")]
    public float minLoadingTime = 3f;

    private int dotState;
    private float dotTimer;
    private float displayedPercent;
    
    private bool readyReached;
    private float readyTimestamp;   
    private float percentAtReady;   

    public void LoadScene()
    {
        string selectedLevel = GameObject.FindAnyObjectByType<MainMenu>().selectedLevel;
        Debug.Log("SelectedLevel : " + selectedLevel);
        int sceneIndex = SceneManager.GetSceneByName(selectedLevel).buildIndex;
        Debug.Log(sceneIndex);
        StartCoroutine(LoadSceneAsync(sceneIndex));
    }

    public void LoadDefaultScene()
    {
        int sceneIndex = SceneManager.GetSceneByName("Level_Graveyard").buildIndex;
        Debug.Log(sceneIndex);
        StartCoroutine(LoadSceneAsync(sceneIndex));
    }

    IEnumerator LoadSceneAsync(int SceneID)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(SceneID);
        if (operation != null)
        {
            operation.allowSceneActivation = false;

            // UI init
            if (LoadingScreen != null) LoadingScreen.SetActive(true);
            if (LoadingPercentText != null) LoadingPercentText.text = "0 %";
            if (LoadingDotsText != null) LoadingDotsText.text = "Loading.";

            displayedPercent = 0f;
            dotState = 0;
            dotTimer = 0f;
            readyReached = false;
            readyTimestamp = 0f;
            percentAtReady = 0f;

            float elapsed = 0f;

            while (!operation.isDone)
            {
                elapsed += Time.unscaledDeltaTime;

                float rawProgress = operation.progress; 
                float normalizedProgress = (rawProgress < 0.9f) ? Mathf.Clamp01(rawProgress / 0.9f) : 1f;
                
                if (!readyReached)
                {
                    if (normalizedProgress >= 1f)
                    {
                        readyReached = true;
                        readyTimestamp = elapsed;
                        percentAtReady = displayedPercent;
                    }
                    else
                    {
                        float targetPercent = Mathf.Round(normalizedProgress * 100f);
                        displayedPercent = Mathf.MoveTowards(displayedPercent, targetPercent,
                            smoothingSpeed * Time.unscaledDeltaTime);
                    }
                }
                else
                {
                    float timeSinceReady = elapsed - readyTimestamp;
                    float remainingWait =
                        minLoadingTime - readyTimestamp; 

                    if (remainingWait <= 0f)
                    {
                        displayedPercent = Mathf.MoveTowards(displayedPercent, 100f,
                            smoothingSpeed * Time.unscaledDeltaTime);
                    }
                    else
                    {
                        float t = Mathf.Clamp01(timeSinceReady / remainingWait);
                        t = Mathf.SmoothStep(0f, 1f, t); // nicer easing
                        displayedPercent = Mathf.Lerp(percentAtReady, 100f, t);
                    }
                }

                int displayInt = Mathf.RoundToInt(displayedPercent);
                if (LoadingPercentText != null) LoadingPercentText.text = displayInt.ToString() + " %";
                
                dotTimer += Time.unscaledDeltaTime;
                if (dotTimer >= dotInterval)
                {
                    dotTimer = 0f;
                    dotState = (dotState + 1) % 3;
                    if (LoadingDotsText != null)
                    {
                        string dots = new string('.', dotState + 1);
                        LoadingDotsText.text = "Loading" + dots;
                    }
                }
                
                bool isReady = rawProgress >= 0.9f;
                bool timePassed = elapsed >= minLoadingTime;
                if (isReady && timePassed)
                {
                    if (LoadingPercentText != null) LoadingPercentText.text = "100 %";
                    if (LoadingDotsText != null) LoadingDotsText.text = "Loading...";

                    operation.allowSceneActivation = true;
                }

                yield return null;
            }
        }
    }
}
