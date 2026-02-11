using UnityEngine;


//Basic script for testing damage on player
public class Weapon : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.SwitchState<DeathState>();
        }
    }
}
