using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalPointScript : MonoBehaviour
{
    /*-----------------------------------スクリプト-----------------------------------*/
    private SoundManager sM;

    /*-----------------------------------サウンド-----------------------------------*/
    private AudioClip goalSE;

    /*-----------------------------------変数-----------------------------------*/
    [HideInInspector]
    public bool goalJudge;//ゴール判定用

    /* ======================================================================= *
     * 初期化
     * ======================================================================= */
    void Start()
    {
        //スクリプト読み込み
        sM = GameObject.Find("SoundManager").GetComponent<SoundManager>();

        //サウンド読み込み
        goalSE = Resources.Load("Sounds/goal", typeof(AudioClip)) as AudioClip;

        goalJudge = false;
    }

    /* ======================================================================= *
     * 当たり判定
     * ======================================================================= */
    void OnTriggerEnter(Collider other)
    {
        //ゴールについたら
        if (other.gameObject.tag == "GoalPoint")
        {
            sM.SEPlay(goalSE, false, 1);// ゴールSE
            goalJudge = true;
        }
    }
}
