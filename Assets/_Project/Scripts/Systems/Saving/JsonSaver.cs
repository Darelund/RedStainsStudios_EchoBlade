using System.IO;
using UnityEngine;

public class JsonSaver : IFileSaver
{
    private string FullPath = Application.persistentDataPath + Save_File_Name; //A folder inside AppData
    private const string Save_File_Name = "/GameSave.Json";
    //private string FullPath = Application.dataPath; //Should be a folder in Unity

    //Directory
    //File Stream - OpenRead
    //TODO: Do async in the future

    //Save it to file
    public void Save(GameData gameData)
    {
       string jsonData = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(FullPath, jsonData);
        //Debug.Log(FullPath);
    }
    //Load it from a file
    public GameData Load()
    {
        if (FileExists() is false) return null;

        string jsonData = File.ReadAllText(FullPath);
        GameData gameData = JsonUtility.FromJson<GameData>(jsonData);

        return gameData;
    }
    private bool FileExists()
    {
        bool fileExists = File.Exists(FullPath);
        return fileExists;
    }
   
}
