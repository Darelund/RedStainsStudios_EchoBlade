using System.Security.Cryptography;
using System.Xml;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class AbilityBar : MonoBehaviour, ISavable
{
    [SerializeField] private Sprite defaultEmptySprite;
    [SerializeField] private Image iconImage;
    [SerializeField] private Image cooldownImage;

    private bool isEmpty = true;
    public int Unique_ID;
    private TMP_Text keyText;
    private int keyNumber;
    private int playerAbilityID;

    private void Awake()
    {
        iconImage.sprite = defaultEmptySprite;
        keyText = GetComponentInChildren<TMP_Text>();
        keyText.text = keyNumber.ToString();
        cooldownImage.fillAmount = 0; //Fill amount goes to 1 one some for some reason after you get some abilities. So dumb solve
        if (isEmpty is true)
            GetComponentInChildren<TMP_Text>().enabled = false;
    }
    private void OnDisable()
    {
        if (isEmpty is false)
        {
            PlayerAbility valueAsPlayerAbility = (PlayerAbility)playerAbilityID;
            switch (valueAsPlayerAbility)//I know, beautiful code, anyways I might make make it better in the future
            {
                case PlayerAbility.ShadowWalk:
                    Movement.OnPhaseCoolDown -= UpdateCoolDown;

                    break;
                case PlayerAbility.Tail:
                    Tail.OnTailCoolDown -= UpdateCoolDown;

                    break;
                case PlayerAbility.Lure:
                    Lure.OnLureCoolDown -= UpdateCoolDown;

                    break;
                case PlayerAbility.LightsOut:
                    Lights_Out.OnLightOutCoolDown -= UpdateCoolDown;

                    break;
                case PlayerAbility.SilentDakeDown:
                    TakeDown.OnTakeDownCoolDown -= UpdateCoolDown;

                    break;
            }
        }
    }
    public void ResetAbilityBar()
    {
        if(isEmpty is false)
        {
            PlayerAbility valueAsPlayerAbility = (PlayerAbility)playerAbilityID;
            switch (valueAsPlayerAbility)//I know, beautiful code, anyways I might make make it better in the future
            {
                case PlayerAbility.ShadowWalk:
                    Movement.OnPhaseCoolDown -= UpdateCoolDown;

                    break;
                case PlayerAbility.Tail:
                    Tail.OnTailCoolDown -= UpdateCoolDown;

                    break;
                case PlayerAbility.Lure:
                    Lure.OnLureCoolDown -= UpdateCoolDown;

                    break;
                case PlayerAbility.LightsOut:
                    Lights_Out.OnLightOutCoolDown -= UpdateCoolDown;

                    break;
                case PlayerAbility.SilentDakeDown:
                    TakeDown.OnTakeDownCoolDown -= UpdateCoolDown;

                    break;
            }
        }

        Debug.Log("Reseting");
        isEmpty = true;
        iconImage.sprite = defaultEmptySprite;
        GetComponentInChildren<TMP_Text>().enabled = false;

        
    }
    private void UpdateCoolDown(float value)
    {
        cooldownImage.fillAmount = value;
    }

    public void SetNewIcon(Sprite newSprite, Sprite newCooldownSprite, int Id, PlayerAbility Value)
    {
        if (IsAbilityBarEmpty() is false) return;
        isEmpty = false;
        GetComponentInChildren<TMP_Text>().enabled = true;
        keyNumber = (Id + 1);
        keyText.text = keyNumber.ToString();
        iconImage.sprite = newSprite;
        cooldownImage.sprite = newCooldownSprite;

        switch (Value)//I know, beautiful code, anyways I might make make it better in the future
        {
            case PlayerAbility.ShadowWalk:
                Movement.OnPhaseCoolDown += UpdateCoolDown;

                break;
            case PlayerAbility.Tail:
                Tail.OnTailCoolDown += UpdateCoolDown;

                break;
            case PlayerAbility.Lure:
                Lure.OnLureCoolDown += UpdateCoolDown;

                break;
            case PlayerAbility.LightsOut:
                Lights_Out.OnLightOutCoolDown += UpdateCoolDown;

                break;
            case PlayerAbility.SilentDakeDown:
                TakeDown.OnTakeDownCoolDown += UpdateCoolDown;

                break;
        }
        playerAbilityID = (int)Value;
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
                abilityData.PlayerAbility = (PlayerAbility)playerAbilityID;
                return;
            }
        }

        gameData.abilityBarData.Add(new AbilityBarData() { ID = Unique_ID, IsEmpty = isEmpty, IconSpriteID = iconImage.sprite.name, CooldownSpriteID = cooldownImage.sprite.name, AbilityBarKey = keyNumber, PlayerAbility = (PlayerAbility)playerAbilityID });
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
                    PlayerAbility valueAsPlayerAbility = abilityData.PlayerAbility;
                    switch (valueAsPlayerAbility)//I know, beautiful code, anyways I might make make it better in the future
                    {
                        case PlayerAbility.ShadowWalk:
                            Movement.OnPhaseCoolDown += UpdateCoolDown;

                            break;
                        case PlayerAbility.Tail:
                            Tail.OnTailCoolDown += UpdateCoolDown;

                            break;
                        case PlayerAbility.Lure:
                            Lure.OnLureCoolDown += UpdateCoolDown;

                            break;
                        case PlayerAbility.LightsOut:
                            Lights_Out.OnLightOutCoolDown += UpdateCoolDown;

                            break;
                        case PlayerAbility.SilentDakeDown:
                            TakeDown.OnTakeDownCoolDown += UpdateCoolDown;

                            break;
                    }
                    playerAbilityID = (int)valueAsPlayerAbility;
                }
                return;
            }
        }
    }
}
