using System;
using System.Collections.Generic;
using UnityEngine;


public enum SoundType
{
    Footstep,
    TakeDown
}
public class HearingManager : MonoBehaviour
{

    #region Singleton
    private static HearingManager instance;
    public static HearingManager Instance => instance;
    [SerializeField] private GameObject soundWavePrefab;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }
    #endregion



    public event Action<HeardSound> OnHearing;

  

    public void OnSoundWasEmitted(Vector3 soundLocation, SoundType hearingSound, /*,float soundIntensity*/ SoundWaveData soundWaveData)
    {
        if(soundWaveData.CreateSoundWave)
        {
         var soundWave = Instantiate(soundWavePrefab, soundLocation, Quaternion.identity);
            // soundWave.GetComponentInChildren<ParticleSystem>().dur
            soundWave.GetComponent<SoundWave>().InitializeSoundWave(soundWaveData.Duration, soundWaveData.MaxSize);
            //soundWave.GetComponent<SoundWave>().duration = soundWaveData.Duration;
            //soundWave.GetComponent<SoundWave>().startSize = soundWaveData.MaxSize;

        }


        OnHearing?.Invoke(new HeardSound(soundLocation, hearingSound /*soundIntensity)*/));
    }
    public class SoundWaveData
    {
        public float MaxSize;
        public float Speed;
        public float Duration;
        public bool CreateSoundWave = true;

        public SoundWaveData(float maxSize, float speed, float duration, bool createSoundWave)
        {
            MaxSize = maxSize;
            Speed = speed;
            Duration = duration;
            CreateSoundWave = createSoundWave;
        }
        public static SoundWaveData NoSoundWave()
        {
            return new SoundWaveData(0, 0, 0, false);
        }
    }
    public class HeardSound : EventArgs
    {
       public readonly Vector3 soundLocation;
       public readonly SoundType soundType;
       //public readonly float soundIntensity;
        public HeardSound(Vector3 soundLocation, SoundType hearingSound /*float soundIntensity*/)
        {
            this.soundLocation = soundLocation;
            this.soundType = hearingSound;
            //this.soundIntensity = soundIntensity;
        }
    }
}
