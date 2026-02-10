using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Lure : MonoBehaviour
{
    [SerializeField] private float range;
    [SerializeField] private float cooldown;

    [SerializeField] private Image coolDownImage;

    private float timer = 0;

    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private InputAction lure;


    private Vector3 offset = new Vector3(0.5f, 0, 0.5f);
    //offset to fix enemies walking into eachother
    private void Start()
    {
        if (coolDownImage != null)
            coolDownImage.fillAmount = 1;

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
            if (coolDownImage != null)
                coolDownImage.fillAmount = Mathf.Lerp(0, 1, timer);
        }
    }

    private void Lure_Performed(InputAction.CallbackContext obj)
    {
        if (PlayerAbilities.Instance.GetAbilityState(PlayerAbility.Lure) is false || timer > 0) return;


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
        if (coolDownImage != null)
            coolDownImage.fillAmount = 0;

        //resets the offset after using the lure
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
