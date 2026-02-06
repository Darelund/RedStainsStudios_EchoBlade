using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Intro : MonoBehaviour
{
    [SerializeField] private List<GameObject> scrollingTextGameObjects = new List<GameObject>();
    [SerializeField] private float scrollSpeed = 10f;
    [SerializeField] private CanvasGroup fadePanelCanvasGroup;
    private bool BeginFade = false;
    
    private void Update()
    {
      


        foreach (GameObject go in scrollingTextGameObjects)
        {
            go.transform.position += new Vector3(0, scrollSpeed, 0) * Time.deltaTime;

        }
        if (BeginFade is true) return;
        if (scrollingTextGameObjects[scrollingTextGameObjects.Count - 1].transform.position.y > 2300)
        {
            BeginFade = true;
            SkipAll();
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            SkipForward();
        }
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            BeginFade = true;
            SkipAll();
        }
    }
    private void SkipForward()
    {
        foreach (GameObject go in scrollingTextGameObjects)
        {
            Debug.Log("Skip forward");
            go.transform.position += new Vector3(0, scrollSpeed * 400, 0) * Time.deltaTime;
        }
          
    }
    private void SkipAll()
    {
        StartCoroutine(FadeCoroutine());
    }

    private void LoadFirstScene()
    {
        SceneManager.LoadScene("Level_Morgue");
    }
    private IEnumerator FadeCoroutine()
    {
        float start = 0;
        float end = 1;
        while(start < end)
        {
            start += Time.deltaTime * 0.3f;
            fadePanelCanvasGroup.alpha = Mathf.Lerp(0, end, start);
            yield return null;
        }
        LoadFirstScene();
    }
}
