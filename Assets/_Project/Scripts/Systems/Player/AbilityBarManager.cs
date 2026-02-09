using System.Collections.Generic;
using UnityEngine;

public class AbilityBarManager : MonoBehaviour
{
    #region Singleton
    private static AbilityBarManager instance;
    public static AbilityBarManager Instance => instance;


    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
            return;
        }

        instance = this;
    }
    #endregion

    [SerializeField] private List<AbilityBar> abilityBarList = new List<AbilityBar>();

   // private int currentEmptyBar = 0;

    public void SetNewIcon(Sprite newSprite, Sprite newCooldownSprite)
    {
        foreach (var abilityBar in abilityBarList)
        {
            //Debug.Log("Ball");
            if (abilityBar.IsAbilityBarEmpty())
            {
                Debug.Log("Change Icon!!");
                abilityBar.SetNewIcon(newSprite, newCooldownSprite);
                return;
            }
        }
        Debug.LogError("NO SKILLBARS LEFT, anyways. Probably a dog problem");
    }
    public void ResetEveryAbilityBar()
    {
        foreach (var abilityBar in abilityBarList)
        {
            abilityBar.ResetAbilityBar();
        }
    }
}
