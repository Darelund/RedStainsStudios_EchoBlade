using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Tail : MonoBehaviour
{
    [SerializeField] private float duration;
    [SerializeField] private float range;
    [SerializeField] private float cooldown;

    //[SerializeField] public Image coolDownImage;//All images should be seperate with an event. Now I have to do a dumb solution in the AbilityBar to make this work. We need to come up with a less dumb solution later - Vidar
    public static event Action<float> OnTailCoolDown;
    private float timer = 0;
    private bool isTailing = false;
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private InputAction tail;
    [SerializeField] private SkinnedMeshRenderer playerVisuals;
    [SerializeField] private TrailRenderer playerShadow; //TODO: Add a real shadow later. Probably like a animated 2d sprite with canvas

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //if (coolDownImage != null)
        //    coolDownImage.fillAmount = 1;

        tail = inputActions.FindActionMap("Player").FindAction("Tail");
        tail.performed += Tail_Performed;
    }
    private void Tail_Performed(InputAction.CallbackContext obj)
    {
        if (PlayerAbilities.Instance.GetAbilityState(PlayerAbility.Tail) is false) return;

        if (timer <= 0)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, range);

            List<GameObject> enemies = new List<GameObject>();

            foreach (Collider enemy in colliders)
            {
                if (enemy.gameObject.CompareTag("Enemy"))
                {
                    enemies.Add(enemy.gameObject);
                }
            }

            Debug.Log("Found " + enemies.Count + " enemies");

            if (enemies.Count != 0)
            {
                GameObject closestEnemy = GetClosestEnemy(enemies);
                if (closestEnemy == null) return;
                StartCoroutine(TailEnemy(closestEnemy));
            }
        }

    }
    private void OnDisable()
    {
        tail.performed -= Tail_Performed;
    }
    private void Update()
    {
        if (timer >= 0)
        {
            timer -= Time.deltaTime;
            OnTailCoolDown?.Invoke(Mathf.Lerp(0, 1, timer));
            //if (coolDownImage != null)
            //    coolDownImage.fillAmount = Mathf.Lerp(0, 1, timer);
        }
        if(isTailing is true)
        LeaveTailEarly();
    }


    //TODO: Add this to the input System so it works for more than keyboards
    private void LeaveTailEarly()
    {
        if(Mouse.current.rightButton.wasPressedThisFrame is true)
        {
            StopAllCoroutines();
            LeaveTail();
        }
    }

    private GameObject GetClosestEnemy(List<GameObject> enemies)
    {
        GameObject closestEnemy = null;
        float closestDistance = Mathf.Infinity;
        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (enemy.GetComponent<EnemyController>().IsDead()) continue;

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }
        return closestEnemy;
    }

    
    IEnumerator TailEnemy(GameObject enemy)
    {
        isTailing = true;
        // Enable tail effect
        GetComponent<PlayerController>().UseGravity = false;
        playerVisuals.enabled = false;
        playerShadow.enabled = true;
        gameObject.layer = 16; //No collision layer
        Vector3 offset = new Vector3(0, 0, -1f);

        float start = 0;
        float end = 1;

        while(start < end)
        {
            start += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, enemy.transform.TransformPoint(offset), start);
            yield return null;
        }


        Debug.Log("Tail effect activated");
        //playerVisuals.enabled = true;
        //playerShadow.enabled = false;
        gameObject.layer = 11;

        float abilityTimer;

        if (PlayerAbilities.Instance.GetAbilityState(PlayerAbility.AbilityDuration))
        {
            abilityTimer = duration * 1.5f;
        }
        else
        {
            abilityTimer = duration;
        }


        

        while (abilityTimer >= 0)
        {
            abilityTimer -= Time.deltaTime;



            transform.position = enemy.transform.TransformPoint(offset);

            yield return null;
        }
        LeaveTail();
    }
    private void LeaveTail()
    {
        isTailing = false;
        playerVisuals.enabled = true;
        playerShadow.enabled = false;
        gameObject.layer = 0;

        LeaveTailPosition();
        if (PlayerAbilities.Instance.GetAbilityState(PlayerAbility.AbilityHaste))
        {
            timer = cooldown * 0.8f;
        }
        else
        {
            timer = cooldown;
        }


        // Disable tail effect
        Debug.Log("Tail effect deactivated");
        GetComponent<PlayerController>().UseGravity = true;
    }
    private void LeaveTailPosition()
    {
        Camera cam = Camera.main;
        if (cam != null)
        {
            // convert mouse screen pos to world at the tail's current depth
            float screenZ = cam.WorldToScreenPoint(transform.position).z;
            Vector3 screenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenZ);
            Vector3 targetWorldPos = cam.ScreenToWorldPoint(screenPos);

            // compute world-space delta, clamp magnitude, then add the clamped delta to current position
            Vector3 delta = targetWorldPos - transform.position;
            Vector3 clampedDelta = Vector3.ClampMagnitude(delta, 1.5f);

            transform.position = transform.position + clampedDelta;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
