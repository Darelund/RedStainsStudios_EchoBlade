using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class GameData
{
    //TODO: Save what?
    //Tombstones interacted with
    //skills unlocked
    //Skillpoints left
    //Time played?
    //Number of deaths 
    //LastPlayedScene
    //Unlocked scenes
    public bool NewGame;

    public List<PlayerAbilityData> PlayerAbilityData;
    public List<AltarData> altarData;
    public List<SkillsNodeData> SkillsNodeData;
    public List<AbilityBarData> abilityBarData;
    public int SkillPointsLeft;
    public int SkillsPointsUsed;

    public int Deaths;
    public TimePlayedData TimePlayed;



    public string LastPlayedScene;

    public bool GraveyardUnlocked;
    public bool ManorUnlocked;
    public bool EscapeUnlocked;



    //Empty save, which is when we get a new save
    public static GameData CreateNewGameData()
    {
        GameData gameData = new GameData();
        //Debug.LogError("NEW GAME DATA");
        gameData.NewGame = true;

        gameData.PlayerAbilityData = new List<PlayerAbilityData>()
        {
            new PlayerAbilityData(PlayerAbility.ShadowWalk, false) {},
            new PlayerAbilityData(PlayerAbility.Tail, false) {},
            new PlayerAbilityData(PlayerAbility.Lure, false) {},
            new PlayerAbilityData(PlayerAbility.Jeff, false) {},
            new PlayerAbilityData(PlayerAbility.SilentDakeDown, false) { },
            new PlayerAbilityData(PlayerAbility.LightsOut, false) { },
            new PlayerAbilityData(PlayerAbility.MovementSpeed, false) { },
            new PlayerAbilityData(PlayerAbility.AbilityHaste, false) { },
            new PlayerAbilityData(PlayerAbility.AbilityDuration, false) { },
            new PlayerAbilityData(PlayerAbility.DetectionReduction, false) { }

        };
        gameData.altarData = new List<AltarData>();
        gameData.SkillsNodeData = new List<SkillsNodeData>();
        gameData.abilityBarData = new List<AbilityBarData>();
        gameData.SkillPointsLeft = 0;
        gameData.SkillsPointsUsed = 0;
 
        gameData.Deaths = 0;
        gameData.TimePlayed = new TimePlayedData(0, 0, 0);
      
        gameData.LastPlayedScene = "Level_Graveyard";
        gameData.GraveyardUnlocked = true;
        gameData.ManorUnlocked = false;
        gameData.EscapeUnlocked = false;

        return gameData;
    }

    //public GameData()
    //{
       
    //    NewGame = true;
    //    PlayerAbilityData = new List<PlayerAbilityData>()
    //    {
    //        new PlayerAbilityData(PlayerAbility.ShadowWalk, false) {},
    //        new PlayerAbilityData(PlayerAbility.Tail, false) {},
    //        new PlayerAbilityData(PlayerAbility.Lure, false) {},
    //        new PlayerAbilityData(PlayerAbility.Jeff, false) {},
    //        new PlayerAbilityData(PlayerAbility.SilentDakeDown, false) { },
    //        new PlayerAbilityData(PlayerAbility.LightsOut, false) { },
    //        new PlayerAbilityData(PlayerAbility.MovementSpeed, false) { },
    //        new PlayerAbilityData(PlayerAbility.AbilityHaste, false) { },
    //        new PlayerAbilityData(PlayerAbility.AbilityDuration, false) { },
    //        new PlayerAbilityData(PlayerAbility.DetectionReduction, false) { }

    //    };
    //    altarData = new List<AltarData>();
    //    SkillsNodeData = new List<SkillsNodeData>();
    //    abilityBarData = new List<AbilityBarData>();
    //    SkillPointsLeft = 0;
    //    SkillsPointsUsed = 0;

    //    Deaths = 0;
    //    TimePlayed = new TimePlayedData(0, 0, 0);

    //    LastPlayedScene = "Level_Graveyard";
    //    GraveyardUnlocked = true;
    //    ManorUnlocked = false;
    //    EscapeUnlocked = false;

    //}
}
[System.Serializable]
public class TimePlayedData
{
    public float Hours;
    public float Minutes;
    public float Seconds;

    public TimePlayedData(float hours, float minutes, float seconds)
    {
        Hours = hours;
        Minutes = minutes;
        Seconds = seconds;
    }
    public TimePlayedData GetTime() => new TimePlayedData(Hours, Minutes, Seconds);
}
[System.Serializable]
public class AltarData
{
    public int ID;
    public bool IsUsed;
}
[System.Serializable]
public class SkillsNodeData //What skills in the SkillTree that have been bought
{
    public int ID;
    public string SkillName;
    public bool IsUsed;
    public bool IsUnlocked;
    public List<bool> ConnectingPaths = new List<bool>();
}
[System.Serializable]
public class AbilityBarData //What Abilities that you have
{
    // I am not sure if I can save sprites, so I will have to come up with another way to get the right images
    public int ID;
    public bool IsEmpty;
    public string CooldownSpriteID;
    public string IconSpriteID;
    public int AbilityBarKey; //Weird, only some abilites use keys. So this is kinda dumb
    public PlayerAbility PlayerAbility;
}
[System.Serializable]
public class PlayerAbilityData //What abilites are unlocked
{
    //Don't have time to make dictionary work

    //public Dictionary<PlayerAbility, bool> Ability;

    //public PlayerAbilityData()
    //{
    //    Ability = new Dictionary<PlayerAbility, bool>()
    //    {
    //        [PlayerAbility.ShadowWalk] = false,
    //        [PlayerAbility.Tail] = false,
    //        [PlayerAbility.Lure] = false,
    //        [PlayerAbility.Jeff] = false,
    //        [PlayerAbility.SilentDakeDown] = false,
    //        [PlayerAbility.LightsOut] = false,
    //        [PlayerAbility.MovementSpeed] = false,
    //        [PlayerAbility.AbilityHaste] = false,
    //        [PlayerAbility.AbilityDuration] = false,
    //        [PlayerAbility.DetectionReduction] = false,
    //    };
    //}
    public string AbilityName;
    public PlayerAbility PlayerAbility;
    public bool HasAbility;
    public PlayerAbilityData(PlayerAbility playerAbility, bool hasAbility)
    {
        this.PlayerAbility = playerAbility;
        this.HasAbility = hasAbility;
        AbilityName = Convert.ToString(PlayerAbility);
    }
}
