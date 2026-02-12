using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundEffectLibrary : MonoBehaviour
{
    [SerializeField] private SoundEffectGroup[] soundEffectGroups;
    private Dictionary<string, List<AudioClip>> soundDictionary;

    private void Awake()
    {
        InitializeDictionary();
    }

    private void InitializeDictionary()
    {
        soundDictionary = new Dictionary<string, List<AudioClip>>();
        foreach (SoundEffectGroup soundEffectGroup in soundEffectGroups)
        {
            soundDictionary[soundEffectGroup.name.ToLower()] = soundEffectGroup.audioClips;
        }
    }

    public AudioClip GetRandomClip(string name)
    {
        List<AudioClip> audioClips;

        if (soundDictionary.ContainsKey(name) is false)
        {
            Debug.LogError($"Couldn't find a SoundDictionary by name: {name}");
            return null;
        }
        audioClips = soundDictionary[name];
        if (audioClips.Count <= 0) return null;


        return audioClips[Random.Range(0, audioClips.Count)];
    }
}

[System.Serializable]
public struct SoundEffectGroup
{
    public string name;
    public List<AudioClip> audioClips;
}