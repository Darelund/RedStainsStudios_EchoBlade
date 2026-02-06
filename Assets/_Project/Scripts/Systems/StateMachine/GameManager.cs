using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : StateMachine, ISavable
{
    private int currentSkillPoints = 0;
    public int skillPointsUsed;

    public Dictionary<string, bool> ScenesUnlocked = new Dictionary<string, bool>();
    //public bool GraveyardUnlocked;
    //public bool ManorUnlocked;
    //public bool EscapeUnlocked;

    public int GetCurrentSkillPoints() => currentSkillPoints;

    public void IncreaseSkillPoints(int skillPointsAdded)
    {
        currentSkillPoints += skillPointsAdded;
    }
    public void DecreaseSkillPoints(int skillPointsRemoved)
    {
        skillPointsUsed++;
        currentSkillPoints -= skillPointsRemoved;
    }



    public static GameManager Instance;
    private void Awake()
    {
        ScenesUnlocked = new Dictionary<string, bool>()
        {
            ["Level_Graveyard"] = true,
            ["Level_Manor"] = false,
            ["Level_Escape"] = false
        };


        if (Instance == null || Instance == this) Instance = this;
        else Destroy(this);
    }

    private void Start()
    {
        SwitchState<PlayingState>();
    }
    private void Update()
    {
        UpdateStateMachine();
    }
    private void FixedUpdate()
    {
        FixedUpdateStateMachine();
    }
    public void BackToMainMenu()
    {
        Time.timeScale = 1; //TODO: Remove this when we don't need it anymore
        SceneManager.LoadScene("Main Menu");
    }

    public void BackToDesktop()
    {
        Time.timeScale = 1; //TODO: Remove this when we don't need it anymore
        Application.Quit();
    }
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Save(GameData gameData)
    {
        gameData.SkillsPointsUsed = skillPointsUsed;
        gameData.SkillPointsLeft = currentSkillPoints;

        Debug.Log($"Grave: {ScenesUnlocked["Level_Graveyard"]},Manor: {ScenesUnlocked["Level_Manor"]}, Escape: {ScenesUnlocked["Level_Escape"]}");
        gameData.GraveyardUnlocked = ScenesUnlocked["Level_Graveyard"];
        gameData.ManorUnlocked = ScenesUnlocked["Level_Manor"];
        gameData.EscapeUnlocked = ScenesUnlocked["Level_Escape"];

    }

    public void Load(GameData gameData)
    {
        currentSkillPoints = gameData.SkillPointsLeft;
        skillPointsUsed = gameData.SkillsPointsUsed;

        ScenesUnlocked["Level_Graveyard"] = gameData.GraveyardUnlocked;
        ScenesUnlocked["Level_Manor"] = gameData.ManorUnlocked;
        ScenesUnlocked["Level_Escape"] = gameData.EscapeUnlocked;
    }
    public void ResetSkillPoints()
    {
        currentSkillPoints += skillPointsUsed;
        skillPointsUsed = 0;

        //TODO: Change back IconSprites
        //TODO: Reset AbilityBar images
      //  AbilityBarManager.Instance.
    }
}
