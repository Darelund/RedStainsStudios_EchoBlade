using UnityEngine;
using UnityEngine.Android;

public class InteractionPrompt : MonoBehaviour
{

    [SerializeField] private GameObject prompt;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") == true)
        {
            prompt.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") == true)
        {
            prompt.SetActive(false);
        }
    }


}
