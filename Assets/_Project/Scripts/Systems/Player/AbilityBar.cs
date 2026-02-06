using System.Xml;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AbilityBar : MonoBehaviour, ISavable
{
    [SerializeField] private Sprite defaultEmptySprite;
    [SerializeField] private Image iconImage;
    [SerializeField] private Image cooldownImage;

    private bool isEmpty = true;
    public int Unique_ID;

    private void Awake()
    {
        iconImage.sprite = defaultEmptySprite;
    }

    public void ResetAbilityBar()
    {
        isEmpty = true;
        iconImage.sprite = defaultEmptySprite;

    }

    public void SetNewIcon(Sprite newSprite, Sprite newCooldownSprite)
    {
        if (IsAbilityBarEmpty() is false) return;
        isEmpty = false;

        iconImage.sprite = newSprite;
        cooldownImage.sprite = newCooldownSprite;
    }

    public bool IsAbilityBarEmpty()
    {
        return isEmpty;
    }

    public void Save(GameData gameData)
    {
        foreach (AbilityBarData abilityData in gameData.abilityBarData)
        {
            if (abilityData.ID == Unique_ID)
            {
                abilityData.IsEmpty = isEmpty;
                abilityData.IconSpriteID = iconImage.sprite.name;
                abilityData.CooldownSpriteID = cooldownImage.sprite.name;
                return;
            }
        }

        gameData.abilityBarData.Add(new AbilityBarData() { ID = Unique_ID, IsEmpty = isEmpty, IconSpriteID = iconImage.sprite.name, CooldownSpriteID = cooldownImage.sprite.name });
    }

    public void Load(GameData gameData)
    {
        if (gameData.altarData.Count <= 0) return;


        foreach (AbilityBarData abilityData in gameData.abilityBarData)
        {
            if (abilityData.ID == Unique_ID)
            {
                isEmpty = abilityData.IsEmpty;
                iconImage.sprite = SpriteDataBase.Instance.Get(abilityData.IconSpriteID);
                cooldownImage.sprite = SpriteDataBase.Instance.Get(abilityData.CooldownSpriteID);
                return;
            }
        }
    }
}
