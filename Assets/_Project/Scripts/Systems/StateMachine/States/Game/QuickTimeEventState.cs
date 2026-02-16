using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private GameObject QuickTimeUI;
    private QuickTimeEventType eventType;
    private List<KeyControl> keySequence = new();
    private KeyControl currentKey;
    private int currentKeyIndex;
    private Key keyboardInput;

    private float QuickEventTime = 30;
    private float currentTime = 30;


    private bool isOver = false;
    private bool succeeded = false;


    public event Action<QuickTimeEventArgs> OnQuickTime;

    public class QuickTimeEventArgs : EventArgs
    {
        public float CurrentTime;
        public int CurrentKey; //Will probably change it later
        public QuickTimeEventArgs(float currentTime, int currentKey)
        {
            CurrentTime = currentTime;
            CurrentKey = currentKey;
        }

    }

    private void Start()
    {
        abilityBarUI = GameObject.Find("AbilitybarPanel");
        questScreenUI = GameObject.Find("QuestlogPanel");
        QuickTimeUI = GameObject.Find("QuickTimeEventUI");
    }
    private void OnEnable()
    {
       // Keyboard.current. += Current_onTextInput;
    }
    private void OnDisable()
    {
      //  Keyboard.current.onTextInput -= Current_onTextInput;
    }
    public override void EnterState()
    {
       

        AIManager.Instance.StopAllAI();
        abilityBarUI?.SetActive(false);
        questScreenUI?.SetActive(false);
       
        eventType = (QuickTimeEventType)UnityEngine.Random.Range(0, 2); //Randomly choose an event

        switch (eventType)
        {
            case QuickTimeEventType.RandomKey:
                keySequence = GetRandomKeyboardKeys().ToList();
                break;
            case QuickTimeEventType.ArrowKey:
                keySequence = GetRandomArrowKeys().ToList();
                break;
          
        }
        QuickTimeUI?.GetComponent<QuickTimeEventUI>().Show();
        QuickTimeUI.GetComponent<QuickTimeEventUI>().InitializeUI(keySequence);
        currentKey = keySequence[currentKeyIndex];
    }
    public override void UpdateState()
    {
        if(isOver)
        {
            if(succeeded is true)
            {
                //Make enemy die???
                GameManager.Instance.SwitchState<PlayingState>();
            }
            else if(succeeded is true)
            {
                //Make enemy turn around or something?
                GameManager.Instance.SwitchState<PlayingState>();
            }
            return;
        }

        CheckPressedKey();
        GetKeyInput();
        CheckQuickTimeEventStatus();
        currentTime -= Time.deltaTime;
        OnQuickTime?.Invoke(new QuickTimeEventArgs((currentTime / QuickEventTime), currentKeyIndex)); //Tell UI about change
        return;
    }
    private void GetKeyInput()
    {
        if (currentKey != null && currentKey.keyCode == keyboardInput)
        {
            NextKey();
        }
    }
    private void NextKey()
    {
        currentKeyIndex++;
        if (currentKeyIndex < keySequence.Count)
            currentKey = keySequence[currentKeyIndex];
    }

   // Event even = Event.current;
    private void CheckPressedKey()
    {
        //  if (even.isKey is false) return;
        foreach (var key in Keyboard.current.allKeys)
        {
            if (key == null) continue;
            if(key.wasPressedThisFrame)
            {
                keyboardInput = key.keyCode;
            }
        }

        // if(even.type == EventType.KeyDown)
        //keyboardInput = (Key)even.keyCode;
        Debug.Log($"Keyboard input: {keyboardInput}");
    }

    private void CheckQuickTimeEventStatus()
    {
        if(currentKeyIndex >= keySequence.Count)
        {
            //If we have equal or more than means we have taken all keys
            //Won
            isOver = true;
            succeeded = true;
        }

        if(currentTime <= 0)
        {
            //Game Over
            isOver = true;
            succeeded = false;
        }
    }
    public override void ExitState()
    {
        StartCoroutine(ResumeState());
    }

    private IEnumerator ResumeState()
    {
        QuickTimeUI?.GetComponent<QuickTimeEventUI>().Hide();
        QuickTimeUI.SetActive(false);
        abilityBarUI.SetActive(true);
        questScreenUI.SetActive(true);
        yield return new WaitForSeconds(.6f);
        AIManager.Instance.ResumeAllAI();
    }


    private KeyControl[] GetRandomKeyboardKeys()
    {
        int amountOfKeyboardKeys = UnityEngine.Random.Range(6, 12);
        KeyControl[] keyControls = new KeyControl[amountOfKeyboardKeys];
        for (int i = 0; i < keyControls.Length; i++)
        {
            Key key = (Key)UnityEngine.Random.Range(0, Keyboard.KeyCount - 1);
           // var randomKey = (Keyboard.current[]);
            //if (key == Key.UpArrow ||
            //   key == Key.DownArrow ||
            //   key == Key.LeftArrow ||
            //   key == Key.RightArrow)
            //{
            //    //Don't use these keys
            //    i--;
            //    continue;
            //}
            if(key >= Key.A && key <= Key.Z)
            {
                keyControls[i] = Keyboard.current[key];
            }
            else
            {
                i--;
                continue;
            }
        }
        return keyControls;
    }
    private KeyControl[] GetRandomArrowKeys()
    {
        int amountOfKeys = UnityEngine.Random.Range(5, 12);
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
        return keyControls[UnityEngine.Random.Range(0, keyControls.Count)];
    }
}
