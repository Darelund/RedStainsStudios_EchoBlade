using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Door : MonoBehaviour
{
    [SerializeField] private string LevelName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Movement>() != null)
        {
            //QuestLog.instance.ProgressQuest();
            SavingManager.Instance.SaveData();
            SceneManager.LoadScene(LevelName ??= "LevelManor");
        }
    }
}
