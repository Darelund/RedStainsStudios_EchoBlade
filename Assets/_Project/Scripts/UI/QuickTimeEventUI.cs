using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public class QuickTimeEventUI : MonoBehaviour
{
    [SerializeField] private Slider timeSlider;
    [SerializeField] private TMP_Text keysText;
    [SerializeField] private CanvasGroup canvasGroup;

    private void Start()
    {
        GameObject.FindAnyObjectByType<QuickTimeEventState>().OnQuickTime += QuickTimeEventUI_OnQuickTime;
    }

    private void QuickTimeEventUI_OnQuickTime(QuickTimeEventState.QuickTimeEventArgs obj)
    {
        //Debug.Log(obj.CurrentTime);
        timeSlider.value = obj.CurrentTime;

        for (int i = 0; i < keysText.text.Length; i++)
        {
            if(i < obj.CurrentKey)
            {
                StringBuilder sb = new StringBuilder(keysText.text);
                sb[i] = ' ';

                //var oldText = keysText.text;
                //var charText = oldText.ToCharArray();
                //charText[i] = ' ';
                //oldText = Convert.ToString(charText);
                keysText.text = sb.ToString();
            }
        }
    }

    public void InitializeUI(List<KeyControl> keySequence)
    {
        keysText.text = "";

        foreach (KeyControl key in keySequence)
        {
            if(key != null)
            keysText.text += key.displayName;
        }
    }
    public void Show()
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        canvasGroup.alpha = 1;
    }
    public void Hide()
    {
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        canvasGroup.alpha = 0;
    }
}
