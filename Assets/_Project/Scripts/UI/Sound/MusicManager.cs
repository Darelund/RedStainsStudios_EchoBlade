using UnityEngine;
using UnityEngine.UI;
public class MusicManager : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private Slider musicSlider;
    public SavePlayerPrefs volumeSaver;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (volumeSaver != null)
        {
            float savedVolume = volumeSaver.LoadData("music");
            if (musicSlider != null)
            {
                musicSlider.SetValueWithoutNotify(savedVolume);
            }
            audioSource.volume = savedVolume;


        }
    }

    public void SliderEvent_SetVolume(float volume)
    {

        audioSource.volume = volume;

        if (volumeSaver != null)
        {
            volumeSaver.SaveData("music", volume);
        }
    }
}
