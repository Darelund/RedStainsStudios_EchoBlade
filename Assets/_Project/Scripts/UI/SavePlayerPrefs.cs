using UnityEngine;

public class SavePlayerPrefs : MonoBehaviour
{
    private float defaultVolume = 0.5f;

    public void SaveData(string key,float volume)
    {
        float clampedVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(key, clampedVolume);
        PlayerPrefs.Save();
    }
    public float LoadData(string key)
    {
        float savedVolume = PlayerPrefs.GetFloat(key, defaultVolume);

        return savedVolume;
    }
}
