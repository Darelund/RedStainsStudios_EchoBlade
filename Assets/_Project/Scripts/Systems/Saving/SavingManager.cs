using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        instance = this;
    }
    #endregion




    private List<ISavable> savables;
    private GameData gameData;
    private IFileSaver fileSaver;


    private void Start()
    {
        savables = GameObject.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).
            OfType<ISavable>().ToList();
        Debug.Log(savables.Count);
        // savables = GameObject.FindObjectsOfType<MonoBehaviour>()

        fileSaver = new JsonSaver();
        //LOAD DAT DATA!!!
        LoadData();

        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        Application.quitting += Application_quitting;
    }

   

    #region Saving and Loading between scene changed and application quitting
    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        LoadData();
    }

    private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
    {
        SaveData();
    }

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
            return;
        }
        foreach (var savable in savables)
        {
            savable.Load(gameData);
        }
    }
}
