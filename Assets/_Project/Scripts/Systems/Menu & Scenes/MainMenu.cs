using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour, ISavable
{
    [SerializeField] private Button GraveyardBtn;
    [SerializeField] private Button ManorBtn;
    [SerializeField] private Button EscapeBtn;
    [SerializeField] public string selectedLevel;



    public void LoadLevelPLAY()
    {
        SceneManager.LoadScene(selectedLevel);
    }

    public void LoadLevelMM()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void Exit()
    {
        Application.Quit();
    }
    public void SelectLevel(string level)
    {
        selectedLevel = level;
    }
    public void Save(GameData gameData)
    {
        gameData.LastPlayedScene = selectedLevel;
    }

    public void Load(GameData gameData)
    {
        selectedLevel = gameData.LastPlayedScene;

       if (gameData.GraveyardUnlocked is false)
        {
            GraveyardBtn.gameObject.SetActive(false);
        }
        if (gameData.ManorUnlocked is false)
        {
            ManorBtn.gameObject.SetActive(false);
        }
        if (gameData.EscapeUnlocked is false)
        {
            EscapeBtn.gameObject.SetActive(false);
        }
    }
}
