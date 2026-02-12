using UnityEngine;

public class EndgamePoint : MonoBehaviour
{

    [SerializeField] private GameObject endScreen;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            endScreen.gameObject.SetActive(true);
        }
    }


}
