using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class DeathState : State, ISavable
{
    [SerializeField] private GameObject DeathScreenUI;
    public int currentNumberOfDeaths;
    public override void EnterState()
    {
        //DeathScreenUI.SetActive(true);
        //Show death screen

        //Time.timeScale = 0;
        GameObject.FindAnyObjectByType<Movement>().GetComponentInChildren<Animator>().Play("Dying");
        currentNumberOfDeaths++;
        SavingManager.Instance.SaveData();



        GameObject.FindAnyObjectByType<Volume>().profile.TryGet(out DepthOfField d);
        d.active = true;
        StartCoroutine(FadeInDeathScreenCoroutine());
    }
    public override void UpdateState()
    {

    }
    private IEnumerator FadeInDeathScreenCoroutine()
    {
        CanvasGroup canvasGroup = DeathScreenUI.GetComponent<CanvasGroup>();
        float startAlpha = 0;
        float endAlpha = 1;

        while(startAlpha < endAlpha)
        {
            startAlpha += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, startAlpha);
            yield return null;
        }
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
    public void CloseDeathScreenUI()
    {
        Time.timeScale = 1;
    }

    public void Save(GameData gameData)
    {
        gameData.Deaths = currentNumberOfDeaths;
    }

    public void Load(GameData gameData)
    {
        currentNumberOfDeaths = gameData.Deaths;
    }
}
