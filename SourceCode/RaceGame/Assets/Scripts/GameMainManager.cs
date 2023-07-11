using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMainManager : MonoBehaviour
{
    AudioSource audioSource;
    /*-----------------------------------スクリプト-----------------------------------*/
    private resultTime rT;//リサルトタイム
    private rankUI rU;//ランクUI
    private CommonVariable cV;//共通変数
    private SoundManager sM;
    private CountDownScript cdS;

    /*-----------------------------------サウンド-----------------------------------*/
    private AudioClip mainBGM;
    private AudioClip goalBGM;

    /*---------------列挙型---------------*/
    private enum GearEnum
    {
        GearDown = -1, GearUp = 1
    }

    [HideInInspector]
    public enum Status
    {
        MUSIC, START, PLAY, GOAL, GAMEOVER
    }
    [HideInInspector]
    public Status carStatus;

    /*---------------インスペクターに表示させるオブジェクトや変数等---------------*/

    //[Header("プレイヤー別のカーオブジェクト")]
    [SerializeField]
    private GameObject player1CarObj, player2CarObj;

    /*---------------定数---------------*/

    private carScript carScript1p, carScript2p;
    private checkPointScript cPScript1p, cPScript2p;
    private GameObject CommonCanvas, CommonCanvasTime;

    [HideInInspector]
    public GameObject ccVBattle;

    /*---------------変数---------------*/
    [HideInInspector]
    public float totalTime;//トータルタイム
    [HideInInspector]
    public float maxSpeed;

    private float GameOverTime1P, GameOverTime2P;

    bool LButtonFlg1P, RButtonFlg1P;//LRボタンを一度だけ押すため1P用
    bool LButtonFlg2P, RButtonFlg2P;//LRボタンを一度だけ押すため2P用
    bool AButtonFlg1P;//Aボタンを一度だけ押すため1P用

    public int[,] sectionTimes = new int[4, 3];//分秒ミリ秒を入れるやつ（セクション）
    public int[,] totalTimes = new int[1, 3];//分秒ミリ秒を入れるやつ（トータル）

    /* ======================================================================= *
     * 初期化
     * ======================================================================= */
    private void Start()
    {
        //スクリプト読み込み
        cV = GameObject.Find("CommonVariable").GetComponent<CommonVariable>();
        cdS = GameObject.Find("CommonCanvas").GetComponent<CountDownScript>();
        sM = GameObject.Find("SoundManager").GetComponent<SoundManager>();

        //サウンド読み込み
        goalBGM = Resources.Load("Sounds/goalBGM", typeof(AudioClip)) as AudioClip;

        //---------------=1Pの車を召喚=---------------//
        switch (cV.carType1P)
        {
            //EK9の召喚
            case CommonVariable.CarType1P.Civic:
                //位置決め
                player1CarObj = Instantiate((GameObject)Resources.Load("Prefabs/cars/EK9"), new Vector3(1550.078f, 0.262351f, -5322.915f), Quaternion.Euler(0.586f, 123.253f, -4.231f));
                break;

            //ランエボの召喚
            case CommonVariable.CarType1P.Evo:
                //位置決め
                player1CarObj = Instantiate((GameObject)Resources.Load("Prefabs/cars/Evo6"), new Vector3(1550.078f, 0.262351f, -5322.915f), Quaternion.Euler(0.586f, 123.253f, -4.231f));
                break;

            //セリカの召喚
            case CommonVariable.CarType1P.Celica:
                //位置決め
                player1CarObj = Instantiate((GameObject)Resources.Load("Prefabs/cars/Celica"), new Vector3(1550.078f, 0.262351f, -5322.915f), Quaternion.Euler(0.586f, 123.253f, -4.231f));
                break;

            //スカイラインの召喚
            case CommonVariable.CarType1P.Skyline:
                //位置決め
                player1CarObj = Instantiate((GameObject)Resources.Load("Prefabs/cars/R33"), new Vector3(1550.078f, 0.262351f, -5322.915f), Quaternion.Euler(0.586f, 123.253f, -4.231f));
                break;
        }
        //カーオブジェクトの子オブジェクト（ゴールチェック）を取得
        cPScript1p = player1CarObj.transform.Find("GeneralPurposeColl").gameObject.GetComponent<checkPointScript>();
        //カースクリプト取得
        carScript1p = player1CarObj.GetComponent<carScript>();


        //モード別
        switch (cV.gameMode)
        {
            //対戦モード
            case CommonVariable.GameMode.BATTLE:

                //サウンド読み込み
                mainBGM = Resources.Load("Sounds/gameMainBGM", typeof(AudioClip)) as AudioClip;

                //バトルモード用の時間に設定
                cV.remainigTime = cV.BATTLE_TIME;

                //---------------=2Pの車を召喚=---------------//
                switch (cV.carType2P)
                {
                    //EK9の召喚
                    case CommonVariable.CarType2P.Civic:
                        //位置決め
                        player2CarObj = Instantiate((GameObject)Resources.Load("Prefabs/cars/EK9"), new Vector3(1548.26f, -0.05531552f, -5326.751f), Quaternion.Euler(0.375f, 123.508f, -4.245f));
                        break;

                    //ランエボの召喚
                    case CommonVariable.CarType2P.Evo:
                        //位置決め
                        player2CarObj = Instantiate((GameObject)Resources.Load("Prefabs/cars/Evo6"), new Vector3(1548.26f, -0.05531552f, -5326.751f), Quaternion.Euler(0.375f, 123.508f, -4.245f));
                        break;

                    //セリカの召喚
                    case CommonVariable.CarType2P.Celica:
                        //位置決め
                        player2CarObj = Instantiate((GameObject)Resources.Load("Prefabs/cars/Celica"), new Vector3(1548.26f, -0.05531552f, -5326.751f), Quaternion.Euler(0.375f, 123.508f, -4.245f));
                        break;

                    //スカイラインの召喚
                    case CommonVariable.CarType2P.Skyline:
                        //位置決め
                        player2CarObj = Instantiate((GameObject)Resources.Load("Prefabs/cars/R33"), new Vector3(1548.26f, -0.05531552f, -5326.751f), Quaternion.Euler(0.375f, 123.508f, -4.245f));
                        break;
                }

                //カーオブジェクトの子オブジェクト（ゴールチェック）を取得
                cPScript2p = player2CarObj.transform.Find("GeneralPurposeColl").gameObject.GetComponent<checkPointScript>();
                //カースクリプト取得
                carScript2p = player2CarObj.GetComponent<carScript>();

                ccVBattle = Instantiate((GameObject)Resources.Load("Prefabs/CommonCanvasBattle"));
                rU = ccVBattle.GetComponent<rankUI>();

                Instantiate((GameObject)Resources.Load("Prefabs/BattleModeCamera"));

                carScript1p.myNumber = 1;
                carScript2p.myNumber = 2;

                initUI();

                break;

            //タイムアタックモード
            case CommonVariable.GameMode.TIMEATTACK:

                //サウンド読み込み
                mainBGM = Resources.Load("Sounds/gameMainBGM2", typeof(AudioClip)) as AudioClip;

                //タイムアタック用のUI作成
                rT = Instantiate((GameObject)Resources.Load("Prefabs/CommonCanvasTime")).GetComponent<resultTime>();

                //タイムアタック用の時間に設定
                cV.remainigTime = cV.TIMEATTACK_TIME;
                break;
        }

        carStatus = Status.MUSIC;
        LButtonFlg1P = RButtonFlg1P = false;

        GameOverTime1P =
             GameOverTime2P =
        totalTime = 0;//トータルタイム初期化
    }

    /* ======================================================================= *
     * フィックストアプデ
     * 時間に関するもの
     * ======================================================================= */
    private void FixedUpdate()
    {
        switch (carStatus)
        {
            case Status.MUSIC:
                sM.BGMPlay(mainBGM, true, 0.5f);
                carStatus = Status.START;

                break;
            case Status.START://ゲーム開始時の動作

                //カウントダウンが終わればプレイに移る
                if (cdS.CountDown()) carStatus = Status.PLAY;

                break;

            case Status.PLAY://ゲーム中の動作

                break;

            case Status.GOAL://ゲーム終了時の動作

                //ゲームモード別
                switch (cV.gameMode)
                {
                    case CommonVariable.GameMode.TIMEATTACK:

                        if (rT.ResultTimeAnime(true)) { SceneManager.LoadScene("GameHomeScene"); }

                        break;
                    case CommonVariable.GameMode.BATTLE:

                        if (rU.RankUIAnime()) { SceneManager.LoadScene("GameHomeScene"); }

                        break;
                }
                break;

            case Status.GAMEOVER://ゲームオーバー時の動作

                //ゲームモード別
                switch (cV.gameMode)
                {
                    case CommonVariable.GameMode.TIMEATTACK:

                        if (rT.ResultTimeAnime(false)) { SceneManager.LoadScene("GameHomeScene"); }

                        break;
                    case CommonVariable.GameMode.BATTLE:

                        if (rU.RankUIAnime()) { SceneManager.LoadScene("GameHomeScene"); }

                        break;
                }
                break;
        }
    }

    /* ======================================================================= *
     * アプデ
     * コントローラー関連
     * ======================================================================= */
    private void Update()
    {
        //例外ゲームオーバー
        //強制ゲームオーバー
        if (Input.GetKeyDown("backspace")) carStatus = Status.GAMEOVER;

        //1P車
        //コースに外れる
        if (player1CarObj.transform.localPosition.y < -200) carStatus = Status.GAMEOVER;
        //車両横転
        if (carScript1p.nowSpeed < 1.5f)
        {
            if (GameOverTime1P > 10)
            {
                if (carStatus != Status.GAMEOVER)
                {
                    carStatus = Status.GAMEOVER;
                    sM.BGMPlay(goalBGM, true, 0.5f);
                }
            }
            else
            {
                GameOverTime1P += Time.deltaTime;
            }
        }
        else
        {
            GameOverTime1P = 0;
        }

        //2P車
        if (cV.gameMode == CommonVariable.GameMode.BATTLE)
        {
            //コースに外れる
            if (player2CarObj.transform.localPosition.y < -200) carStatus = Status.GAMEOVER;
            //車両横転
            if (carScript2p.nowSpeed < 1.5f)
            {
                if (GameOverTime2P > 5)
                {
                    if (carStatus != Status.GAMEOVER)
                    {
                        carStatus = Status.GAMEOVER;
                        sM.BGMPlay(goalBGM, true, 0.5f);
                    }
                }
                else
                {
                    GameOverTime2P += Time.deltaTime;
                }
            }
            else
            {
                GameOverTime2P = 0;
            }
        }

        switch (carStatus)
        {
            case Status.START://ゲーム開始時の動作

                //1P
                //自動運転
                //スタート時にはギアの操作だけできる
                carScript1p.Car_Accel(AutoMove(1, carScript1p.nowSpeed));
                ShiftGear1P();
                Camera1P();

                //2P
                if (cV.gameMode == CommonVariable.GameMode.BATTLE)
                {
                    //自動運転
                    carScript2p.Car_Accel(AutoMove(1, carScript1p.nowSpeed));
                    ShiftGear2P();
                }
                break;

            case Status.PLAY://ゲーム中の動作

                //タイムカウント
                countTotalTime();

                //ゲーム中はすべての操作をできるようにする
                //1P
                ShiftGear1P();
                Controll1P();
                Camera1P();

                //2P
                if (cV.gameMode == CommonVariable.GameMode.BATTLE)
                {
                    ShiftGear2P();
                    Controll2P();

                    if (carScript1p.wwDC.newPassesNum > carScript2p.wwDC.newPassesNum)
                    {
                        cV.Rank1P = 1;
                        cV.Rank2P = 2;
                    }
                    else
                    {
                        cV.Rank1P = 2;
                        cV.Rank2P = 1;
                    }
                }

                //最大速度更新
                if (carScript1p.nowSpeed > maxSpeed) maxSpeed = carScript1p.nowSpeed;

                //ゴール、ゲームオーバー時の動作
                if (cV.remainigTime <= 0)
                {
                    carStatus = Status.GAMEOVER;
                    sM.BGMPlay(goalBGM, true, 0.5f);
                }
                if (cPScript1p.goalJudge)
                {
                    carStatus = Status.GOAL;
                    sM.BGMPlay(goalBGM, true, 0.5f);
                }
                if (cPScript2p.goalJudge)
                {
                    carStatus = Status.GOAL;
                    sM.BGMPlay(goalBGM, true, 0.5f);
                }
                break;

            case Status.GOAL://ゲーム終了時の動作
            case Status.GAMEOVER://ゲームオーバー時の動作

                //自動ブレーキ
                //1P
                carScript1p.Car_Accel(0.0f);
                carScript1p.Car_Brake(6);
                //2P
                if (cV.gameMode == CommonVariable.GameMode.BATTLE)
                {
                    carScript2p.Car_Accel(0.0f);
                    carScript2p.Car_Brake(6);
                }
                break;

        }
    }

    /* ======================================================================= *
     * 分計算
     * ======================================================================= */
    public int CalMinute(float getTime) { return (int)(getTime / 60); }

    /* ======================================================================= *
     * 秒計算
     * ======================================================================= */
    public float CalSecond(float getTime) { return getTime % 60; }

    /* ======================================================================= *
     * トータルタイムカウント
     * 残り時間カウント
     * ======================================================================= */
    void countTotalTime()
    {
        //カウントアップ
        totalTime += Time.deltaTime;

        //カウントダウン
        cV.remainigTime -= Time.deltaTime;

        //分ミリ秒の計算
        totalTimes[0, 0] = CalMinute(totalTime);

        float second = CalSecond(totalTime);

        totalTimes[0, 1] = (int)second;
        totalTimes[0, 2] = (int)(1000 * (second - (int)second));
    }

    /* ======================================================================= *
     * カウントダウン時、自動で動く
     * ======================================================================= */
    private float AutoMove(float autoAccel, float autoSpeed)
    {
        if (autoSpeed <= 120) return autoAccel;
        else return 0.0f;
    }

    /* ======================================================================= *
     * アクセル、ブレーキ、ステアリング操作 1P用
     * ======================================================================= */
    void Controll1P()
    {
        /*--------------------トリガー--------------------*/
        if (Input.GetAxis("Trigger1") == 0)
        {
            // アクセルもブレーキも使っていない
            carScript1p.Car_Accel(0.0f);
            carScript1p.Car_Brake(0.0f);
        }
        //アクセル - 1P
        else if (Input.GetAxis("Trigger1") < 0)
        {
            //ブレーキを使わない
            carScript1p.Car_Brake(0.0f);

            // コントローラーのトリガーの数字が中途半端なので0.99以上で強制的に1にする。
            if (Input.GetAxis("Trigger1") < -0.99)
            {
                carScript1p.Car_Accel(1.0f);
            }
            else
            {
                carScript1p.Car_Accel(Input.GetAxis("Trigger1") * -1);
            }
        }
        // ブレーキ - 1P
        else
        {
            //アクセルを使わない
            carScript1p.Car_Accel(0.0f);

            // コントローラーのトリガーの数字が中途半端なので-0.99以上で強制的に-1にする。
            if (Input.GetAxis("Trigger1") > 0.99)
            {
                carScript1p.Car_Brake(1.0f);
            }
            else
            {
                carScript1p.Car_Brake(Input.GetAxis("Trigger1"));
            }
        }

        /*--------------左アナログスティック--------------*/
        //----------------------
        //---ハンドリング操作---
        //----------------------
        //左右 - 1P
        if (Input.GetAxis("Handle1") != 0)
        {
            carScript1p.Car_Handle(Input.GetAxis("Handle1"));
        }
        //真ん中 - 1P
        else
        {
            carScript1p.Car_Handle(0);
        }
    }

    /* ======================================================================= *
     * ギア操作 1P用
     * ======================================================================= */
    void ShiftGear1P()
    {
        /*--------------LRボタン--------------*/
        //シフトアップ - 1P
        if (Input.GetAxis("RButton1") == 1 && !RButtonFlg1P)
        {
            carScript1p.Car_Shift((int)GearEnum.GearUp);
            RButtonFlg1P = true;
        }
        else if (Input.GetAxis("RButton1") == 0)
        {
            RButtonFlg1P = false;
        }

        //シフトダウン - 1P
        if (Input.GetAxis("LButton1") == 1 && !LButtonFlg1P)
        {
            carScript1p.Car_Shift((int)GearEnum.GearDown);
            LButtonFlg1P = true;
        }
        else if (Input.GetAxis("LButton1") == 0)
        {
            LButtonFlg1P = false;
        }
    }

    /* ======================================================================= *
     * カメラ視点操作操作 1P用
     * ======================================================================= */
    void Camera1P()
    {
        /*--------------LRボタン--------------*/
        //シフトアップ - 1P
        if (Input.GetAxis("AButton1") == 1 && !AButtonFlg1P)
        {
            carScript1p.CameraPosition();
            AButtonFlg1P = true;
        }
        else if (Input.GetAxis("AButton1") == 0)
        {
            AButtonFlg1P = false;
        }
    }

    /* ======================================================================= *
     * アクセル、ブレーキ、ステアリング操作 2P用
     * ======================================================================= */
    void Controll2P()
    {
        /*--------------------トリガー--------------------*/
        if (Input.GetAxis("Trigger2") == 0)
        {
            // アクセルもブレーキも使っていない
            carScript2p.Car_Accel(0.0f);
            carScript2p.Car_Brake(0.0f);
        }
        //アクセル - 2P
        else if (Input.GetAxis("Trigger2") < 0)
        {
            //ブレーキを使わない
            carScript2p.Car_Brake(0.0f);

            // コントローラーのトリガーの数字が中途半端なので0.99以上で強制的に1にする。
            if (Input.GetAxis("Trigger2") < -0.99)
            {
                carScript2p.Car_Accel(1.0f);
            }
            else
            {
                carScript2p.Car_Accel(Input.GetAxis("Trigger2") * -1);
            }
        }
        // ブレーキ - 2P
        else
        {
            //アクセルを使わない
            carScript2p.Car_Accel(0.0f);

            // コントローラーのトリガーの数字が中途半端なので-0.99以上で強制的に-1にする。
            if (Input.GetAxis("Trigger2") > 0.99)
            {
                carScript2p.Car_Brake(1.0f);
            }
            else
            {
                carScript2p.Car_Brake(Input.GetAxis("Trigger2"));
            }
        }

        /*--------------左アナログスティック--------------*/
        //----------------------
        //---ハンドリング操作---
        //----------------------
        //左右 - 2P
        if (Input.GetAxis("Handle2") != 0)
        {
            carScript2p.Car_Handle(Input.GetAxis("Handle2"));
        }
        //真ん中 - 2P
        else
        {
            carScript2p.Car_Handle(0);
        }
    }

    /* ======================================================================= *
     * ギア操作 2P用
     * ======================================================================= */
    void ShiftGear2P()
    {
        /*--------------LRボタン--------------*/
        //シフトアップ - 2P
        if (Input.GetAxis("RButton2") == 1 && !RButtonFlg2P)
        {
            carScript2p.Car_Shift((int)GearEnum.GearUp);
            RButtonFlg2P = true;
        }
        else if (Input.GetAxis("RButton2") == 0)
        {
            RButtonFlg2P = false;
        }

        //シフトダウン - 2P
        if (Input.GetAxis("LButton2") == 1 && !LButtonFlg2P)
        {
            carScript2p.Car_Shift((int)GearEnum.GearDown);
            LButtonFlg2P = true;
        }
        else if (Input.GetAxis("LButton2") == 0)
        {
            LButtonFlg2P = false;
        }
    }

    private void initUI()
    {
        //1PUI等設定
        player1CarObj.transform.Find("Camera").GetComponent<Camera>().rect = new Rect(-0.5f, 0.25f, 1, 0.5f);
        Vector3 setV3Pos = player1CarObj.transform.Find("myCanvas/SpeedMeterImage").transform.localPosition;
        Vector3 setV3Sca = player1CarObj.transform.Find("myCanvas/SpeedMeterImage").transform.localScale;
        player1CarObj.transform.Find("myCanvas/SpeedMeterImage").transform.localPosition = new Vector3(setV3Pos.x - ((960 - setV3Pos.x) / 2 + setV3Pos.x), setV3Pos.y / 2, setV3Pos.z);
        player1CarObj.transform.Find("myCanvas/SpeedMeterImage").transform.localScale = new Vector3(setV3Sca.x / 2, setV3Sca.y / 2, setV3Sca.z);

        setV3Pos = player1CarObj.transform.Find("myCanvas/SpeedNeedleImage").transform.localPosition;
        setV3Sca = player1CarObj.transform.Find("myCanvas/SpeedNeedleImage").transform.localScale;
        player1CarObj.transform.Find("myCanvas/SpeedNeedleImage").transform.localPosition = new Vector3(setV3Pos.x - ((960 - setV3Pos.x) / 2 + setV3Pos.x), setV3Pos.y / 2, setV3Pos.z);
        player1CarObj.transform.Find("myCanvas/SpeedNeedleImage").transform.localScale = new Vector3(setV3Sca.x / 2, setV3Sca.y / 2, setV3Sca.z);

        setV3Pos = player1CarObj.transform.Find("myCanvas/RPMMeterImage").transform.localPosition;
        setV3Sca = player1CarObj.transform.Find("myCanvas/RPMMeterImage").transform.localScale;
        player1CarObj.transform.Find("myCanvas/RPMMeterImage").transform.localPosition = new Vector3(setV3Pos.x - ((960 - setV3Pos.x) / 2 + setV3Pos.x), setV3Pos.y / 2, setV3Pos.z);
        player1CarObj.transform.Find("myCanvas/RPMMeterImage").transform.localScale = new Vector3(setV3Sca.x / 2, setV3Sca.y / 2, setV3Sca.z);

        setV3Pos = player1CarObj.transform.Find("myCanvas/RPMNeedleImage").transform.localPosition;
        setV3Sca = player1CarObj.transform.Find("myCanvas/RPMNeedleImage").transform.localScale;
        player1CarObj.transform.Find("myCanvas/RPMNeedleImage").transform.localPosition = new Vector3(setV3Pos.x - ((960 - setV3Pos.x) / 2 + setV3Pos.x), setV3Pos.y / 2, setV3Pos.z);
        player1CarObj.transform.Find("myCanvas/RPMNeedleImage").transform.localScale = new Vector3(setV3Sca.x / 2, setV3Sca.y / 2, setV3Sca.z);

        setV3Pos = player1CarObj.transform.Find("myCanvas/speedDigiMeterImage").transform.localPosition;
        setV3Sca = player1CarObj.transform.Find("myCanvas/speedDigiMeterImage").transform.localScale;
        player1CarObj.transform.Find("myCanvas/speedDigiMeterImage").transform.localPosition = new Vector3(setV3Pos.x - ((960 - setV3Pos.x) / 2 + setV3Pos.x), setV3Pos.y / 2, setV3Pos.z);
        player1CarObj.transform.Find("myCanvas/speedDigiMeterImage").transform.localScale = new Vector3(setV3Sca.x / 2, setV3Sca.y / 2, setV3Sca.z);

        setV3Pos = player1CarObj.transform.Find("myCanvas/GearMTImage").transform.localPosition;
        setV3Sca = player1CarObj.transform.Find("myCanvas/GearMTImage").transform.localScale;
        player1CarObj.transform.Find("myCanvas/GearMTImage").transform.localPosition = new Vector3(setV3Pos.x - ((960 - setV3Pos.x) / 2 + setV3Pos.x), setV3Pos.y / 2, setV3Pos.z);
        player1CarObj.transform.Find("myCanvas/GearMTImage").transform.localScale = new Vector3(setV3Sca.x / 2, setV3Sca.y / 2, setV3Sca.z);

        setV3Pos = player1CarObj.transform.Find("myCanvas/MTText").transform.localPosition;
        setV3Sca = player1CarObj.transform.Find("myCanvas/MTText").transform.localScale;
        player1CarObj.transform.Find("myCanvas/MTText").transform.localPosition = new Vector3(setV3Pos.x - ((960 - setV3Pos.x) / 2 + setV3Pos.x), setV3Pos.y / 2, setV3Pos.z);
        player1CarObj.transform.Find("myCanvas/MTText").transform.localScale = new Vector3(setV3Sca.x / 2, setV3Sca.y / 2, setV3Sca.z);

        setV3Pos = player1CarObj.transform.Find("myCanvas/TotalTimeImg").transform.localPosition;
        setV3Sca = player1CarObj.transform.Find("myCanvas/TotalTimeImg").transform.localScale;
        player1CarObj.transform.Find("myCanvas/TotalTimeImg").transform.localPosition = new Vector3(setV3Pos.x - ((960 - setV3Pos.x) / 2 + setV3Pos.x), setV3Pos.y / 2, setV3Pos.z);
        player1CarObj.transform.Find("myCanvas/TotalTimeImg").transform.localScale = new Vector3(setV3Sca.x / 2, setV3Sca.y / 2, setV3Sca.z);

        setV3Pos = player1CarObj.transform.Find("myCanvas/sign").transform.localPosition;
        setV3Sca = player1CarObj.transform.Find("myCanvas/sign").transform.localScale;
        player1CarObj.transform.Find("myCanvas/sign").transform.localPosition = new Vector3(setV3Pos.x - ((960 - setV3Pos.x) / 2 + setV3Pos.x), setV3Pos.y / 2, setV3Pos.z);
        player1CarObj.transform.Find("myCanvas/sign").transform.localScale = new Vector3(setV3Sca.x / 2, setV3Sca.y / 2, setV3Sca.z);

        setV3Pos = player1CarObj.transform.Find("myCanvas/TimeUIText").transform.localPosition;
        setV3Sca = player1CarObj.transform.Find("myCanvas/TimeUIText").transform.localScale;
        player1CarObj.transform.Find("myCanvas/TimeUIText").transform.localPosition = new Vector3(setV3Pos.x - ((960 - setV3Pos.x) / 2 + setV3Pos.x), setV3Pos.y / 2, setV3Pos.z);
        player1CarObj.transform.Find("myCanvas/TimeUIText").transform.localScale = new Vector3(setV3Sca.x / 2, setV3Sca.y / 2, setV3Sca.z);

        setV3Pos = player1CarObj.transform.Find("myCanvas/Gyakusou").transform.localPosition;
        setV3Sca = player1CarObj.transform.Find("myCanvas/Gyakusou").transform.localScale;
        player1CarObj.transform.Find("myCanvas/Gyakusou").transform.localPosition = new Vector3(setV3Pos.x - ((960 - setV3Pos.x) / 2 + setV3Pos.x), setV3Pos.y / 2, setV3Pos.z);
        player1CarObj.transform.Find("myCanvas/Gyakusou").transform.localScale = new Vector3(setV3Sca.x / 2, setV3Sca.y / 2, setV3Sca.z);

        setV3Pos = player1CarObj.transform.Find("myCanvas/RevRamp").transform.localPosition;
        setV3Sca = player1CarObj.transform.Find("myCanvas/RevRamp").transform.localScale;
        player1CarObj.transform.Find("myCanvas/RevRamp").transform.localPosition = new Vector3(setV3Pos.x - ((960 - setV3Pos.x) / 2 + setV3Pos.x), setV3Pos.y / 2, setV3Pos.z);
        player1CarObj.transform.Find("myCanvas/RevRamp").transform.localScale = new Vector3(setV3Sca.x / 2, setV3Sca.y / 2, setV3Sca.z);

        //2PUI等設定
        player2CarObj.transform.Find("Camera").GetComponent<Camera>().rect = new Rect(0.5f, 0.25f, 1, 0.5f);

        setV3Pos = player2CarObj.transform.Find("myCanvas/SpeedMeterImage").transform.localPosition;
        setV3Sca = player2CarObj.transform.Find("myCanvas/SpeedMeterImage").transform.localScale;
        player2CarObj.transform.Find("myCanvas/SpeedMeterImage").transform.localPosition = new Vector3(960 + setV3Pos.x - ((960 - setV3Pos.x) / 2 + setV3Pos.x), setV3Pos.y / 2, setV3Pos.z);
        player2CarObj.transform.Find("myCanvas/SpeedMeterImage").transform.localScale = new Vector3(setV3Sca.x / 2, setV3Sca.y / 2, setV3Sca.z);

        setV3Pos = player2CarObj.transform.Find("myCanvas/SpeedNeedleImage").transform.localPosition;
        setV3Sca = player2CarObj.transform.Find("myCanvas/SpeedNeedleImage").transform.localScale;
        player2CarObj.transform.Find("myCanvas/SpeedNeedleImage").transform.localPosition = new Vector3(960 + setV3Pos.x - ((960 - setV3Pos.x) / 2 + setV3Pos.x), setV3Pos.y / 2, setV3Pos.z);
        player2CarObj.transform.Find("myCanvas/SpeedNeedleImage").transform.localScale = new Vector3(setV3Sca.x / 2, setV3Sca.y / 2, setV3Sca.z);

        setV3Pos = player2CarObj.transform.Find("myCanvas/RPMMeterImage").transform.localPosition;
        setV3Sca = player2CarObj.transform.Find("myCanvas/RPMMeterImage").transform.localScale;
        player2CarObj.transform.Find("myCanvas/RPMMeterImage").transform.localPosition = new Vector3(960 + setV3Pos.x - ((960 - setV3Pos.x) / 2 + setV3Pos.x), setV3Pos.y / 2, setV3Pos.z);
        player2CarObj.transform.Find("myCanvas/RPMMeterImage").transform.localScale = new Vector3(setV3Sca.x / 2, setV3Sca.y / 2, setV3Sca.z);

        setV3Pos = player2CarObj.transform.Find("myCanvas/RPMNeedleImage").transform.localPosition;
        setV3Sca = player2CarObj.transform.Find("myCanvas/RPMNeedleImage").transform.localScale;
        player2CarObj.transform.Find("myCanvas/RPMNeedleImage").transform.localPosition = new Vector3(960 + setV3Pos.x - ((960 - setV3Pos.x) / 2 + setV3Pos.x), setV3Pos.y / 2, setV3Pos.z);
        player2CarObj.transform.Find("myCanvas/RPMNeedleImage").transform.localScale = new Vector3(setV3Sca.x / 2, setV3Sca.y / 2, setV3Sca.z);

        setV3Pos = player2CarObj.transform.Find("myCanvas/speedDigiMeterImage").transform.localPosition;
        setV3Sca = player2CarObj.transform.Find("myCanvas/speedDigiMeterImage").transform.localScale;
        player2CarObj.transform.Find("myCanvas/speedDigiMeterImage").transform.localPosition = new Vector3(960 + setV3Pos.x - ((960 - setV3Pos.x) / 2 + setV3Pos.x), setV3Pos.y / 2, setV3Pos.z);
        player2CarObj.transform.Find("myCanvas/speedDigiMeterImage").transform.localScale = new Vector3(setV3Sca.x / 2, setV3Sca.y / 2, setV3Sca.z);

        setV3Pos = player2CarObj.transform.Find("myCanvas/GearMTImage").transform.localPosition;
        setV3Sca = player2CarObj.transform.Find("myCanvas/GearMTImage").transform.localScale;
        player2CarObj.transform.Find("myCanvas/GearMTImage").transform.localPosition = new Vector3(960 + setV3Pos.x - ((960 - setV3Pos.x) / 2 + setV3Pos.x), setV3Pos.y / 2, setV3Pos.z);
        player2CarObj.transform.Find("myCanvas/GearMTImage").transform.localScale = new Vector3(setV3Sca.x / 2, setV3Sca.y / 2, setV3Sca.z);

        setV3Pos = player2CarObj.transform.Find("myCanvas/MTText").transform.localPosition;
        setV3Sca = player2CarObj.transform.Find("myCanvas/MTText").transform.localScale;
        player2CarObj.transform.Find("myCanvas/MTText").transform.localPosition = new Vector3(960 + setV3Pos.x - ((960 - setV3Pos.x) / 2 + setV3Pos.x), setV3Pos.y / 2, setV3Pos.z);
        player2CarObj.transform.Find("myCanvas/MTText").transform.localScale = new Vector3(setV3Sca.x / 2, setV3Sca.y / 2, setV3Sca.z);

        setV3Pos = player2CarObj.transform.Find("myCanvas/TotalTimeImg").transform.localPosition;
        setV3Sca = player2CarObj.transform.Find("myCanvas/TotalTimeImg").transform.localScale;
        player2CarObj.transform.Find("myCanvas/TotalTimeImg").transform.localPosition = new Vector3(960 + setV3Pos.x - ((960 - setV3Pos.x) / 2 + setV3Pos.x), setV3Pos.y / 2, setV3Pos.z);
        player2CarObj.transform.Find("myCanvas/TotalTimeImg").transform.localScale = new Vector3(setV3Sca.x / 2, setV3Sca.y / 2, setV3Sca.z);

        setV3Pos = player2CarObj.transform.Find("myCanvas/sign").transform.localPosition;
        setV3Sca = player2CarObj.transform.Find("myCanvas/sign").transform.localScale;
        player2CarObj.transform.Find("myCanvas/sign").transform.localPosition = new Vector3(960 + setV3Pos.x - ((960 - setV3Pos.x) / 2 + setV3Pos.x), setV3Pos.y / 2, setV3Pos.z);
        player2CarObj.transform.Find("myCanvas/sign").transform.localScale = new Vector3(setV3Sca.x / 2, setV3Sca.y / 2, setV3Sca.z);

        setV3Pos = player2CarObj.transform.Find("myCanvas/TimeUIText").transform.localPosition;
        setV3Sca = player2CarObj.transform.Find("myCanvas/TimeUIText").transform.localScale;
        player2CarObj.transform.Find("myCanvas/TimeUIText").transform.localPosition = new Vector3(960 + setV3Pos.x - ((960 - setV3Pos.x) / 2 + setV3Pos.x), setV3Pos.y / 2, setV3Pos.z);
        player2CarObj.transform.Find("myCanvas/TimeUIText").transform.localScale = new Vector3(setV3Sca.x / 2, setV3Sca.y / 2, setV3Sca.z);

        setV3Pos = player2CarObj.transform.Find("myCanvas/Gyakusou").transform.localPosition;
        setV3Sca = player2CarObj.transform.Find("myCanvas/Gyakusou").transform.localScale;
        player2CarObj.transform.Find("myCanvas/Gyakusou").transform.localPosition = new Vector3(960 + setV3Pos.x - ((960 - setV3Pos.x) / 2 + setV3Pos.x), setV3Pos.y / 2, setV3Pos.z);
        player2CarObj.transform.Find("myCanvas/Gyakusou").transform.localScale = new Vector3(setV3Sca.x / 2, setV3Sca.y / 2, setV3Sca.z);

        setV3Pos = player2CarObj.transform.Find("myCanvas/RevRamp").transform.localPosition;
        setV3Sca = player2CarObj.transform.Find("myCanvas/RevRamp").transform.localScale;
        player2CarObj.transform.Find("myCanvas/RevRamp").transform.localPosition = new Vector3(960 + setV3Pos.x - ((960 - setV3Pos.x) / 2 + setV3Pos.x), setV3Pos.y / 2, setV3Pos.z);
        player2CarObj.transform.Find("myCanvas/RevRamp").transform.localScale = new Vector3(setV3Sca.x / 2, setV3Sca.y / 2, setV3Sca.z);

    }
}