using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;


public enum QuickTimeEventType
{
    RandomKey,
    ArrowKey
}

//TODO: Finish this
public class QuickTimeEventState : State
{
    [SerializeField] private GameObject abilityBarUI; 
    [SerializeField] private GameObject questScreenUI;
    private QuickTimeEventType eventType;
    public override void EnterState()
    {
        AIManager.Instance.StopAllAI();
        abilityBarUI.SetActive(false);
        questScreenUI.SetActive(false);

        eventType = (QuickTimeEventType)Random.Range(0, 2); //Randomly choose an event

        switch (eventType)
        {
            case QuickTimeEventType.RandomKey:
                break;
            case QuickTimeEventType.ArrowKey:
                break;
          
        }
    }
    public override void UpdateState()
    {
        return;
    }
    public override void ExitState()
    {
        StartCoroutine(ResumeState());
    }

    private IEnumerator ResumeState()
    {
        abilityBarUI.SetActive(true);
        questScreenUI.SetActive(true);
        yield return new WaitForSeconds(.6f);
        AIManager.Instance.ResumeAllAI();
    }


    private KeyControl[] GetRandomKeyboardKeys()
    {
        int amountOfKeyboardKeys = Random.Range(6, 12);
        KeyControl[] keyControls = new KeyControl[amountOfKeyboardKeys];
        for (int i = 0; i < keyControls.Length; i++)
        {
            var randomKey = (Keyboard.current[(Key)Random.Range(0, Keyboard.KeyCount - 1)]);
            if (randomKey.keyCode == Key.UpArrow ||
               randomKey.keyCode == Key.DownArrow ||
               randomKey.keyCode == Key.LeftArrow ||
               randomKey.keyCode == Key.RightArrow)
            {
                //Don't use these keys
                i--;
                continue;
            }
            keyControls[i] = randomKey;
        }
        return keyControls;
    }
    private KeyControl[] GetRandomArrowKeys()
    {
        int amountOfKeys = Random.Range(5, 12);
        KeyControl[] keyControls = new KeyControl[amountOfKeys];
        //Keyboard.current.
        for (int i = 0; i < keyControls.Length; i++)
        {
            keyControls[i] = RandomArrowKey();
        }
        return keyControls;
    }
    private KeyControl RandomArrowKey()
    {
        List<KeyControl> keyControls = new List<KeyControl>()
        {
            Keyboard.current.upArrowKey,
            Keyboard.current.leftArrowKey,
            Keyboard.current.rightArrowKey,
            Keyboard.current.downArrowKey
        };
        return keyControls[Random.Range(0, keyControls.Count)];
    }
}
