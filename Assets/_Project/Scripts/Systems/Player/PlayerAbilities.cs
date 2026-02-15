using System.Collections.Generic;
using System.Linq;
using UnityEngine;




[System.Serializable]
public enum PlayerAbility
{
    ShadowWalk,
    Tail,
    Lure,
    LightsOut,
    Jeff,
    SilentDakeDown,
    MovementSpeed,
    AbilityHaste,
    AbilityDuration,
    DetectionReduction
}
public class PlayerAbilities : MonoBehaviour, ISavable
{
    #region Singleton + Add Abilities

    private static PlayerAbilities instance;
    public static PlayerAbilities Instance => instance;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;
        //DontDestroyOnLoad(gameObject);


        AddingAbilities();

    }


    #endregion

    [SerializeField] private Dictionary<PlayerAbility, bool> abilities = new Dictionary<PlayerAbility, bool>();

    public bool GetAbilityState(PlayerAbility playerAbility)
    {
        return abilities[playerAbility];
    }
    public void ActivateAbility(PlayerAbility playerAbility)
    {
        if (playerAbility == PlayerAbility.MovementSpeed)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().speed *= 1.15f;
        }
        abilities[playerAbility] = true;
    }
   


    
    private void AddingAbilities()
    {
        abilities[PlayerAbility.ShadowWalk] = false;
        abilities[PlayerAbility.Tail] = false;
        abilities[PlayerAbility.Lure] = false;
        abilities[PlayerAbility.Jeff] = false;
        abilities[PlayerAbility.SilentDakeDown] = false; 
        abilities[PlayerAbility.LightsOut] = false;
        abilities[PlayerAbility.MovementSpeed] = false; 
        abilities[PlayerAbility.AbilityHaste] = false; 
        abilities[PlayerAbility.AbilityDuration] = false; 
        abilities[PlayerAbility.DetectionReduction] = false; 
    }

    public void Save(GameData gameData)
    {
        gameData.PlayerAbilityData.Find(a => a.PlayerAbility == PlayerAbility.ShadowWalk).HasAbility         = abilities[PlayerAbility.ShadowWalk];
        gameData.PlayerAbilityData.Find(a => a.PlayerAbility == PlayerAbility.Tail).HasAbility               = abilities[PlayerAbility.Tail];
        gameData.PlayerAbilityData.Find(a => a.PlayerAbility == PlayerAbility.Lure).HasAbility               = abilities[PlayerAbility.Lure];
        gameData.PlayerAbilityData.Find(a => a.PlayerAbility == PlayerAbility.Jeff).HasAbility               = abilities[PlayerAbility.Jeff];
        gameData.PlayerAbilityData.Find(a => a.PlayerAbility == PlayerAbility.SilentDakeDown).HasAbility     = abilities[PlayerAbility.SilentDakeDown];
        gameData.PlayerAbilityData.Find(a => a.PlayerAbility == PlayerAbility.LightsOut).HasAbility          = abilities[PlayerAbility.LightsOut];
        gameData.PlayerAbilityData.Find(a => a.PlayerAbility == PlayerAbility.MovementSpeed).HasAbility      = abilities[PlayerAbility.MovementSpeed];
        gameData.PlayerAbilityData.Find(a => a.PlayerAbility == PlayerAbility.AbilityHaste).HasAbility       = abilities[PlayerAbility.AbilityHaste];
        gameData.PlayerAbilityData.Find(a => a.PlayerAbility == PlayerAbility.AbilityDuration).HasAbility    = abilities[PlayerAbility.AbilityDuration];
        gameData.PlayerAbilityData.Find(a => a.PlayerAbility == PlayerAbility.DetectionReduction).HasAbility = abilities[PlayerAbility.DetectionReduction];
    }

    public void Load(GameData gameData)
    {
        abilities[PlayerAbility.ShadowWalk]         = gameData.PlayerAbilityData.Find(a => a.PlayerAbility == PlayerAbility.ShadowWalk).HasAbility;
        abilities[PlayerAbility.Tail]               = gameData.PlayerAbilityData.Find(a => a.PlayerAbility == PlayerAbility.Tail).HasAbility;
        abilities[PlayerAbility.Lure]               = gameData.PlayerAbilityData.Find(a => a.PlayerAbility == PlayerAbility.Lure).HasAbility;
        abilities[PlayerAbility.Jeff]               = gameData.PlayerAbilityData.Find(a => a.PlayerAbility == PlayerAbility.Jeff).HasAbility;
        abilities[PlayerAbility.SilentDakeDown]     = gameData.PlayerAbilityData.Find(a => a.PlayerAbility == PlayerAbility.SilentDakeDown).HasAbility;
        abilities[PlayerAbility.LightsOut]          = gameData.PlayerAbilityData.Find(a => a.PlayerAbility == PlayerAbility.LightsOut).HasAbility;
        abilities[PlayerAbility.MovementSpeed]      = gameData.PlayerAbilityData.Find(a => a.PlayerAbility == PlayerAbility.MovementSpeed).HasAbility;
        abilities[PlayerAbility.AbilityHaste]       = gameData.PlayerAbilityData.Find(a => a.PlayerAbility == PlayerAbility.AbilityHaste).HasAbility;
        abilities[PlayerAbility.AbilityDuration]    = gameData.PlayerAbilityData.Find(a => a.PlayerAbility == PlayerAbility.AbilityDuration).HasAbility;
        abilities[PlayerAbility.DetectionReduction] = gameData.PlayerAbilityData.Find(a => a.PlayerAbility == PlayerAbility.DetectionReduction).HasAbility;
    }
}
