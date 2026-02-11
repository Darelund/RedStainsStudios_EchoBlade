using System.Collections.Generic;
using UnityEngine;
public class SoundEffectManager : MonoBehaviour
{
    [SerializeField] private List<AudioSource> audioSources = new List<AudioSource>();
    private SoundEffectLibrary soundEffectLibrary;
    public SavePlayerPrefs volumeSaver;

    #region Singleton and initialization
    public static SoundEffectManager Instance;

  
    private void Awake()
    {
        if (Instance == null && volumeSaver != null)
        {
            Instance = this;
            soundEffectLibrary = GetComponent<SoundEffectLibrary>();
            LoadSavedVolume();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    private void LoadSavedVolume()
    {
        float savedVolume = volumeSaver.LoadData("sfx");
        foreach (AudioSource aSource in audioSources)
        {
            aSource.volume = savedVolume;
        }
    }
    public void PlayARandomSFX(string soundName, float pitch = 1)
    {
        //Find clip
        soundName = soundName.ToLower();
        AudioClip audioClip = soundEffectLibrary.GetRandomClip(soundName);

        if (audioClip == null) return;

        //Pick a random audio source to use
        foreach (AudioSource aSource in audioSources)
        {
            if (aSource.isPlaying is true) continue;

            aSource.pitch = pitch;
            aSource.PlayOneShot(audioClip);
            break;

        }
    }
}