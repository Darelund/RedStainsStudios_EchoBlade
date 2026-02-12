using UnityEngine;
using UnityEngine.SceneManagement;

public class Player_SoundEffects : MonoBehaviour
{

    private AudioSource audioSource;

    private bool isInside = false;
    [SerializeField] private AudioClip[] footstepsGround;
    [SerializeField] private AudioClip[] footstepsFloor;
    [SerializeField] private AudioClip shadowwalkClip;
    [SerializeField] private AudioClip lureClip;
    [SerializeField] private AudioClip takedownClip;
    [SerializeField] private AudioClip deathClip;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (SceneManager.GetActiveScene().name != "Level_Graveyard")
        {
            isInside = true;
        }
    }

    public void PlayFootstep()
    {
        if(isInside)
        audioSource.PlayOneShot(footstepsFloor[Random.Range(0, footstepsFloor.Length)]);
        else
        audioSource.PlayOneShot(footstepsGround[Random.Range(0, footstepsGround.Length)]);
    }

    public void PlayShadowwalk()
    {
        audioSource.PlayOneShot(shadowwalkClip);
    }

    public void PlayLure()
    {
        audioSource.PlayOneShot(lureClip);
    }

    public void PlayTakedown() 
    {
        audioSource.PlayOneShot(takedownClip);
    }

    public void PlayDeath()
    {
        audioSource.PlayOneShot(deathClip);
    }

}
