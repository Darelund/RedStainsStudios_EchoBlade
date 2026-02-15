using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private Transform rotPivot;
    [SerializeField] private GameObject trail;

    public float speed;
    //[SerializeField] private float maxSpeed = 10;//Maybe add this one later
    [SerializeField] private float cooldown;

    public static event Action<float> OnPhaseCoolDown;

    private float timer = 0;

    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private InputAction moveAction;
    [SerializeField] private InputAction fade;

    [SerializeField] private SkinnedMeshRenderer playerSkinnedMeshRenderer;
    private Material playerDefaultMaterial;
    private Material playerShadowMaterial;

    [SerializeField] private float phasetime = 2f;


    private bool isPhasing = false;
    [SerializeField] private bool isRunning = false;
    [SerializeField] private bool isGrounded;
    public bool UseGravity = true;
    private bool canUseActions = true;


    public CharacterController controller;
    Vector3 moveInput;
    private Vector3 oldPos;
    private readonly float footstepPerSecond = 0.75f;
    private float footstepPerSecondTimer;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();

       
        playerDefaultMaterial = playerSkinnedMeshRenderer.material;
        playerShadowMaterial = new Material(playerDefaultMaterial);
        playerShadowMaterial.color = Color.dimGray;
    }
  
    public void EnableAllActions()
    {
      canUseActions = true;
    }
    private void OnEnable()
    {

        moveAction = inputActions.FindActionMap("Player").FindAction("Move");
        fade = inputActions.FindActionMap("Player").FindAction("Fade");
        inputActions.FindActionMap("Player").Enable();

        moveAction.performed += OnMovement_performed;
        moveAction.started += MoveAction_started;
        moveAction.canceled += OnMovement_Cancelled;

        inputActions.FindActionMap("Player").FindAction("Crouch").performed += OnCrouch_performed;
        inputActions.FindActionMap("Player").FindAction("Crouch").canceled += OnCrouch_Cancelled;

        fade.performed += Fade_performed;
    }

    private void MoveAction_started(InputAction.CallbackContext obj)
    {
        if (!canUseActions) return;

        anim.SetBool("isSneaking", true);
    }

    private void OnMovement_performed(InputAction.CallbackContext obj)
    {
        if (!canUseActions) return;
        moveInput = obj.ReadValue<Vector2>();
    }
    private void OnMovement_Cancelled(InputAction.CallbackContext obj)
    {
        if (!canUseActions) return;
        anim.SetBool("isSneaking", false);
      
        moveInput = Vector2.zero;
    }

    private void OnCrouch_performed(InputAction.CallbackContext obj)
    {
        if (!canUseActions) return;
        isRunning = true;
        anim.SetBool("isRunning", true);
    }
    private void OnCrouch_Cancelled(InputAction.CallbackContext obj)
    {
        if (!canUseActions) return;
        isRunning = false;
        anim.SetBool("isRunning", false);
    }

    public void DisableAllActions()
    {
        canUseActions = false;
    }
    private void OnDisable()
    {

        controller.Move(Vector3.zero);

        inputActions.FindActionMap("Player").FindAction("Crouch").performed -= OnCrouch_performed;
        inputActions.FindActionMap("Player").FindAction("Crouch").canceled -= OnCrouch_Cancelled;
        fade.performed -= Fade_performed;
        moveAction.performed -= OnMovement_performed;
        moveAction.canceled -= OnMovement_Cancelled;
        moveAction.started -= MoveAction_started;
    }

    private void Fade_performed(InputAction.CallbackContext obj)
    {
        if (PlayerAbilities.Instance.GetAbilityState(PlayerAbility.ShadowWalk) is false) return;

        if (!isPhasing && timer <= 0)
        {
            trail.SetActive(true);
            isPhasing = true;
            StartCoroutine(Phase());
            anim.SetTrigger("isSneaking");
            anim.Play("ShadowWalk");
        }
    }

    public void UpdateMovement()
    {
        if (!canUseActions) return;

        if (timer >= 0)
        {
            timer -= Time.deltaTime;
            OnPhaseCoolDown?.Invoke(Mathf.Lerp(0, 1, timer));

        }
        Vector3 movement = new Vector3(moveInput.x, 0, moveInput.y);

        if (UseGravity)
        {
            isGrounded = Physics.Raycast(transform.position, Vector3.down, out _, 0.2f);
            if (isGrounded is false)
            {
                controller.Move(new Vector3(0, (isGrounded is false ? -9.81f : 0), 0));
            }

        }


        movement = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0) * movement;

        controller.Move(movement * Time.deltaTime * (isRunning == true ? speed * 2 : speed));

        if (movement.sqrMagnitude > 0)
        {
            rotPivot.forward = Vector3.Lerp(rotPivot.forward, -(movement.normalized), 44 * Time.deltaTime);

            footstepPerSecondTimer += Time.deltaTime;
        }
        else
        {
            footstepPerSecondTimer = 0;
        }

        if (isRunning is true && movement.magnitude > 0 && footstepPerSecond < footstepPerSecondTimer)
        {
            footstepPerSecondTimer = 0;

            var dir = (transform.position - oldPos).normalized * 2;
            HearingManager.Instance.OnSoundWasEmitted(transform.position + dir, SoundType.Footstep, new HearingManager.SoundWaveData(10, 0, UnityEngine.Random.Range(1.0f, 1.0f), true));
        }
        oldPos = transform.position;
    }
  

    IEnumerator Phase()
    {
        LayerMask phaseableLayer = LayerMask.GetMask("Phaseable");
        playerSkinnedMeshRenderer.material = playerShadowMaterial;
        controller.excludeLayers = phaseableLayer;

        if (PlayerAbilities.Instance.GetAbilityState(PlayerAbility.AbilityDuration))
        {
            yield return new WaitForSeconds(phasetime * 1.5f);
        }
        else
        {
            yield return new WaitForSeconds(phasetime);
        }

        playerSkinnedMeshRenderer.material = playerDefaultMaterial;
        isPhasing = false;

        controller.excludeLayers = 0;
        trail.SetActive(false);

        if (PlayerAbilities.Instance.GetAbilityState(PlayerAbility.AbilityHaste))
        {
            timer = cooldown * 0.8f;
        }
        else
        {
            timer = cooldown;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.6f);
    }
}
