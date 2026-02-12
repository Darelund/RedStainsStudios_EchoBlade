using System;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Lure : MonoBehaviour
{
    [SerializeField] ParticleSystem _particleSystem;
    [SerializeField] private float range;
    [SerializeField] private float cooldown;

    //[SerializeField] public Image coolDownImage;//All images should be seperate with an event. Now I have to do a dumb solution in the AbilityBar to make this work. We need to come up with a less dumb solution later - Vidar
    public static event Action<float> OnLureCoolDown;

    private float timer = 0;
    private float particletimer = 0;
    private bool isPlaying = false;

    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private InputAction lure;


    private Vector3 offset = new Vector3(0.5f, 0, 0.5f);
    //offset to fix enemies walking into eachother
    private void Start()
    {
        //if (coolDownImage != null)
        //    coolDownImage.fillAmount = 1;

        lure = inputActions.FindActionMap("Player").FindAction("Lure");

        lure.performed += Lure_Performed;
       
    }
    private void OnDisable()
    {
        lure.performed -= Lure_Performed;
        
    }
    private void Update()
    {
        if (timer >= 0)
        {
            timer -= Time.deltaTime;
            OnLureCoolDown?.Invoke(Mathf.Lerp(0, 1, timer));
            //if (coolDownImage != null)
            //    coolDownImage.fillAmount = ;
        }
        if (isPlaying)
        {
            particletimer += Time.deltaTime;
            if (particletimer > 1)
            {
                particletimer = 0;
                isPlaying = false;
                _particleSystem.Stop();
            }
        }
    }

    private void Lure_Performed(InputAction.CallbackContext obj)
    {
        if (PlayerAbilities.Instance.GetAbilityState(PlayerAbility.Lure) is false || timer > 0) return;
        if (isPlaying == false)
        {
            _particleSystem.Stop();
            _particleSystem.Play();
            isPlaying = true;
        }
       
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, range);
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.CompareTag("Enemy"))
            {
                EnemyController enemy = hitCollider.gameObject.GetComponent<EnemyController>();

                enemy.PointOfInterest.Position = transform.position + offset;
                offset += new Vector3(0.5f, 0, 0.5f);
                //lures the enemies to one specific point with a small offset
                enemy.SwitchState<EnemyInvestigateState>();
            }
        }
        offset = Vector3.zero;

        if (PlayerAbilities.Instance.GetAbilityState(PlayerAbility.AbilityHaste))
        {
            timer = cooldown * 0.8f;
        }
        else
        {
            timer = cooldown;
        }
        OnLureCoolDown?.Invoke(0);
        //if (coolDownImage != null)
        //    coolDownImage.fillAmount = 0;
        
        //resets the offset after using the lure
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
