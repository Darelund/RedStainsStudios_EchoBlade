using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SFXManager : MonoBehaviour
{
    //Inspector
    [Header("Mixers")]
    public AudioMixer MasterAudioMixer;

    [Header("Slider Refrences")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider musicSlider;

    [Space(10)]
    [Header("Saving")]
    public SavePlayerPrefs volumeSaver;
    [Space(10)]

    //Private
    private const string MASTER_PARAM = "MasterVolume"; //Mixer Exposed Params
    private const string SFX_PARAM = "SFXVolume";
    private const string MUSIC_PARAM = "MusicVolume";

    public void Start()
    {
        if (volumeSaver != null && masterSlider != null && sfxSlider != null && musicSlider != null)
        {
            //Load Saved Values
            float masterVal = volumeSaver.LoadData("master");
            float sfxVal = volumeSaver.LoadData("sfx");
            float musicVal = volumeSaver.LoadData("music");

            //Clamp to Avoid log[0]
            masterVal = Mathf.Clamp(masterVal, 0.0001f, 1f);
            sfxVal = Mathf.Clamp(sfxVal, 0.0001f, 1f);
            musicVal = Mathf.Clamp(musicVal, 0.0001f, 1f);

            //Set the Values
            masterSlider.value = masterVal;
            sfxSlider.value = sfxVal;
            musicSlider.value = musicVal;

            //Set the Mixers
            SetMasterVolume(masterVal);
            SetSFXVolume(sfxVal);
            SetMusicVolume(musicVal);
        }
    }

    public void SetMasterVolume(float linear)
    {
        float db = LinearToDb(linear);
        MasterAudioMixer.SetFloat(MASTER_PARAM, db);

        if (volumeSaver != null) volumeSaver.SaveData("master", linear);
    }

    public void SetSFXVolume(float linear)
    {
        float db = LinearToDb(linear);
        MasterAudioMixer.SetFloat(SFX_PARAM, db);

        if (volumeSaver != null) volumeSaver.SaveData("sfx", linear);
    }

    public void SetMusicVolume(float linear)
    {
        float db = LinearToDb(linear);
        MasterAudioMixer.SetFloat(MUSIC_PARAM, db);

        if (volumeSaver != null) volumeSaver.SaveData("music", linear);
    }

    private float LinearToDb(float linear)
    {
        if (linear <= 0.0001f) return -80f;
        return Mathf.Log10(linear) * 20f;
    }
}
