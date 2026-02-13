using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SavingManager : MonoBehaviour
{
    #region Singleton
    private static SavingManager instance;
    public static SavingManager Instance => instance;

    public bool UseSingleton = true;
    private void Awake()
    {
      

        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        Initialize();
        if (UseSingleton is false) return;
        DontDestroyOnLoad(gameObject);
    }
    #endregion




    private List<ISavable> savables;
    private GameData gameData;
    private IFileSaver fileSaver;


    private void Start()
    {
       
    }
    private void Initialize()
    {
        //savables = GameObject.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).
        //   OfType<ISavable>().ToList();
        //savables.ForEach(s => Debug.Log(s));
        fileSaver = new JsonSaver();
        //LoadData();

        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        //SceneManager.sceneUnloaded += SceneManager_sceneUnloaded;
        Application.quitting += Application_quitting;
        //SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }

    //private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
    //{
    //    Debug.Log("Active scene changed");
    //    if (arg0 != null && arg11 )
    //        SaveData();
    //}

    //private void SceneManager_sceneUnloaded(Scene arg0)
    //{
    //    Debug.Log($"{arg0.name} sceneunloaded");

    //    SaveData();
    //    //if (arg0.name != null && GameManager.Instance.ScenesUnlocked.TryGetValue(arg0.name, out bool value2) is true)
    //    //{
    //    //    Debug.Log($"New scene: {arg0.name} changed");
    //    //    //Debug.Log($"Before {GameManager.Instance.ScenesUnlocked[arg1.name]}");
    //    //    //Debug.Log(GameManager.Instance.ScenesUnlocked[arg1.name]);
    //    //    GameManager.Instance.ScenesUnlocked[arg0.name] = true;
    //    //    Debug.Log($"After {GameManager.Instance.ScenesUnlocked[arg0.name]}");

    //    //}
    //}
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
        //SceneManager.sceneUnloaded += SceneManager_sceneUnloaded;
        Application.quitting -= Application_quitting;
    }


    #region Saving and Loading between scene changed and application quitting
    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        LoadData();
        if (arg0.name != null && GameManager.Instance != null && GameManager.Instance.ScenesUnlocked.ContainsKey(arg0.name) is true)
        {
            Debug.Log($"scene: {arg0.name} loaded");
            GameManager.Instance.ScenesUnlocked[arg0.name] = true;
        }
    }
    private void Application_quitting()
    {
        gameData.LastPlayedScene = SceneManager.GetActiveScene().name;
        SaveData();
    }
    #endregion

    public void SaveData()
    {
        foreach (var savable in savables)
        {
            //Debug.Log("Saving");
            savable.Save(gameData);
        }
        fileSaver.Save(gameData);
    }
    public void LoadData()
    {
        //if (gameData == null)
         gameData = fileSaver.Load();


        if (gameData == null)
        {
            Debug.LogError("Seems like this is your first time loading?" +
                "That means I will create a new save for you");
            gameData = GameData.CreateNewGameData();
            savables = GameObject.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).
            OfType<ISavable>().ToList();
            SaveData();
        }
        else
        {
            LoadSavables();
        }
    }
    public void NewGame()
    {
        fileSaver.DeleteFile();
        gameData = GameData.CreateNewGameData();
        LoadSavables();
        foreach (var savable in savables)
        {
            savable.Save(gameData);
        }
        fileSaver.Save(gameData);
    }
    private void LoadSavables()
    {
        savables = GameObject.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).
              OfType<ISavable>().ToList();
        foreach (var savable in savables)
        {
            savable.Load(gameData);
        }
    }
}
