using UnityEngine;

public class EndgamePoint : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private GameObject endScreen;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            audioSource.Play();
            GameManager.Instance.SwitchState<CutsceneState>();
            endScreen.gameObject.SetActive(true);
        }
    }


}
