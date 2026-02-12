using UnityEngine;

public interface IFileSaver
{
    public void Save(GameData gameData);
    public GameData Load();
    bool FileExists();
    void DeleteFile();
}
