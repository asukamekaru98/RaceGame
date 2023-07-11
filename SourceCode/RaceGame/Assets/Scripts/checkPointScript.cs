using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class checkPointScript : MonoBehaviour
{
    /*-----------------------------------スクリプト-----------------------------------*/
    private GameMainManager gM;//ゲームメインマネージャー
    private CommonVariable cV;//共通変数
    private SoundManager sM;//サウンドマネージャー

    /*-----------------------------------サウンド-----------------------------------*/
    private AudioClip soundCheckPoint;
    private AudioClip goalSE;

    //各チェックポイントのオブジェクト
    private GameObject check1Obj, check2Obj, check3Obj, check4Obj;
    private GameObject timeExtendUI;
    //セクションテキスト
    private Text section1Txt, section2Txt, section3Txt, section4Txt;

    /*---------------列挙型---------------*/
    private enum CheckPoints
    {
        SEC1, SEC2, SEC3, SEC4, NONE
    }
    CheckPoints checkPoints;

    /*---------------定数---------------*/
    private const float ADD_TIME = 60;//追加する時間
    private const float ALPHA_MAX = 10.0f;//タイムエクステンドUIの最大アルファ値
    private const float ALPHA_SPEED = 0.1f;

    /*---------------変数---------------*/
    float check1Time, check2Time, check3Time, check4Time;
    float tEUIAlpha;//タイムエクステンドUIのアルファ値

    bool timeExtendFlg;

    [HideInInspector]
    public bool goalJudge;//ゴール判定用

    /* ======================================================================= *
     * 初期化
     * ======================================================================= */
    void Start()
    {
        //スクリプト読み込み
        gM = GameObject.Find("GameMainManager").GetComponent<GameMainManager>();//ゲームメインマネージャー
        cV = GameObject.Find("CommonVariable").GetComponent<CommonVariable>();//共通変数
        sM = GameObject.Find("SoundManager").GetComponent<SoundManager>();//サウンドマネージャー

        switch (cV.gameMode)
        {
            case CommonVariable.GameMode.TIMEATTACK:

                //チェックポイントオブジェクトの読み込み
                check1Obj = GameObject.Find("SectionTimeImg/SectionTime1FrameImg/SectionTime1Txt");
                check2Obj = GameObject.Find("SectionTimeImg/SectionTime2FrameImg/SectionTime2Txt");
                check3Obj = GameObject.Find("SectionTimeImg/SectionTime3FrameImg/SectionTime3Txt");
                check4Obj = GameObject.Find("SectionTimeImg/SectionTime4FrameImg/SectionTime4Txt");
                timeExtendUI = GameObject.Find("timeExtendUI");

                section1Txt = check1Obj.GetComponent<Text>();
                section2Txt = check2Obj.GetComponent<Text>();
                section3Txt = check3Obj.GetComponent<Text>();
                section4Txt = check4Obj.GetComponent<Text>();

                //チェックポイント初期化
                checkPoints = CheckPoints.SEC1;
                break;
            case CommonVariable.GameMode.BATTLE:

                //チェックポイント初期化
                checkPoints = CheckPoints.SEC4;
                break;
        }




        //サウンド読み込み
        soundCheckPoint = Resources.Load("Sounds/checkpoint", typeof(AudioClip)) as AudioClip;//ポン
        goalSE = Resources.Load("Sounds/goal", typeof(AudioClip)) as AudioClip;

        //タイムエクステンドUIのアルファ値の初期化
        tEUIAlpha = 0;

        goalJudge = false;

        //タイムエクステンドUIを表示させない！！！！！！！！！！！！！！！！！！！！！！！！！！！！
        timeExtendFlg = false;
    }

    /* ======================================================================= *
     * アプデ
     * ======================================================================= */
    void Update()
    {
        switch (cV.gameMode)
        {
            case CommonVariable.GameMode.TIMEATTACK:

                float second;//秒数入れ

                switch (checkPoints)
                {
                    case CheckPoints.SEC1:
                        //タイム計算
                        check1Time = gM.totalTime;

                        //分ミリ秒の計算
                        gM.sectionTimes[0, 0] = gM.CalMinute(check1Time);

                        second = gM.CalSecond(check1Time);

                        gM.sectionTimes[0, 1] = (int)second;
                        gM.sectionTimes[0, 2] = (int)(1000 * (second - (int)second));

                        //テキスト表示
                        section1Txt.text = gM.sectionTimes[0, 0].ToString("00") + "'" + gM.sectionTimes[0, 1].ToString("00") + "\"" + gM.sectionTimes[0, 2].ToString("000");
                        break;
                    case CheckPoints.SEC2:
                        //タイム計算
                        check2Time = gM.totalTime - check1Time;

                        //分ミリ秒の計算
                        gM.sectionTimes[1, 0] = gM.CalMinute(check2Time);

                        second = gM.CalSecond(check2Time);

                        gM.sectionTimes[1, 1] = (int)second;
                        gM.sectionTimes[1, 2] = (int)(1000 * (second - (int)second));

                        //テキスト表示
                        section2Txt.text = gM.sectionTimes[1, 0].ToString("00") + "'" + gM.sectionTimes[1, 1].ToString("00") + "\"" + gM.sectionTimes[1, 2].ToString("000");
                        break;
                    case CheckPoints.SEC3:
                        //タイム計算
                        check3Time = gM.totalTime - check1Time - check2Time;

                        //分ミリ秒の計算
                        gM.sectionTimes[2, 0] = gM.CalMinute(check3Time);

                        second = gM.CalSecond(check3Time);

                        gM.sectionTimes[2, 1] = (int)second;
                        gM.sectionTimes[2, 2] = (int)(1000 * (second - (int)second));

                        //テキスト表示
                        section3Txt.text = gM.sectionTimes[2, 0].ToString("00") + "'" + gM.sectionTimes[2, 1].ToString("00") + "\"" + gM.sectionTimes[2, 2].ToString("000");
                        break;
                    case CheckPoints.SEC4:
                        //タイム計算
                        check4Time = gM.totalTime - check1Time - check2Time - check3Time;

                        //分ミリ秒の計算
                        gM.sectionTimes[3, 0] = gM.CalMinute(check4Time);

                        second = gM.CalSecond(check4Time);

                        gM.sectionTimes[3, 1] = (int)second;
                        gM.sectionTimes[3, 2] = (int)(1000 * (second - (int)second));

                        //テキスト表示
                        section4Txt.text = gM.sectionTimes[3, 0].ToString("00") + "'" + gM.sectionTimes[3, 1].ToString("00") + "\"" + gM.sectionTimes[3, 2].ToString("000");
                        break;
                }
                break;

            case CommonVariable.GameMode.BATTLE:

                break;
        }
    }

    /* ======================================================================= *
     * フィックストアプデ
     * ======================================================================= */
    void FixedUpdate()
    {
        //タイムエクステンドUIのアニメーション
        if (timeExtendFlg) timeExtendFlg = TimeExtendUIAnimation();
    }

    /* ======================================================================= *
     * タイムエクステンド アニメーション
     * ======================================================================= */
    bool TimeExtendUIAnimation()
    {
        float WHITE = 255;

        //濃ゆくなる
        if (tEUIAlpha <= ALPHA_MAX)
        {
            //アルファ値を増やす
            tEUIAlpha += ALPHA_SPEED;

            //タイムエクステンドUIを表示させる
            timeExtendUI.SetActive(true);
        }
        else
        {
            //タイムエクステンドUIのアルファ値の初期化
            tEUIAlpha = 0;

            //タイムエクステンドUIを非表示にさせる
            timeExtendUI.SetActive(false);
            return false;
        }

        //アルファ値追加
        timeExtendUI.GetComponent<Image>().color = new Color(WHITE, WHITE, WHITE, tEUIAlpha);

        return true;
    }

    /* ======================================================================= *
     * 当たり判定
     * チェックポイント
     * ======================================================================= */
    void OnTriggerEnter(Collider other)
    {
        switch (checkPoints)
        {
            case CheckPoints.SEC1:
                //チェックポイント通過
                if (other.gameObject.name == "check1")
                {
                    //タイム追加
                    cV.remainigTime += ADD_TIME;

                    //タイムエクステンドUIのアニメーションをさせる
                    timeExtendFlg = true;

                    //SEを鳴らす
                    sM.SEPlay(soundCheckPoint, false, 1);// SE:チェックポイント

                    //次のタイムを計算させる
                    checkPoints = CheckPoints.SEC2;
                }
                break;
            case CheckPoints.SEC2:
                //チェックポイント通過
                if (other.gameObject.name == "check2")
                {
                    //タイム追加
                    cV.remainigTime += ADD_TIME;

                    //タイムエクステンドUIのアニメーションをさせる
                    timeExtendFlg = true;

                    //SEを鳴らす
                    sM.SEPlay(soundCheckPoint, false, 1);// SE:チェックポイント

                    //次のタイムを計算させる
                    checkPoints = CheckPoints.SEC3;
                }
                break;
            case CheckPoints.SEC3:
                //チェックポイント通過
                if (other.gameObject.name == "check3")
                {
                    //タイム追加
                    cV.remainigTime += ADD_TIME;

                    //タイムエクステンドUIのアニメーションをさせる
                    timeExtendFlg = true;

                    //SEを鳴らす
                    sM.SEPlay(soundCheckPoint, false, 1);// SE:チェックポイント

                    //次のタイムを計算させる
                    checkPoints = CheckPoints.SEC4;
                }
                break;
            case CheckPoints.SEC4:
                //チェックポイント通過
                if (other.gameObject.tag == "GoalPoint")
                {
                    sM.SEPlay(goalSE, false, 1);// ゴールSE
                    goalJudge = true;

                    //次のタイムを計算させる
                    checkPoints = CheckPoints.NONE;
                }
                break;
        }
    }
}
