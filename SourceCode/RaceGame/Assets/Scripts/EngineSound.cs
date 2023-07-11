using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class EngineSound : MonoBehaviour
{

    AudioSource audioSourceFire;
    AudioSource audioSourceTurbo;
    AudioSource audioSourceEngine;
    AudioSource audioSourceDrift;
    AudioSource audioSourceBrake;
    AudioSource audioSourceRevAlarm;

    /*---------------インスペクターに表示させるオブジェクトや変数等---------------*/

    [Header("オーディオソース")]
    [SerializeField]
    private AudioClip ENGINE_SOUND;
    [Header("ボリューム")]
    [SerializeField]
    private float VOLUME;
    [Header("最大ピッチ")]
    [SerializeField]
    private float MAX_PITCH;
    [Header("最低ピッチ")]
    [SerializeField]
    private float MIN_PITCH;

    // Use this for initialization
    void Start()
    {
        audioSourceFire = gameObject.AddComponent<AudioSource>();
        audioSourceTurbo = gameObject.AddComponent<AudioSource>();
        audioSourceEngine = gameObject.AddComponent<AudioSource>();
        audioSourceDrift = gameObject.AddComponent<AudioSource>();
        audioSourceBrake = gameObject.AddComponent<AudioSource>();
        audioSourceRevAlarm = gameObject.AddComponent<AudioSource>();

        //ドリフト音
        audioSourceDrift.clip = Resources.Load("Sounds/drift", typeof(AudioClip)) as AudioClip;
        audioSourceDrift.volume = 0;
        audioSourceDrift.loop = true;
        audioSourceDrift.Play();

        //ドリフト音
        audioSourceBrake.clip = Resources.Load("Sounds/brakenoise", typeof(AudioClip)) as AudioClip;
        audioSourceBrake.volume = 0;
        audioSourceBrake.loop = true;
        audioSourceBrake.Play();

        //エンジン音
        audioSourceEngine.clip = ENGINE_SOUND;
        audioSourceEngine.loop = true;
        audioSourceEngine.volume = VOLUME;
        audioSourceEngine.Play();

        //レブアラーム
        audioSourceRevAlarm.clip = Resources.Load("Sounds/revAlarm", typeof(AudioClip)) as AudioClip;
        audioSourceRevAlarm.loop = true;
        audioSourceRevAlarm.volume = 0.5f;
    }
    
    public void FirePlay(AudioClip AC, float Volume)
    {
        audioSourceFire.clip = AC;
        audioSourceFire.volume = Volume;

        audioSourceFire.PlayOneShot(AC);
    }

    public void TurboPlay(AudioClip AC, float Volume)
    {
        audioSourceTurbo.clip = AC;
        audioSourceTurbo.volume = Volume;

        audioSourceTurbo.PlayOneShot(AC);
    }

    public void GetEngineRPM(float nowRPM,float MAX_RPM)
    {
        audioSourceEngine.pitch = MIN_PITCH + (((MAX_PITCH - MIN_PITCH) / MAX_RPM) * nowRPM);
    }

    public void DriftSound(float volume)
    {
        audioSourceDrift.volume = volume;
    }

    public void BrakeSound(float volume)
    {
        audioSourceBrake.volume = volume;
    }

    public void RevAlarm(bool isPlay)
    {
        if(isPlay)audioSourceRevAlarm.Play();
        else audioSourceRevAlarm.Stop();
    }
}
