using UnityEngine;


//Basic script for testing damage on player
public class DamageTest : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            if (other.gameObject.layer != 11)
                GameManager.Instance.SwitchState<DeathState>();
        }
    }
}
