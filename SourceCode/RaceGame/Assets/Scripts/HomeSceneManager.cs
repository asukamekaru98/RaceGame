using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HomeSceneManager : MonoBehaviour
{
    /*---------------スクリプト---------------*/
    private CommonVariable cV;//共通変数
    private SoundManager sM;//サウンド

    /*-----------------------------------サウンド-----------------------------------*/
    private AudioClip homeBGM;
    private AudioClip pushSE;

    /*---------------ゲームオブジェクト---------------*/
    private GameObject CanvasSelectMode, CanvasSelectCarMode;//キャンバス

    private GameObject TimeAttackModeWakuImg, BattleModeWakuImg;//モードアイコン
    private GameObject ModeWakuRed;//モードアイコン赤枠

    private GameObject WakuAka, WakuAkaHalf;//赤枠
    private GameObject WakuAo, WakuAoHalf;//青枠

    private GameObject Upper, Footer;//アッパーフッター

    private GameObject[] Car1PMode = new GameObject[4];
    private GameObject[] Car2PMode = new GameObject[4];
    GameObject Car1Model, Car2Model;

    /*---------------列挙型---------------*/
    //どのセレクトか
    private enum Status
    {
        START_BGM, START_SLIDE, SELECT_MODE, HIDE_SLIDE, DISP_SLIDE, SELECT_CAR, END_SLIDE
    }
    private Status homeStatus;

    //どのモードか
    private enum Mode
    {
        GAMEMODE_ATTACK, GAMEMODE_BATTLE
    }
    private Mode isMode;

    //どの車か
    private enum Car
    {
        CAR_CIVIC, CAR_CELICA, CAR_EVO6, CAR_R33
    }
    private Car isCar1P, isCar2P;

    /*---------------定数---------------*/
    //点滅関連
    const float ALPHA_MAX = 1.0f;// アルファ値の最大
    const float ALPHA_MIN = 0.2f;// アルファ値の最小
    const float WAKU_FLASHING_SPEED = 0.03f;// 文字の点滅速度

    //アッパーフッター関連
    const float UPPER_DISP = 475;//スライド移動速度
    const float UPPER_HIDE = 610;//スライド移動速度
    const float FOOTER_DISP = -460;//スライド移動速度
    const float FOOTER_HIDE = -590;//スライド移動速度

    //セレクトモード関連
    const float MODE_WAKU_SPEED = 0.2f;//セレクトモード枠の速さ
    const float MODE_WAKU_MAX = 3.5f;//セレクトモード枠の最大スケール
    const float MODE_WAKU_MIN = 1;//セレクトモード枠の最小スケール

    /*---------------変数---------------*/
    //コントローラー関連
    bool onPushStick1P;//スティックフラグ1P
    bool onPushTrigger1P;//トリガーフラグ1P
    bool onPushStick2P;//スティックフラグ2P
    bool onPushTrigger2P;//トリガーフラグ2P

    //点滅関連
    private float textAlpha = 1;// テキスト点滅のための数字
    private bool flashingFlg;// 点滅で濃ゆくなるか薄くなるかのフラグ

    //セレクトモード関連
    private float ModeWakuScale;

    private bool isStart;
    private bool Select1P;//1P選択終了フラグ
    private bool Select2P;//2P選択終了フラグ

    /* ======================================================================= *
     * 初期化
     * ======================================================================= */
    void Start()
    {
        //ステータスの初期化
        homeStatus = Status.START_BGM;
        //モードの初期化
        isMode = Mode.GAMEMODE_ATTACK;
        //モードの初期化
        isCar1P = Car.CAR_CIVIC;
        isCar2P = Car.CAR_R33;

        //***********************スクリプト読み込み***********************
        cV = GameObject.Find("CommonVariable").GetComponent<CommonVariable>();
        sM = GameObject.Find("SoundManager").GetComponent<SoundManager>();

        //***********************サウンド読み込み***********************
        homeBGM = Resources.Load("Sounds/homeBGM", typeof(AudioClip)) as AudioClip;//ポーン
        pushSE = Resources.Load("Sounds/homese", typeof(AudioClip)) as AudioClip;//ピン

        //***********************オブジェクト読み込み***********************
        //キャンバス読み込み
        CanvasSelectMode = GameObject.Find("CanvasSelectMode");
        CanvasSelectCarMode = GameObject.Find("CanvasSelectCarMode");

        //セレクトモード関連読み込み
        TimeAttackModeWakuImg = GameObject.Find("CanvasSelectMode/TimeAttackModeImg/TimeAttackModeWakuImg");
        BattleModeWakuImg = GameObject.Find("CanvasSelectMode/BattleModeImg/BattleModeWakuImg");
        ModeWakuRed = GameObject.Find("CanvasSelectMode/ModeWakuAka");

        //枠読み込み
        WakuAka = GameObject.Find("CanvasSelectCarMode/WakuAka");
        WakuAkaHalf = GameObject.Find("CanvasSelectCarMode/WakuAka_Hulf");
        WakuAo = GameObject.Find("CanvasSelectCarMode/WakuAo");
        WakuAoHalf = GameObject.Find("CanvasSelectCarMode/WakuAo_Hulf");

        //アッパーフッター読み込み
        Upper = GameObject.Find("CanvasCommon/UIUpper");
        Footer = GameObject.Find("CanvasCommon/UIFooter");

        //車ゲームオブジェクト
        Car1PMode[0] = GameObject.Find("Cars1P/SelectEK9");
        Car1PMode[1] = GameObject.Find("Cars1P/SelectCelica");
        Car1PMode[2] = GameObject.Find("Cars1P/SelectEvo6");
        Car1PMode[3] = GameObject.Find("Cars1P/SelectR33");
        Car2PMode[0] = GameObject.Find("Cars2P/SelectEK9");
        Car2PMode[1] = GameObject.Find("Cars2P/SelectCelica");
        Car2PMode[2] = GameObject.Find("Cars2P/SelectEvo6");
        Car2PMode[3] = GameObject.Find("Cars2P/SelectR33");
        Car1Model = GameObject.Find("Cars1P");
        Car2Model = GameObject.Find("Cars2P");

        //***********************オブジェクトの表示非表示***********************
        //キャンバスの表示非表示
        CanvasSelectMode.SetActive(false);
        CanvasSelectCarMode.SetActive(false);
        //枠の表示非表示
        WakuAka.SetActive(false);
        WakuAkaHalf.SetActive(false);
        WakuAo.SetActive(false);
        WakuAoHalf.SetActive(false);
        //オブジェクトの表示非表示
        Car1Model.SetActive(false);
        Car2Model.SetActive(false);

        for (int i = 0; i < 4; i++)
        {
            Car1PMode[i].SetActive(false);
            Car2PMode[i].SetActive(false);
        }

        //***********************変数初期化***********************
        //コントローラ関連
        onPushStick1P = true;//スティックフラグ初期化
        onPushTrigger1P = true;//トリガーフラグ初期化

        //セレクトモード関連
        ModeWakuScale = MODE_WAKU_MIN;//セレクトモード枠のスケール初期化

        //セレクトモード関連
        Select1P = false;//1P選択終了フラグ
        Select2P = false;//2P選択終了フラグ
    }

    /* ======================================================================= *
     * フィックストアプデ
     * アニメーション関連
     * ======================================================================= */
    private void FixedUpdate()
    {
        switch (homeStatus)
        {

            case Status.START_SLIDE:

                if (Upper.transform.localPosition.y != UPPER_DISP && Footer.transform.localPosition.y != FOOTER_DISP)
                {
                    //アッパーフッターの出現
                    Upper.transform.localPosition = new Vector2(0, UFSlideAnime(Upper.transform.localPosition.y, UPPER_DISP, 7));
                    Footer.transform.localPosition = new Vector2(0, UFSlideAnime(Footer.transform.localPosition.y, FOOTER_DISP, 7));
                }
                else
                {
                    //モード枠初期化
                    ModeWakuRed.SetActive(true);
                    ModeWakuScale = MODE_WAKU_MIN;//セレクトモード枠のスケール初期化
                    ModeWakuRed.transform.localScale = new Vector2(ModeWakuScale, ModeWakuScale);

                    //セレクトモードキャンバス表示
                    CanvasSelectMode.SetActive(true);
                    //セレクトモード移行
                    homeStatus = Status.SELECT_MODE;
                }
                break;

            case Status.SELECT_MODE:

                //セレクトモードアニメーション
                SelectModeAnime();

                break;

            case Status.HIDE_SLIDE:

                //赤い丸枠が大きくなる
                if (ModeWakuScale < MODE_WAKU_MAX)
                {
                    ModeWakuScale += MODE_WAKU_SPEED;

                    if (ModeWakuScale >= MODE_WAKU_MAX)
                    {
                        //モード枠非表示化
                        ModeWakuRed.SetActive(false);
                        //セレクトモードキャンバス非表示
                        CanvasSelectMode.SetActive(false);
                        //セレクトカーモードキャンバス表示
                        CanvasSelectCarMode.SetActive(false);
                    }

                    ModeWakuRed.transform.localScale = new Vector2(ModeWakuScale, ModeWakuScale);
                }
                else if (Upper.transform.localPosition.y != UPPER_HIDE && Footer.transform.localPosition.y != FOOTER_HIDE)
                {
                    //アッパーフッターが隠れる
                    Upper.transform.localPosition = new Vector2(0, UFSlideAnime(Upper.transform.localPosition.y, UPPER_HIDE, 7));
                    Footer.transform.localPosition = new Vector2(0, UFSlideAnime(Footer.transform.localPosition.y, FOOTER_HIDE, 7));
                }
                else
                {
                    //セレクトモード移行
                    homeStatus = Status.DISP_SLIDE;
                }
                break;

            case Status.DISP_SLIDE:
                if (Upper.transform.localPosition.y != UPPER_DISP && Footer.transform.localPosition.y != UPPER_DISP)
                {
                    //アッパーフッターの出現
                    Upper.transform.localPosition = new Vector2(0, UFSlideAnime(Upper.transform.localPosition.y, UPPER_DISP, 7));
                    Footer.transform.localPosition = new Vector2(0, UFSlideAnime(Footer.transform.localPosition.y, UPPER_DISP, 7));
                }
                else
                {
                    //セレクトカーモードキャンバス表示
                    CanvasSelectCarMode.SetActive(true);


                    //1Pカーオブジェクト表示
                    Car1Model.SetActive(true);
                    Car1PMode[(short)isCar1P].SetActive(true);

                    //カーモデル表示
                    if (isMode == Mode.GAMEMODE_BATTLE)
                    {
                        //2Pカーオブジェクト表示
                        Car2Model.SetActive(true);
                        Car2PMode[(short)isCar2P].SetActive(true);

                        //位置を初期化
                        Car1Model.transform.localPosition = new Vector3(-2, 0, 0);
                        Car2Model.transform.localPosition = new Vector3(2, 0, 0);
                        //角度を初期化
                        Car1Model.transform.localEulerAngles = new Vector3(0, 0, 0);
                        Car2Model.transform.localEulerAngles = new Vector3(0, 0, 0);
                    }
                    else
                    {
                        //位置を初期化
                        Car1Model.transform.localPosition = new Vector3(0, 0, 0);
                        //角度を初期化
                        Car1Model.transform.localEulerAngles = new Vector3(0, 0, 0);
                    }

                    //セレクトモード移行
                    homeStatus = Status.SELECT_CAR;
                }
                break;

            case Status.SELECT_CAR:

                //セレクトカーアニメーション
                SelectCarAnime();

                Car1Model.transform.Rotate(new Vector3(0, 90, 0) * Time.deltaTime, Space.World);
                //カーモデル表示
                if (isMode == Mode.GAMEMODE_BATTLE)
                {
                    Car2Model.transform.Rotate(new Vector3(0, 90, 0) * Time.deltaTime, Space.World);
                }
                break;

            case Status.END_SLIDE:

                if (Upper.transform.localPosition.y != UPPER_HIDE && Footer.transform.localPosition.y != FOOTER_HIDE)
                {
                    //アッパーフッターが隠れる
                    Upper.transform.localPosition = new Vector2(0, UFSlideAnime(Upper.transform.localPosition.y, UPPER_HIDE, 7));
                    Footer.transform.localPosition = new Vector2(0, UFSlideAnime(Footer.transform.localPosition.y, FOOTER_HIDE, 7));
                }
                else
                {
                    if (isStart)
                    {
                        switch (isCar1P)
                        {
                            case Car.CAR_CIVIC: cV.carType1P = CommonVariable.CarType1P.Civic; break;
                            case Car.CAR_CELICA: cV.carType1P = CommonVariable.CarType1P.Celica; break;
                            case Car.CAR_EVO6: cV.carType1P = CommonVariable.CarType1P.Evo; break;
                            case Car.CAR_R33: cV.carType1P = CommonVariable.CarType1P.Skyline; break;
                        }
                        switch (isCar2P)
                        {
                            case Car.CAR_CIVIC: cV.carType2P = CommonVariable.CarType2P.Civic; break;
                            case Car.CAR_CELICA: cV.carType2P = CommonVariable.CarType2P.Celica; break;
                            case Car.CAR_EVO6: cV.carType2P = CommonVariable.CarType2P.Evo; break;
                            case Car.CAR_R33: cV.carType2P = CommonVariable.CarType2P.Skyline; break;
                        }
                        switch (isMode)
                        {
                            case Mode.GAMEMODE_ATTACK: cV.gameMode = CommonVariable.GameMode.TIMEATTACK; break;
                            case Mode.GAMEMODE_BATTLE: cV.gameMode = CommonVariable.GameMode.BATTLE; break;
                        }
                        SceneManager.LoadScene("LoadingScene");
                    }
                    else
                    {
                        //セレクトモードキャンバス非表示
                        CanvasSelectMode.SetActive(false);
                        //セレクトカーモードキャンバス表示
                        CanvasSelectCarMode.SetActive(false);

                        //カーオブジェクト非表示
                        Car1PMode[(short)isCar1P].SetActive(false);
                        Car2PMode[(short)isCar2P].SetActive(false);

                        //カーモデル初期化
                        isCar1P = Car.CAR_CIVIC;
                        isCar2P = Car.CAR_CIVIC;
                        //シーン移動
                        homeStatus = Status.START_SLIDE;
                    }
                }

                break;
        }
    }

    /* ======================================================================= *
     * アプデ
     * キー入力
     * ======================================================================= */
    private void Update()
    {
        switch (homeStatus)
        {
            case Status.START_BGM:
                sM.BGMPlay(homeBGM, true, 0.5f);
                homeStatus = Status.START_SLIDE;
                break;
            case Status.SELECT_MODE:

                //セレクトモードキー入力
                SelectModeKey();

                break;
            case Status.SELECT_CAR:

                //セレクトカーモードキー入力
                if (isMode == Mode.GAMEMODE_ATTACK)
                {
                    SelectCarModeKey();
                }
                else
                {
                    SelectCarModeBattleKey();
                }


                //枠ポジション変更
                ChangePosition();
                break;
        }
    }

    /* ======================================================================= *
     * セレクトモードキー
     * ======================================================================= */
    private void SelectModeKey()
    {
        /*--------------------アナログスティック--------------------*/
        //離したとき
        if (!onPushStick1P && (Input.GetAxis("Handle1") == 0 || Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow)))
        {
            onPushStick1P = true;
        }
        //左入力
        if (onPushStick1P && (Input.GetAxis("Handle1") < 0 || Input.GetKeyDown(KeyCode.LeftArrow)))
        {
            onPushStick1P = false;
            if (isMode == Mode.GAMEMODE_BATTLE)
            {
                //モードを選ぶ
                isMode = Mode.GAMEMODE_ATTACK;

                //枠のポジション設定
                ModeWakuRed.transform.position = GameObject.Find("CanvasSelectMode/TimeAttackModeImg").transform.position;
            }
            else
            {
                //モードを選ぶ
                isMode = Mode.GAMEMODE_BATTLE;

                //枠のポジション設定
                ModeWakuRed.transform.position = GameObject.Find("CanvasSelectMode/BattleModeImg").transform.position;
            }
        }
        //右入力
        if (onPushStick1P && (Input.GetAxis("Handle1") > 0 || Input.GetKeyDown(KeyCode.RightArrow)))
        {
            onPushStick1P = false;
            if (isMode == Mode.GAMEMODE_ATTACK)
            {
                //モードを選ぶ
                isMode = Mode.GAMEMODE_BATTLE;

                //枠のポジション設定
                ModeWakuRed.transform.position = GameObject.Find("CanvasSelectMode/BattleModeImg").transform.position;
            }
            else
            {
                //モードを選ぶ
                isMode = Mode.GAMEMODE_ATTACK;

                //枠のポジション設定
                ModeWakuRed.transform.position = GameObject.Find("CanvasSelectMode/TimeAttackModeImg").transform.position;
            }
        }

        /*--------------------トリガー--------------------*/
        //離したとき
        if (!onPushTrigger1P && (Input.GetAxis("Trigger1") == 0 || Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow)))
        {
            onPushTrigger1P = true;
        }
        //右入力
        if (onPushTrigger1P && (Input.GetAxis("Trigger1") < 0 || Input.GetKeyDown(KeyCode.DownArrow)))
        {
            onPushTrigger1P = false;
            sM.SEPlay(pushSE, false, 1);// SE:ホームSE

            //セレクトモード移行
            homeStatus = Status.HIDE_SLIDE;
        }

        //Debug.Log(Input.GetAxis("Trigger1"));
    }

    /* ======================================================================= *
     * セレクトモードアニメーション
     * ======================================================================= */
    private void SelectModeAnime()
    {
        //アルファ値を設定する
        ModeWakuRed.GetComponent<CanvasRenderer>().SetAlpha(flushAlpha());
    }

    /* ======================================================================= *
     * アルファ値の変化
     * ======================================================================= */
    private float flushAlpha()
    {
        //点滅：濃ゆくなる
        if (flashingFlg && textAlpha <= ALPHA_MAX)
        {
            textAlpha += WAKU_FLASHING_SPEED;//アルファ値を増やす
        }
        else
        {
            flashingFlg = false;//上限に来たら減らす
        }

        //点滅：薄くなる
        if (!flashingFlg && textAlpha > ALPHA_MIN)
        {
            textAlpha -= WAKU_FLASHING_SPEED;//アルファ値を減らす
        }
        else
        {
            flashingFlg = true;//上限に来たら増やす
        }

        return textAlpha;
    }

    /* ======================================================================= *
     * セレクトモードアニメーション
     * ======================================================================= */
    private float UFSlideAnime(float startY, float endY, float speed)
    {
        float returnY = startY;

        if (startY == endY)
        {
            return returnY;
        }
        else if (startY < endY)
        {
            returnY += speed;

            if (endY < returnY) returnY = endY;
        }
        else if (startY > endY)
        {
            returnY -= speed;

            if (endY > returnY) returnY = endY;
        }

        return returnY;
    }

    /* ======================================================================= *
     * セレクトカーモードキー
     * ======================================================================= */
    private void SelectCarModeKey()
    {
        /*--------------------アナログスティック = 1P = --------------------*/
        //離したとき
        if (!onPushStick1P && (Input.GetAxis("Handle1") == 0 || Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow)))
        {
            onPushStick1P = true;
        }
        //左入力
        if (onPushStick1P && (Input.GetAxis("Handle1") < 0 || Input.GetKeyDown(KeyCode.LeftArrow)))
        {
            onPushStick1P = false;

            Car1PMode[(short)isCar1P].SetActive(false);
            if (isCar1P == Car.CAR_CIVIC)
            {
                //前モードを選ぶ
                isCar1P = Car.CAR_R33;
            }
            else
            {
                //次の車選択
                isCar1P--;
            }
            Car1PMode[(short)isCar1P].SetActive(true);
            //changeCar();
        }
        //右入力
        if (onPushStick1P && (Input.GetAxis("Handle1") > 0 || Input.GetKeyDown(KeyCode.RightArrow)))
        {
            onPushStick1P = false;

            Car1PMode[(short)isCar1P].SetActive(false);
            if (isCar1P == Car.CAR_R33)
            {
                //前モードを選ぶ
                isCar1P = Car.CAR_CIVIC;
            }
            else
            {
                //次の車選択
                isCar1P++;
            }
            Car1PMode[(short)isCar1P].SetActive(true);
            //changeCar();
        }

        /*--------------------トリガー 1P--------------------*/
        //離したとき
        if (!onPushTrigger1P && (Input.GetAxis("Trigger1") == 0 || Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow)))
        {
            onPushTrigger1P = true;
        }
        //左入力
        if (onPushTrigger1P && (Input.GetAxis("Trigger1") > 0 || Input.GetKeyDown(KeyCode.UpArrow)))
        {
            onPushTrigger1P = false;
            sM.SEPlay(pushSE, false, 1);// SE:ホームSE

            //戻る
            Select1P = isStart = false;

            checkWaku();//枠確認
            checkFlg();//進めるか確認
        }
        //右入力
        if (onPushTrigger1P && (Input.GetAxis("Trigger1") < 0 || Input.GetKeyDown(KeyCode.DownArrow)))
        {
            onPushTrigger1P = false;
            sM.SEPlay(pushSE, false, 1);// SE:ホームSE

            //進む、選択終了
            Select1P = isStart = true;

            checkFlg();//進めるか確認
        }
    }

    /* ======================================================================= *
     * セレクトカーモードキー（バトルモード）
     * ======================================================================= */
    private void SelectCarModeBattleKey()
    {
        /*--------------------アナログスティック = 1P = --------------------*/
        //離したとき
        if (!onPushStick1P && (Input.GetAxis("Handle1") == 0 || Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow)))
        {
            onPushStick1P = true;
        }
        if (!Select1P)
        {
            //左入力
            if (onPushStick1P && (Input.GetAxis("Handle1") < 0 || Input.GetKeyDown(KeyCode.LeftArrow)))
            {
                onPushStick1P = false;

                Car1PMode[(short)isCar1P].SetActive(false);
                if (isCar1P == Car.CAR_CIVIC)
                {
                    //前モードを選ぶ
                    isCar1P = Car.CAR_R33;
                }
                else
                {
                    //次の車選択
                    isCar1P--;
                }
                Car1PMode[(short)isCar1P].SetActive(true);
                //changeCar();
            }
            //右入力
            if (onPushStick1P && (Input.GetAxis("Handle1") > 0 || Input.GetKeyDown(KeyCode.RightArrow)))
            {
                onPushStick1P = false;

                Car1PMode[(short)isCar1P].SetActive(false);
                if (isCar1P == Car.CAR_R33)
                {
                    //前モードを選ぶ
                    isCar1P = Car.CAR_CIVIC;
                }
                else
                {
                    //次の車選択
                    isCar1P++;
                }
                Car1PMode[(short)isCar1P].SetActive(true);
                //changeCar();
            }
        }

        /*--------------------トリガー 1P--------------------*/
        //離したとき
        if (!onPushTrigger1P && (Input.GetAxis("Trigger1") == 0 || Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow)))
        {
            onPushTrigger1P = true;
        }
        //左入力 戻るやつ
        if (onPushTrigger1P && (Input.GetAxis("Trigger1") > 0 || Input.GetKeyDown(KeyCode.UpArrow)))
        {
            onPushTrigger1P = false;
            sM.SEPlay(pushSE, false, 1);// SE:ホームSE

            //戻る
            Select1P = false;//選択解除
            if (!Select2P)
            {
                isStart = false;//スタートさせない
                checkFlg();//進めるか確認
            }

            //checkWaku();//枠確認

        }
        //右入力 進むやつ
        if (onPushTrigger1P && (Input.GetAxis("Trigger1") < 0 || Input.GetKeyDown(KeyCode.DownArrow)))
        {
            onPushTrigger1P = false;
            sM.SEPlay(pushSE, false, 1);// SE:ホームSE

            //進む、選択終了
            Select1P = true;//選択
            if (Select2P)
            {
                isStart = true;//スタートさせない
                checkFlg();//進めるか確認
            }

            //checkFlg();//進めるか確認
        }

        /*********************************************************************
         * ↑1P
         * ↓2P
         *********************************************************************/

        /*--------------------アナログスティック = 2P = --------------------*/
        //離したとき
        if (!onPushStick2P && (Input.GetAxis("Handle2") == 0 || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D)))
        {
            onPushStick2P = true;
        }
        if (!Select2P)
        {
            //左入力
            if (onPushStick2P && (Input.GetAxis("Handle2") < 0 || Input.GetKeyDown(KeyCode.A)))
            {
                onPushStick2P = false;

                Car2PMode[(short)isCar2P].SetActive(false);
                if (isCar2P == Car.CAR_CIVIC)
                {
                    //前モードを選ぶ
                    isCar2P = Car.CAR_R33;
                }
                else
                {
                    //次の車選択
                    isCar2P--;
                }
                Car2PMode[(short)isCar2P].SetActive(true);
                //changeCar();
            }
            //右入力
            if (onPushStick2P && (Input.GetAxis("Handle2") > 0 || Input.GetKeyDown(KeyCode.D)))
            {
                onPushStick2P = false;

                Car2PMode[(short)isCar2P].SetActive(false);
                if (isCar2P == Car.CAR_R33)
                {
                    //前モードを選ぶ
                    isCar2P = Car.CAR_CIVIC;
                }
                else
                {
                    //次の車選択
                    isCar2P++;
                }
                Car2PMode[(short)isCar2P].SetActive(true);
                //changeCar();
            }
        }

        /*--------------------トリガー 2P--------------------*/
        //離したとき
        if (!onPushTrigger2P && (Input.GetAxis("Trigger2") == 0 || Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S)))
        {
            onPushTrigger2P = true;
        }
        //左入力
        if (onPushTrigger2P && (Input.GetAxis("Trigger2") > 0 || Input.GetKeyDown(KeyCode.W)))
        {
            onPushTrigger2P = false;
            sM.SEPlay(pushSE, false, 1);// SE:ホームSE

            //戻る
            Select2P = false;//選択解除
            if (!Select1P)
            {
                isStart = false;//スタートさせない
                checkFlg();//進めるか確認
            }
        }
        //右入力
        if (onPushTrigger2P && (Input.GetAxis("Trigger2") < 0 || Input.GetKeyDown(KeyCode.S)))
        {
            onPushTrigger2P = false;
            sM.SEPlay(pushSE, false, 1);// SE:ホームSE

            //進む、選択終了
            Select2P = true;//選択
            if (Select1P)
            {
                isStart = true;//スタートさせない
                checkFlg();//進めるか確認
            }

        }
    }

    /* ======================================================================= *
     * 枠確認
     * ======================================================================= */
    private void changeCar()
    {
        //対戦モードか？（2人プレイ）
        if (isMode == Mode.GAMEMODE_BATTLE)
        {

        }
        //タイムアタックモード（1人プレイ）
        else
        {

        }

        if (isCar1P == Car.CAR_CIVIC)
        {
            //前モードを選ぶ
            isCar1P = Car.CAR_R33;
        }
        else
        {
            //次の車選択
            isCar1P--;
        }

        Car1PMode[(short)isCar1P].SetActive(true);
    }

    /* ======================================================================= *
     * 枠確認
     * ======================================================================= */
    private void checkWaku()
    {
        //対戦モードか？（2人プレイ）
        if (isMode == Mode.GAMEMODE_BATTLE)
        {

        }
        //タイムアタックモード（1人プレイ）
        else
        {
            WakuAka.SetActive(true);
        }
    }

    /* ======================================================================= *
     * 状態確認
     * ======================================================================= */
    private void checkFlg()
    {
        //対戦モードか？（2人プレイ）
        if (isMode == Mode.GAMEMODE_BATTLE)
        {
            homeStatus = Status.END_SLIDE;
        }
        //タイムアタックモード（1人プレイ）
        else
        {
            homeStatus = Status.END_SLIDE;
        }
    }

    /* ======================================================================= *
     * 枠ポジション変更
     * ======================================================================= */
    private void ChangePosition()
    {
        GameObject GameObj1P, GameObj2P;

        //対戦モード(2人プレイ)
        if (isMode == Mode.GAMEMODE_BATTLE)
        {
            if (isCar1P == isCar2P)
            {
                WakuAka.SetActive(false);
                WakuAo.SetActive(false);
                WakuAkaHalf.SetActive(true);
                WakuAoHalf.SetActive(true);

                GameObj1P = WakuAkaHalf;
                GameObj2P = WakuAoHalf;
            }
            else
            {
                WakuAka.SetActive(true);
                WakuAo.SetActive(true);
                WakuAkaHalf.SetActive(false);
                WakuAoHalf.SetActive(false);

                GameObj1P = WakuAka;
                GameObj2P = WakuAo;
            }

            float Y2 = GameObj2P.transform.localPosition.y;
            switch (isCar2P)
            {
                case Car.CAR_CIVIC:
                    GameObj2P.transform.localPosition = new Vector2(GameObject.Find("CanvasSelectCarMode/CarInfoCivic").transform.localPosition.x, Y2);
                    break;
                case Car.CAR_CELICA:
                    GameObj2P.transform.localPosition = new Vector2(GameObject.Find("CanvasSelectCarMode/CarInfoCelica").transform.localPosition.x, Y2);
                    break;
                case Car.CAR_EVO6:
                    GameObj2P.transform.localPosition = new Vector2(GameObject.Find("CanvasSelectCarMode/CarInfoEvo6").transform.localPosition.x, Y2);
                    break;
                case Car.CAR_R33:
                    GameObj2P.transform.localPosition = new Vector2(GameObject.Find("CanvasSelectCarMode/CarInfoR33").transform.localPosition.x, Y2);
                    break;
            }
        }
        //タイムアタックモード(1人プレイ)
        else
        {
            GameObj1P = WakuAka;
            WakuAka.SetActive(true);
        }

        float Y1 = GameObj1P.transform.localPosition.y;
        switch (isCar1P)
        {
            case Car.CAR_CIVIC:
                GameObj1P.transform.localPosition = new Vector2(GameObject.Find("CanvasSelectCarMode/CarInfoCivic").transform.localPosition.x, Y1);
                break;
            case Car.CAR_CELICA:
                GameObj1P.transform.localPosition = new Vector2(GameObject.Find("CanvasSelectCarMode/CarInfoCelica").transform.localPosition.x, Y1);
                break;
            case Car.CAR_EVO6:
                GameObj1P.transform.localPosition = new Vector2(GameObject.Find("CanvasSelectCarMode/CarInfoEvo6").transform.localPosition.x, Y1);
                break;
            case Car.CAR_R33:
                GameObj1P.transform.localPosition = new Vector2(GameObject.Find("CanvasSelectCarMode/CarInfoR33").transform.localPosition.x, Y1);
                break;
        }
    }

    /* ======================================================================= *
     * セレクトカーモードアニメーション
     * ======================================================================= */
    private void SelectCarAnime()
    {
        float alpha = flushAlpha();
        switch (isMode)
        {
            //タイムアタックモード
            case Mode.GAMEMODE_ATTACK:

                //アルファ値を設定する
                WakuAka.GetComponent<CanvasRenderer>().SetAlpha(alpha);

                break;

            //対戦モード
            case Mode.GAMEMODE_BATTLE:

                //アルファ値を設定する
                if (Select1P)
                {
                    WakuAka.GetComponent<CanvasRenderer>().SetAlpha(1);
                    WakuAkaHalf.GetComponent<CanvasRenderer>().SetAlpha(1);
                }
                else
                {
                    WakuAka.GetComponent<CanvasRenderer>().SetAlpha(alpha);
                    WakuAkaHalf.GetComponent<CanvasRenderer>().SetAlpha(alpha);
                }

                //アルファ値を設定する
                if (Select2P)
                {
                    WakuAo.GetComponent<CanvasRenderer>().SetAlpha(1);
                    WakuAoHalf.GetComponent<CanvasRenderer>().SetAlpha(1);
                }
                else
                {
                    WakuAo.GetComponent<CanvasRenderer>().SetAlpha(alpha);
                    WakuAoHalf.GetComponent<CanvasRenderer>().SetAlpha(alpha);
                }


                break;
        }
    }
}
