using System.Security.Cryptography;
using System.Xml;
using TMPro;
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
    private TMP_Text keyText;
    private int keyNumber;
    private void Awake()
    {
        iconImage.sprite = defaultEmptySprite;
        keyText = GetComponentInChildren<TMP_Text>();
        keyText.text = keyNumber.ToString();
        if (isEmpty is true)
            GetComponentInChildren<TMP_Text>().enabled = false;
    }

    public void ResetAbilityBar()
    {
        isEmpty = true;
        iconImage.sprite = defaultEmptySprite;
        GetComponentInChildren<TMP_Text>().enabled = false;
    }

    public void SetNewIcon(Sprite newSprite, Sprite newCooldownSprite, int Id)
    {
        if (IsAbilityBarEmpty() is false) return;
        isEmpty = false;
        GetComponentInChildren<TMP_Text>().enabled = true;
        keyNumber = (Id + 1);
        keyText.text = keyNumber.ToString();
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
                abilityData.AbilityBarKey = keyNumber;
                return;
            }
        }

        gameData.abilityBarData.Add(new AbilityBarData() { ID = Unique_ID, IsEmpty = isEmpty, IconSpriteID = iconImage.sprite.name, CooldownSpriteID = cooldownImage.sprite.name, AbilityBarKey = keyNumber });
    }

    public void Load(GameData gameData)
    {
        if (gameData.abilityBarData == null) return;
        if (gameData.abilityBarData.Count <= 0) return;


        foreach (AbilityBarData abilityData in gameData.abilityBarData)
        {
            if (abilityData.ID == Unique_ID)
            {
                isEmpty = abilityData.IsEmpty;
                iconImage.sprite = SpriteDataBase.Instance.Get(abilityData.IconSpriteID);
                cooldownImage.sprite = SpriteDataBase.Instance.Get(abilityData.CooldownSpriteID);
                keyNumber = abilityData.AbilityBarKey;
                keyText.text = keyNumber.ToString();
                if (isEmpty is false)
                {
                    GetComponentInChildren<TMP_Text>().enabled = true;
                }
                return;
            }
        }
    }
}
