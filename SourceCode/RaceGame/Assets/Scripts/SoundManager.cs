using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    AudioSource audioSourceBGM;
    AudioSource audioSourceWhoop;
    AudioSource audioSourceSE;

    private AudioClip soundWhoop;// SE:サイレン

    void Start()
    {
        DontDestroyOnLoad(this);//全シーンで共用

        //サウンド読み込み
        audioSourceSE = gameObject.AddComponent<AudioSource>();
        audioSourceBGM = gameObject.AddComponent<AudioSource>();
        audioSourceWhoop = gameObject.AddComponent<AudioSource>();

        soundWhoop = Resources.Load("Sounds/beepbeep", typeof(AudioClip)) as AudioClip;// SE:サイレン 
        audioSourceWhoop.clip = soundWhoop;
    }

    public void SEPlay(AudioClip AC, bool LoopFlg, float Volume)
    {
        audioSourceSE.clip = AC;
        audioSourceSE.loop = LoopFlg;
        audioSourceSE.volume = Volume;

        audioSourceSE.PlayOneShot(AC);
    }

    public void BGMPlay(AudioClip AC, bool LoopFlg, float Volume)
    {
        audioSourceBGM.clip = AC;
        audioSourceBGM.loop = LoopFlg;
        audioSourceBGM.volume = Volume;

        audioSourceBGM.Play();

    }

    public void StopBGM()
    {
        audioSourceBGM.Stop();
    }

    public void WhoopPlay(bool Play)
    {
        if(Play)audioSourceWhoop.Play();
        else audioSourceWhoop.Stop();
    }

}
