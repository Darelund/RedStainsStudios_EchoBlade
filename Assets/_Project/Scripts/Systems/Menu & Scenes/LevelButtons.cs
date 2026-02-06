using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelButtons : MonoBehaviour
{
    public void LoadLevelPLAY()
    {
        SceneManager.LoadScene("Level_Graveyard");
    }

    public void LoadLevelMM()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
