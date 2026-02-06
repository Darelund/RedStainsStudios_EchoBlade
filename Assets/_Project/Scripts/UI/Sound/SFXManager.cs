using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class SFXManager : MonoBehaviour
{
    public AudioMixer sfxAudioMixer;
    [SerializeField] private Slider sfxSlider;
    public SavePlayerPrefs volumeSaver;
    private const string SFX_VOLUME_PARAM = "SFXVolume";
    public void Start()
    {
        if(sfxSlider != null && volumeSaver != null)
        {
            float savedValue = volumeSaver.LoadData("sfx");
            sfxSlider.value = savedValue;

            SetMixerVolume(savedValue);
        }
    }
    public void SetSFXVolume(float sliderValue)
    {

        SetMixerVolume(sliderValue);

        if(volumeSaver != null)
        {
            volumeSaver.SaveData("sfx", sliderValue);
        }

    }

    private void SetMixerVolume(float linearValue)
    {
        if (sfxAudioMixer == null)
        {
            Debug.LogError("Audio mixer reference is missing in ispector");
            return;
        }
        float volumeDb = 0f;

        if (linearValue <= 0.0001f)
        {
            volumeDb = -80f;
        }
        else
        {
            volumeDb = Mathf.Log10(linearValue) * 20;
        }

        sfxAudioMixer.SetFloat(SFX_VOLUME_PARAM, volumeDb);
    }

}
