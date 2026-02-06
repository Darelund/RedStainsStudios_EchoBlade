using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.GPUSort;

public class SavingManager : MonoBehaviour
{
    #region Singleton
    private static SavingManager instance;
    public static SavingManager Instance => instance;


    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
            return;
        }
        DontDestroyOnLoad(gameObject);
        instance = this;
    }
    #endregion




    private List<ISavable> savables;
    private GameData gameData;
    private IFileSaver fileSaver;


    private void Start()
    {
        savables = GameObject.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).
            OfType<ISavable>().ToList();

        savables.ForEach(s =>
            Debug.Log(s.GetType()) );
            // savables = GameObject.FindObjectsOfType<MonoBehaviour>()

            fileSaver = new JsonSaver();
        //LOAD DAT DATA!!!
        LoadData();

      //  SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        SceneManager.sceneUnloaded += SceneManager_sceneUnloaded;
        Application.quitting += Application_quitting;
    }

    private void SceneManager_sceneUnloaded(Scene arg0)
    {
        if (arg0.name != null && GameManager.Instance.ScenesUnlocked.TryGetValue(arg0.name, out bool value2) is true)
        {
            Debug.Log($"New scene: {arg0.name} changed");
            //Debug.Log($"Before {GameManager.Instance.ScenesUnlocked[arg1.name]}");
            //Debug.Log(GameManager.Instance.ScenesUnlocked[arg1.name]);
            GameManager.Instance.ScenesUnlocked[arg0.name] = true;
            Debug.Log($"After {GameManager.Instance.ScenesUnlocked[arg0.name]}");

        }

        SaveData();
    }



    #region Saving and Loading between scene changed and application quitting
    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if(arg0.name != null && GameManager.Instance.ScenesUnlocked.TryGetValue(arg0.name, out bool value) is true)
        {
            Debug.Log($"scene: {arg0.name} loaded");
            GameManager.Instance.ScenesUnlocked[arg0.name] = true;
        }
        savables = GameObject.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).
           OfType<ISavable>().ToList();
        SaveData();
        LoadData();
    }

    //private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
    //{
    //    if (arg0.name != null && GameManager.Instance.ScenesUnlocked.TryGetValue(arg0.name, out bool value) is true)
    //    {
    //        Debug.Log($"Old scene: {arg0.name} changed");
    //        GameManager.Instance.ScenesUnlocked[arg0.name] = true;
    //    }
    //    if (arg1.name != null && GameManager.Instance.ScenesUnlocked.TryGetValue(arg1.name, out bool value2) is true)
    //    {
    //        Debug.Log($"New scene: {arg1.name} changed");
    //        Debug.Log($"Before {GameManager.Instance.ScenesUnlocked[arg1.name]}");
    //        Debug.Log(GameManager.Instance.ScenesUnlocked[arg1.name]);
    //        GameManager.Instance.ScenesUnlocked[arg1.name] = true;
    //        Debug.Log($"After {GameManager.Instance.ScenesUnlocked[arg1.name]}");

    //    }

    //    SaveData();
    //}

    private void Application_quitting()
    {
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
        gameData = fileSaver.Load();


        if(gameData == null)
        {
            Debug.LogError("Seems like this is your first time loading?" +
                "That means I will create a new save for you");
            gameData = new GameData();
           
        }
        foreach (var savable in savables)
        {
            savable.Load(gameData);
        }
    }
}
