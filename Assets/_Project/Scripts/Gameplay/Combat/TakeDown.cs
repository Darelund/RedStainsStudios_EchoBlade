using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TakeDown : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private InputAction takeDown;

    [SerializeField] private float cooldown;
    [SerializeField] private Image coolDownImage;

    private float timer = 0;

    bool canTakedown = false;

    GameObject enemy;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (coolDownImage != null)
            coolDownImage.fillAmount = 1;
        takeDown = inputActions.FindActionMap("Player").FindAction("TakeDown");

        takeDown.performed += TakeDown_performed;
    }

    private void TakeDown_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (canTakedown && GameObject.FindAnyObjectByType<PlayerAbilities>().GetAbilityState(PlayerAbility.SilentDakeDown) is false) return;

        if (timer > 0 || enemy != null)
        {
            GetComponent<Movement>().controller.Move(Vector3.zero);
            GetComponent<Movement>().DisableAllActions();
            StartCoroutine(CooldownCoroutine());
            Debug.Log("Takedown Executed!");
            GetComponentInChildren<Animator>().SetTrigger("AnimSilentTakedown");
            GetComponentInChildren<Animator>().Play("Kill");
            Debug.Log(enemy.name);
            enemy.GetComponent<EnemyController>().SwitchState<EnemyDeathState>();
            HearingManager.Instance.OnSoundWasEmitted(transform.position, SoundType.TakeDown, new HearingManager.SoundWaveData(4, 5, 4, true));

            //enemy.SetActive(false);
            if (PlayerAbilities.Instance.GetAbilityState(PlayerAbility.AbilityHaste))
            {
                timer = cooldown * 0.8f;
            }
            else
            {
                timer = cooldown;
            }
            if (coolDownImage != null)
                coolDownImage.fillAmount = 0;
        }
    }

    //This cooldown time is based on the length of the animation for take down
    private IEnumerator CooldownCoroutine()
    {
        yield return new WaitForSeconds(3.5f);
        GetComponent<Movement>().EnableAllActions();
    }
    private void OnDisable()
    {
        takeDown.performed -= TakeDown_performed;
    }
    // Update is called once per frame
    void Update()
    {
        if (timer >= 0)
        {
            timer -= Time.deltaTime;
            if (coolDownImage != null)
                coolDownImage.fillAmount = Mathf.Lerp(0, 1, timer);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Takedown!");
            enemy = other.gameObject;
            canTakedown = true;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Left Takedown Area!");
            enemy = null;
            canTakedown = false;
        }
    }
}
