using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityBarManager : MonoBehaviour
{
    #region Singleton
    private static AbilityBarManager instance;
    public static AbilityBarManager Instance => instance;


    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }
    #endregion

    [SerializeField] private List<AbilityBar> abilityBarList = new List<AbilityBar>();
   // private List<Image> abilityBarCooldown
   // private int currentEmptyBar = 0;

    public void SetNewIcon(Sprite newSprite, Sprite newCooldownSprite, int nodeId)
    {
        foreach (var abilityBar in abilityBarList)
        {
            //Debug.Log("Ball");
            if (abilityBar.IsAbilityBarEmpty())
            {
                Debug.Log("Change Icon!!");
                abilityBar.SetNewIcon(newSprite, newCooldownSprite, nodeId);
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
