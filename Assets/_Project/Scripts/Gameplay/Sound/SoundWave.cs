using UnityEngine;

public class SoundWave : MonoBehaviour
{
    private ParticleSystem soundWaveParticles;
    // [SerializeField] public float soundSpeed = 10f;
    //[SerializeField] public float duration = 10f;
    //[SerializeField] public float startSize = 1.5f;

    private void Awake()
    {
        soundWaveParticles = GetComponentInChildren<ParticleSystem>();
    }
    public void InitializeSoundWave(float dur, float size)
    {
        soundWaveParticles.Stop();
        var main = soundWaveParticles.main;
        main.duration = dur;
        main.startSize = new ParticleSystem.MinMaxCurve(size);
        soundWaveParticles.Play();
    }

    void Update()
    {
        //transform.localScale += new Vector3(Time.deltaTime, Time.deltaTime, Time.deltaTime) * soundSpeed;

        if (soundWaveParticles.isStopped)
        {
            Destroy(gameObject);
        }
    }
}
