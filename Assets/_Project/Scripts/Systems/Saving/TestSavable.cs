using UnityEngine;

public class TestSavable : MonoBehaviour, ISavable
{
    public int Number;
    public void Load(GameData gameData)
    {
        Number = gameData.Deaths;
        //Debug.Log($"I loaded data - {Number}");
    }

    public void Save(GameData gameData)
    {
        gameData.Deaths = Number;
    }
}
