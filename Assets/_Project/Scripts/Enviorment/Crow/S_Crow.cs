using UnityEngine;

public class S_Crow : MonoBehaviour
{
    public AudioClip[] Squaks;
    public AudioClip[] Flaps;
    public float minVolume = .7f;
    public float maxVolume = 1.0f;
    public float minPitch = .95f;
    public float maxPitch = 1.05f;
    
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void PlayRandom(AudioClip[] group)
    {
        if (group == null || group.Length == 0) return;
        int index = Random.Range(0, group.Length);
        AudioClip clip = group[index];
        float vol = Random.Range(minVolume, maxVolume);
        float pitch = Random.Range(minPitch, maxPitch);
        audioSource.pitch = pitch;
        audioSource.PlayOneShot(clip, vol);
    }
    
    //Callers for Animation Controller Events
    public void Squak() { PlayRandom(Squaks); }
    public void Flap()  { PlayRandom(Flaps); }
    
}
