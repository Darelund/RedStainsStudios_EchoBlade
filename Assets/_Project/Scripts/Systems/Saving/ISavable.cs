using UnityEngine;

public interface ISavable
{
    public void Save(GameData gameData);
    public void Load(GameData gameData);
}
