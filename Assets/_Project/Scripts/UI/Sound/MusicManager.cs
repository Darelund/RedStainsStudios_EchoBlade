using UnityEngine;
using UnityEngine.UI;
public class SoundManager : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private Slider musicSlider;
    public SavePlayerPrefs volumeSaver;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (volumeSaver == null) return;

        float savedVolume = volumeSaver.LoadData("music");
        if (musicSlider != null)
        {
            musicSlider.SetValueWithoutNotify(savedVolume);
        }
        audioSource.volume = savedVolume;



    }

    public void SliderEvent_SetVolume(float volume)
    {
        audioSource.volume = volume;
        if (volumeSaver == null) return;
        volumeSaver.SaveData("music", volume);

    }
}
