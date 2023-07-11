using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/*--------------タイヤの情報---------------*/
[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public GameObject leftWheelObj;
    public GameObject rightWheelObj;
    public bool motor; // このホイールはモーターにアタッチされているかどうか
    public bool steering; // このホイールはハンドルの角度を反映しているかどうか
}

public class carScript : MonoBehaviour
{
    /*---------------列挙型---------------*/
    private MeterEnum meterEnum;
    private enum MeterEnum
    {
        MIN, MAX
    }

    private reversStatus rStatus;
    private enum reversStatus
    {
        REVERCE = -1, NOT_REVERCE = 1
    }

    /*---------------車別の数字---------------*/

    private readonly float[,] rMAX_SPEED = new float[,]//ギアあたりの最高速度
    {//1,2,3,4,5,6
        { 125.0f, 175.6f, 200.8f, 250.5f, 320.0f, 0 },//civic
        //{ 125.0f, 193.7f, 229.8f, 277.2f, 340.0f, 0 },//evo
        { 125.0f, 143.7f, 197.8f, 248.3f, 340.2f, 0 },
        { 125.0f, 147.5f, 185.9f, 213.2f, 271.1f, 310.0f },//celica
        { 125.0f, 143.7f, 197.8f, 248.3f, 320.2f, 0 },//skyline
    };

    private readonly float[,] rACCELERATION = new float[,]//エンジン回転数の上昇率
    {//1,2,3,4,5,6
         { 60.0f, 17.0f, 16.0f, 10.0f, 2.0f, 0.0f },//civic
         { 60.0f, 17.0f, 10.0f, 4.0f, 2.0f, 0.0f },//evo { 40.0f, 17.0f, 10.0f, 4.0f, 2.0f, 0.0f },
         { 60.0f, 20.0f, 16.0f, 10.0f, 8.0f, 4.0f },//celica //{ 40.0f, 20.0f, 16.0f, 10.0f, 4.0f, 2.0f },
         { 60.0f, 17.0f, 10.0f, 4.0f, 2.0f, 0.0f },//skyline
    };

    private readonly float[,] rHANDRING = new float[,]//ハンドリング
    {
        { 18.0f, 17.0f,16.0f,15.0f,12.0f,10.0f,8.0f,6.0f,4.0f,3.0f,1.0f},//civic
        { 18.0f, 17.0f,16.0f,15.0f,12.0f,10.0f,8.0f,6.0f,4.0f,3.0f,1.0f},//evo
        { 18.0f, 17.0f,16.0f,15.0f,12.0f,10.0f,8.0f,6.0f,4.0f,3.0f,1.0f},//celica
        { 18.0f, 17.0f,16.0f,15.0f,12.0f,10.0f,8.0f,6.0f,4.0f,3.0f,1.0f}//skyline
    };


    private readonly short[] rMAX_GEAR = new short[]//ギア数
   {
        5,//civic
        5,//evo
        6,//celica
        5,//skyline
   };

    private readonly short[] rCAR_MASS = new short[]//重量数
  {
        1040,//civic
        1360,//evo
        1180,//celica
        1550,//skyline
  };

    /*
    private readonly float[,] rGEAR_RATIO = new float[,]//ギア比
    {//1,2,3,4,5,6
        { 4.230f, 2.105f, 1.458f, 1.107f, 0.0848f,0.0f},//civic
        { 2.785f, 1.950f, 1.407f, 1.031f, 0.7610f,0.0f},//evo
        { 3.166f, 2.050f, 1.481f, 1.166f, 0.916f,0.725f},//celica
        { 3.214f, 1.925f, 1.302f, 1.000f, 0.7520f,0.0f},//skyline
    };

    private readonly float[] rGEAR_RADURAT = new float[]//最終減速比
    {
        4.400f,//civic
        4.529f,//evo
        4.529f,//celica
        4.111f,//skyline
    };
    */

    private readonly float[,] rENGINE_TORQUE = new float[,]//回転数別エンジントルク(N・m)
    {//500rpmごと
        { 0.1f, 0.1f, 0.1f, 0.1f, 833.5654f, 862.9854f, 892.4053f, 921.8253f, 951.2452f, 980.6652f, 1010.0852f, 1039.5051f, 1068.9251f, 1098.345f, 1117.9583f, 1157.1849f, 1157.1849f, 1098.345f, 1000.2785f,0.01f},//civic
        //{ 0.1f, 0.1f, 0.1f, 0.1f, 1555.4546f, 1693.4813f, 1725.3333f, 1682.8637f, 1640.394f, 162.9429f, 1555.4546f, 1512.985f, 1470.5153f, 1428.0456f, 1268.7837f, 0.01f, 0.01f, 0.01f, 0.01f,0.01f},//evo
        { 0.1f, 0.1f, 0.1f, 0.1f, 1938.5241f, 2044.9527f, 2136.1781f, 2257.811f, 2364.2396f, 2470.6683f, 2470.6683f, 2409.8514f, 2364.2396f, 2303.4227f, 2242.6068f, 2105.7696f, 2046.9297f, 1948.7887f, 1910.2623f,0.01f},//evo
        //{ 0.1f, 0.1f, 0.1f, 0.1f, 868.8982f, 904.9521f, 941.0059f, 969.8491f, 998.6918f, 1041.9568f,1085.2218f, 1106.8533f, 1135.6966f, 1171.7508f, 1178.9616f, 1150.1183f, 1157.1849f, 1078.0109f, 1000.2785f,0.01f},//celica
        { 0.1f, 0.1f, 0.1f, 0.1f, 833.5654f, 862.9854f, 892.4053f, 921.8253f, 951.2452f, 980.6652f, 1010.0852f, 1039.5051f, 1068.9251f, 1098.345f, 1117.9583f, 1157.1849f, 1157.1849f, 1098.345f, 1000.2785f,0.01f},//celica
        { 0.1f, 0.1f, 0.1f, 0.1f, 1938.5241f, 2044.9527f, 2136.1781f, 2257.811f, 2364.2396f, 2470.6683f, 2470.6683f, 2409.8514f, 2364.2396f, 2303.4227f, 2242.6068f, 2105.7696f, 2046.9297f, 1948.7887f, 1910.2623f,0.01f},//skyline
    };

    /*private readonly float[,] rENGINE_BRAKE = new float[,]//エンジンブレーキ
    {//N,1,2,3,4,5,6
        { 100f, 100f, 100f, 100f, 100f, 100f, 100f },//civic
        { 100f, 0.65f, 0.45f, 0.1f, 0.095f, 0.075f, 0.055f },//evo
        { 100f, 0.65f, 0.45f, 0.1f, 0.095f, 0.075f, 0.055f },//celica
        { 100f, 0.65f, 0.45f, 0.1f, 0.095f, 0.075f, 0.055f },//skyline
        
    };*/

    private readonly float[] rMAX_ENGINE_RPM = new float[]//最大エンジン回転数
    {
        9000.0f,//civic
        7000.0f,//evo
        7000.0f,//celica
        9000.0f,//skyline
    };

    private readonly short[] rENGINE_REV = new short[]//レブ
    {
        300,//civic
        300,//evo
        300,//celica
        300,//skyline
    };

    private readonly short[] rWHEELD_RIVE = new short[]//駆動数
    {
        2,//civic
        4,//evo
        2,//celica
        4,//skyline
    };

    private readonly float[,] rSPEED_METER_NUM = new float[,]//スピードメーターの針の位置
    {//N,1,2,3,4,5,6
        { 127.1f, -197.0f},//civic
        { 137.0f, -295.0f},//evo
        { 182.0f, -278.0f},//celica
        { 103.0f, -152.0f},//skyline
    };
    private readonly float[,] rRPM_METER_NUM = new float[,]//タコメーターの針の位置
    {//N,1,2,3,4,5,6
        { 124.8f, -78.6f},//civic
        { 129.0f, -74.0f},//evo
        { 180.0f, -9.5f},//celica
        { 183.0f, -49.0f},//skyline
    };

    private readonly float[] rBRAKE = new float[]//ブレーキ
    {
        1000.0f,//civic
        1000.0f,//evo
        1000.0f,//celica
        1000.0f,//skyline
    };

    private readonly Vector3[] rCAMERA_POS_FPS = new Vector3[]//カメラポジション ダッシュボード
    {
        new Vector3(0,0.009f,0.008f),//civic
        new Vector3(0,0.011f,0.01f),//evo
        new Vector3(0,0.0088f,0.012f),//celica
        new Vector3(0,0.0095f,0.01f),//skyline
    };

    private readonly Vector3[] rCAMERA_ROT_FPS = new Vector3[]//カメラローテーション ダッシュボード
    {
        new Vector3(1,0,0),//civic
        new Vector3(0.1f,0,0),//evo
        new Vector3(1.5f,0,0),//celica
        new Vector3(2.5f,0,0),//skyline
    };

    /*private readonly Vector3[] rCAMERA_ROT_FPS = new Vector3[]//カメラローテーション ダッシュボード
    {
        new Vector3(1,0,0),//civic
        new Vector3(0.1f,0,0),//evo
        new Vector3(1.5f,0,0),//celica
        new Vector3(2.5f,0,0),//skyline
    };*/

    public Vector3 center;//重心の位置
    Rigidbody rB;

    /*---------------インスペクターに表示させるオブジェクトや変数等---------------*/

    [Header("使用する車")]
    [SerializeField]
    private CarType carType;
    private enum CarType
    {
        Civic, Evo, Celica, Skyline
    }

    [Header("タイヤの情報")]
    public List<AxleInfo> axleInfos; // 個々の車軸の情報

    [Header("スピードメーターの針")]
    public GameObject speedNeedle;

    [Header("タコメーターの針")]
    public GameObject rpmNeedle;

    [Header("ギア画像")]
    public List<GameObject> gearImgs; // ギアの画像

    [Header("速度テキスト")]
    public Text speedtext; // スピード

    [Header("トータルタイムテキスト")]
    [SerializeField]
    private Text totalTimeText;

    [Header("残り時間テキスト")]
    [SerializeField]
    private Text remainingTimeText;

    [Header("逆走ゲームオブジェクト")]
    [SerializeField]
    private GameObject wwDCheckGameObj;

    [Header("自分のキャンバス")]
    [SerializeField]
    private GameObject myCanvas;

    [Header("テールランプマテリアル")]
    [SerializeField]
    private short LAMP_NUM;
    [SerializeField]
    private List<GameObject> LampMaterial;

    [Header("ターボサウンド")]
    [SerializeField]
    private AudioClip TurboSound;

    [Header("ファイアサウンド")]
    [SerializeField]
    private short FireSoundNum;
    [SerializeField]
    private List<AudioClip> FireSound;
    /*-----------------------------------スクリプト-----------------------------------*/
    [HideInInspector]
    public WrongWayDrivingCheck wwDC;//逆走判定
    private EngineSound eS;//エンジンサウンド

    /*-----------------------------------オブジェクト-----------------------------------*/
    private GameObject myCamera; //自分のカメラオブジェクト
    private GameObject revRamp; //レブランプオブジェクト
    private Text myRankTxt; //自分の順位オブジェクト

    /*-----------------------------------定数-----------------------------------*/
    //ゲームメインマネージャー
    private GameObject GMManager;
    private GameMainManager gmManager;

    //共通変数
    private GameObject commonVariable;
    private CommonVariable cV;

    const float TORQUE_STEP = 500.0f;//トルクが上がるエンジン回転数
    const short NEUTRAL = 0;
    const float BRAKE_TIME = 1.5f;//ブレーキをかけ続ける時間
    const int MAX_STRAGE = 60;//スピードメーターをヌルヌルさせる数字
    const int MAX_STRAGE_RPM = 40;//RPMをヌルヌルさせる数字

    const float DRIFT_POS = 0.5f;// ドリフト開始のコントローラー地点
    const float DRIFT_NUM = 1.4f;// ドリフト数
    const float DRIFT_SWITCHING_NUM = 0.2f;// ドリフト数
    const float DRIFT_SWITCHING_TIME = 0.3f;// ドリフト数

    const float DRIFT_SOUND_INC = 0.08f;//ドリフト音増加速度
    const float DRIFT_SOUND_DEC = 0.01f;//ドリフト音減少速度
    const float DRIFT_SOUND_MAX = 0.5f;//ドリフト音最大音

    const float BRAKE_SOUND_INC = 0.1f;//ドリフト音増加速度
    const float BRAKE_SOUND_DEC = 0.03f;//ドリフト音減少速度
    const float BRAKE_SOUND_MAX = 0.8f;//ドリフト音最大音

    const float SPEED_ADJUST = 5.0f;//速度調整

    const float FIRE_TIME_ACCU = 0.8f;//ファイアが貯まる時間
    const int FIRE_TIME_MAX = 15;//ファイアが出る最大時間

    const float TAIL_LAMP_MIN = 0.35f;//テールランプ最小
    const float TAIL_LAMP_MAX = 1.15f;//テールランプ最大

    const float REVERCE_TIME = 0.5f;//リバースするまでの時間

    Vector3 rCAMERA_POS_TPS = new Vector3(0, 0.0214f, -0.055f); // カメラポジション 3人称
    Vector3 rCAMERA_ROT_TPS = new Vector3(8, 0, 0); // カメラローテーション 3人称

    /*-----------------------------------変数-----------------------------------*/
    [HideInInspector]
    public short nowCarGear; // 現在のギア N-1-2-3-4-5-(6)
    public short myNumber; // 自分の番号、バトルモード時

    public short myRank; // 自分の順位、バトルモード時

    private float carHorizontal;//水平方向
    private float carVertical;  //垂直方向

    //エンジン関連
    [HideInInspector]
    public float nowSpeed;     //今の速度
    private float nowTorque;    //今のトルク
    private float nowRPM;       //今のエンジン回転数
    private float nowBrake;     //今のブレーキ
    private float nowEBrake;    //今のエンジンブレーキ

    private bool cameraAngle; // true:ダッシュボード false:3人称

    //ドリフト関連
    private float driftNum;//ドリフト用数字
    private float driftTime;//ドリフト用時間
    private float driftSoundNum;//ドリフトボリューム

    //ブレーキ関連
    private float brakeSoundNum;//ブレーキボリューム

    //ターボ関連
    private bool isTurboSound;//ターボ音フラグ

    //ファイア関連
    private bool isFireSound;//ファイア音フラグ
    private float fireRndTime;//ファイアが出る時間
    private float fireReTime;//ファイア出る時間
    private float accuTime;//ファイアが溜まる時間

    private float countBrakeTime;//0kmでブレーキを掛けている時間
    private bool reverseFlg = false;//バックギアフラグ

    private float tailLamp;//テールランプの明るさ遷移

    private bool revAlarmFlg;//レブアラームのフラグ

    private float reverceTime;//バックタイム

    private int iAvgSpeed = 0;
    private int iAvgRPM = 0;
    private float[] numStrage = new float[MAX_STRAGE];
    private float[] numStrageRPM = new float[MAX_STRAGE_RPM];

    /* ======================================================================= *
     * 初期化
     * ======================================================================= */

    void Start()
    {
        for (int roop = 0; roop < MAX_STRAGE; roop++)
        {
            numStrage[roop] = 0;
        }

        for (int roop = 0; roop < MAX_STRAGE_RPM; roop++)
        {
            numStrageRPM[roop] = 0;
        }

        //ゲームメインマネージャーの読み込み
        GMManager = GameObject.Find("GameMainManager");
        gmManager = GMManager.GetComponent<GameMainManager>();

        //リサルトタイムスクリプトの読み込み
        commonVariable = GameObject.Find("CommonVariable");
        cV = commonVariable.GetComponent<CommonVariable>();

        //スクリプト読み込み
        wwDC = transform.Find("GeneralPurposeColl").gameObject.GetComponent<WrongWayDrivingCheck>();
        eS = gameObject.GetComponent<EngineSound>();

        //カメラオブジェクト読み込み
        myCamera = transform.Find("Camera").gameObject;

        revRamp = GameObject.Find("RevRamp");

        carHorizontal = carVertical = 0;
        // ギアの初期状態を１速にする
        nowCarGear = 0;

        //バックギア
        rStatus = reversStatus.NOT_REVERCE;
        reverceTime = 0;

        //ファイア関連
        accuTime = 0;
        fireRndTime = -1;

        //レブアラームのフラグ
        revAlarmFlg = false;

        rB = GetComponent<Rigidbody>();
        // 重心の設定
        rB.centerOfMass = center;

        // メーター類の初期化
        speedNeedle.GetComponent<needleScript>().getStart(rSPEED_METER_NUM[(int)carType, (int)MeterEnum.MIN], rSPEED_METER_NUM[(int)carType, (int)MeterEnum.MAX], rMAX_SPEED[(int)carType, rMAX_GEAR[(int)carType] - 1]);
        rpmNeedle.GetComponent<needleScript>().getStart(rRPM_METER_NUM[(int)carType, (int)MeterEnum.MIN], rRPM_METER_NUM[(int)carType, (int)MeterEnum.MAX], rMAX_ENGINE_RPM[(int)carType]);

        //順位UI設定
        if (cV.gameMode == CommonVariable.GameMode.BATTLE)
        {
            if (myNumber == 1)
            {
                myRank = cV.Rank1P;
                myRankTxt = gmManager.ccVBattle.transform.Find("1PRank").GetComponent<Text>();
                //ランクを書き起こす
                myRankTxt.text = myRank.ToString();
            }
            else if (myNumber == 2)
            {
                myRank = cV.Rank2P;
                myRankTxt = gmManager.ccVBattle.transform.Find("2PRank").GetComponent<Text>();
                //ランクを書き起こす
                myRankTxt.text = myRank.ToString();
            }
        }

        //テールランプ初期化
        for (int lampfor = 0; lampfor < LAMP_NUM; lampfor++)
        {
            LampMaterial[lampfor].GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.red * TAIL_LAMP_MIN);
        }
    }

    /* ======================================================================= *
     * アプデ
     * ======================================================================= */

    void Update()
    {
        nowSpeed = rB.velocity.magnitude * SPEED_ADJUST;

        float dispSpeed = speedAvg(nowSpeed);//速度の平均計算
        float dispRPM = RPMAvg(nowRPM);

        speedtext.text = dispSpeed.ToString("0");//速度を表示

        totalTimeText.text = gmManager.totalTimes[0, 0].ToString("00") + "'" + gmManager.totalTimes[0, 1].ToString("00") + "\"" + gmManager.totalTimes[0, 2].ToString("000");
        remainingTimeText.text = cV.remainigTime.ToString("0");

        //スピードメーターの針を動かす
        speedNeedle.GetComponent<needleScript>().getNum(dispSpeed);
        //タコメーターの針を動かす
        rpmNeedle.GetComponent<needleScript>().getNum(dispRPM);

        //ゲーム終了時の操作
        switch (gmManager.carStatus)
        {
            case GameMainManager.Status.START:
                if (dispRPM / rMAX_ENGINE_RPM[(int)carType] >= 0.9)
                {
                    revRamp.GetComponent<Image>().color = new Color(1, 0, 0);
                    if (revAlarmFlg && nowCarGear < rMAX_GEAR[(int)carType] - 1) eS.RevAlarm(true);
                    revAlarmFlg = false;
                }
                else if (dispRPM / rMAX_ENGINE_RPM[(int)carType] > 0.7)
                {
                    revRamp.GetComponent<Image>().color = new Color(1, 0.6470588f, 0);
                    if (!revAlarmFlg) eS.RevAlarm(false);
                    revAlarmFlg = true;
                }
                else if (dispRPM / rMAX_ENGINE_RPM[(int)carType] > 0.5)
                {
                    revRamp.GetComponent<Image>().color = new Color(0.253703f, 0.7169812f, 0);
                    if (!revAlarmFlg) eS.RevAlarm(false);
                    revAlarmFlg = true;
                }
                else
                {
                    revRamp.GetComponent<Image>().color = new Color(0, 0, 0);
                    if (!revAlarmFlg) eS.RevAlarm(false);
                    revAlarmFlg = true;
                }
                break;
            case GameMainManager.Status.PLAY:

                if (dispRPM / rMAX_ENGINE_RPM[(int)carType] >= 0.9)
                {
                    revRamp.GetComponent<Image>().color = new Color(1, 0, 0);
                    if (revAlarmFlg && nowCarGear < rMAX_GEAR[(int)carType] - 1) eS.RevAlarm(true);
                    revAlarmFlg = false;
                }
                else if (dispRPM / rMAX_ENGINE_RPM[(int)carType] > 0.7)
                {
                    revRamp.GetComponent<Image>().color = new Color(1, 0.6470588f, 0);
                    if (!revAlarmFlg) eS.RevAlarm(false);
                    revAlarmFlg = true;
                }
                else if (dispRPM / rMAX_ENGINE_RPM[(int)carType] > 0.5)
                {
                    revRamp.GetComponent<Image>().color = new Color(0.253703f, 0.7169812f, 0);
                    if (!revAlarmFlg) eS.RevAlarm(false);
                    revAlarmFlg = true;
                }
                else
                {
                    revRamp.GetComponent<Image>().color = new Color(0, 0, 0);
                    if (!revAlarmFlg) eS.RevAlarm(false);
                    revAlarmFlg = true;
                }

                //順位UI設定
                if (cV.gameMode == CommonVariable.GameMode.BATTLE)
                {
                    //ランクを書き起こす
                    if (myNumber == 1)
                    {
                        myRank = cV.Rank1P;
                        myRankTxt.text = myRank.ToString();
                    }
                    else if (myNumber == 2)
                    {
                        myRank = cV.Rank2P;
                        myRankTxt.text = myRank.ToString();
                    }
                }
                break;
            case GameMainManager.Status.GAMEOVER:
            case GameMainManager.Status.GOAL:
                myCanvas.SetActive(false);

                if (!revAlarmFlg) eS.RevAlarm(false);
                revAlarmFlg = true;

                break;
        }

        //逆走判定
        if (wwDC.WWDCheck()) wwDCheckGameObj.SetActive(true);
        else wwDCheckGameObj.SetActive(false);

        //エンジン音
        eS.GetEngineRPM(nowRPM, rMAX_ENGINE_RPM[(int)carType]);

        //Debug.DrawLine(transform.position, transform.position + transform.rotation * center);
        //アクセル操作
        //Car_Accel(Input.GetAxisRaw("Vertical"));

        //  if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        //  {
        //Car_Accel();        //アクセル操作をしているとき
        //  }

        //ハンドル操作
        // Car_Handle(Input.GetAxisRaw("Horizontal"));
        // Debug.DrawLine(transform.position, transform.position + transform.rotation * center);
    }

    /* ======================================================================= *
     * アプデ
     * 動的なもので使用
     * ======================================================================= */

    void FixedUpdate()
    {

        Debug.Log(nowBrake);
        //WheelColliderにトルクを与える
        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = carHorizontal;
                axleInfo.rightWheel.steerAngle = carHorizontal;
            }
            if (axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = nowTorque / rWHEELD_RIVE[(int)carType];
                axleInfo.rightWheel.motorTorque = nowTorque / rWHEELD_RIVE[(int)carType];

                axleInfo.leftWheel.brakeTorque = nowBrake + nowEBrake;
                axleInfo.rightWheel.brakeTorque = nowBrake + nowEBrake;
            }

            //ホイールのシンクロ
            SyncWheel(axleInfo.leftWheel, axleInfo.leftWheelObj);
            SyncWheel(axleInfo.rightWheel, axleInfo.rightWheelObj);
        }

    }

    /* ======================================================================= *
     * 重心表示
     * ======================================================================= */
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + transform.rotation * center, 0.1f);
    }

    /* ======================================================================= *
     * 対応する視覚的なホイールを見つける
     * Transformとrotationを同期
     * ======================================================================= */
    public void SyncWheel(WheelCollider collider, GameObject Wheel)
    {
        if (!Wheel) return;//モデルがなかったら終了

        Transform visualWheel = Wheel.transform;

        Vector3 position;
        Quaternion rotation;

        collider.GetWorldPose(out position, out rotation);

        //transform.Find("body").transform.localRotation = Quaternion.Euler(bodyY);

        // Vector3 tireY = rotation.eulerAngles;

        visualWheel.transform.rotation = rotation;
        // visualWheel.transform.rotation = Quaternion.Euler(bodyY + tireY);//ローテーション反映
    }

    /* ======================================================================= *
     * スピードとRPMの関係を確認
     * ======================================================================= */
    public bool speedCheck()
    {
        if (nowSpeed <= (nowRPM / rMAX_ENGINE_RPM[(int)carType]) * rMAX_SPEED[(int)carType, nowCarGear] && nowRPM <= rMAX_ENGINE_RPM[(int)carType])
        {
            return true;//エンジン回転数あたりの速度を超えていない
        }
        return false;//超えた
    }

    /* ======================================================================= *
     * RPMとスピードの関係を確認
     * ======================================================================= */
    public float RPMCheck(float getSpeed)
    {
        return getSpeed = getSpeed / rMAX_SPEED[(int)carType, nowCarGear] * rMAX_ENGINE_RPM[(int)carType];
    }

    /* ======================================================================= *
     * 速度の平均を計算する
     * ======================================================================= */
    float speedAvg(float getSpeed)
    {
        float sum = 0;

        numStrage[iAvgSpeed++] = getSpeed;

        if (iAvgSpeed == MAX_STRAGE) iAvgSpeed = 0;

        for (int j = 0; j < MAX_STRAGE; j++)
        {
            sum += numStrage[j];
        }
        return sum / MAX_STRAGE;
    }

    /* ======================================================================= *
     * RPMの平均を計算する
     * ======================================================================= */
    float RPMAvg(float getRPM)
    {
        float sum = 0;

        numStrageRPM[iAvgRPM++] = getRPM;

        if (iAvgRPM == MAX_STRAGE_RPM) iAvgRPM = 0;

        for (int j = 0; j < MAX_STRAGE_RPM; j++)
        {
            sum += numStrageRPM[j];
        }
        return sum / MAX_STRAGE_RPM;
    }

    /* ======================================================================= *
     * マテリアルの色を変える
     * ======================================================================= */
    void SetMatColor(Renderer mesh, Color col)
    {
        mesh.material.color = col; //meshのmaterialの色を変える
    }

    /* ======================================================================= *
     * アクセル操作
     * ======================================================================= */
    public void Car_Accel(float accel)
    {
        carVertical = accel;// 車が前方向に進んでいることの証明

        switch (rStatus)
        {
            case reversStatus.NOT_REVERCE:

                if (nowRPM - RPMCheck(nowSpeed) < 3000)
                {
                    // アクセル操作の再現
                    if (accel > 0/* && nowRPM - RPMCheck(nowSpeed) < 3000*/)
                    {
                        // 最大回転数以内で回転数を上げさせる
                        if (nowRPM <= rMAX_ENGINE_RPM[(int)carType])
                        {
                            // エンジン回転数を上げる
                            nowRPM += accel * rACCELERATION[(int)carType, nowCarGear];
                        }
                        else
                        {
                            // 超えたらレブ
                            nowRPM -= rENGINE_REV[(int)carType];
                        }

                        // ターボ音を鳴らせるようにする
                        isTurboSound = true;

                        //トルク計算
                        //nowTorque = Torque_Calcu(nowRPM);

                        //ファイアをためる
                        if (accuTime < FIRE_TIME_ACCU)
                        {
                            accuTime += Time.deltaTime;
                        }
                        else
                        {
                            isFireSound = true;
                        }

                    }
                    //アクセルを離していたら
                    else// if (accel == 0)
                    {
                        //ターボ音再生
                        if (isTurboSound)
                        {
                            eS.TurboPlay(TurboSound, 0.2f);

                            //次回アクセルを踏まない限り音を鳴らさせない
                            isTurboSound = false;
                        }

                        //ファイア音再生
                        if (isFireSound && accuTime >= FIRE_TIME_ACCU)
                        {
                            System.Random cRandom = new System.Random();
                            if (fireRndTime == -1) fireRndTime = cRandom.Next(FIRE_TIME_MAX);
                            if (fireReTime < fireRndTime * 0.1f)
                            {
                                fireReTime += Time.deltaTime;
                            }
                            else
                            {
                                eS.FirePlay(FireSound[cRandom.Next(FireSoundNum)], 0.8f);

                                //次回アクセルを踏まない限り音を鳴らさせない
                                accuTime = 0;
                                fireRndTime = -1;
                                isTurboSound = false;
                            }
                        }

                        nowRPM -= rACCELERATION[(int)carType, nowCarGear] * (nowRPM / RPMCheck(nowSpeed));
                        if (nowRPM < 1500) nowRPM = 1500;

                    }
                }
                else
                {
                    nowRPM -= rACCELERATION[(int)carType, nowCarGear] * (nowRPM / RPMCheck(nowSpeed));
                    if (nowRPM < 1500) nowRPM = 1500;
                }


                if (nowRPM > 1500)
                {
                    nowEBrake = 0;
                    if (speedCheck()) { nowTorque = Torque_Calcu(nowRPM); }
                    else { nowTorque = -Torque_Calcu(nowRPM); }
                }
                else
                {
                    nowEBrake = 200;
                }
                break;

            case reversStatus.REVERCE:

                if (accel > 0)
                {
                    nowBrake = accel * 2000;
                    if (nowSpeed < 0.5f) rStatus = reversStatus.NOT_REVERCE;
                }
                break;
        }
    }

    /* ======================================================================= *
     * トルク計算
     * ======================================================================= */
    public float Torque_Calcu(float getRPM)
    {
        return getRPM = rENGINE_TORQUE[(int)carType, (int)(getRPM / TORQUE_STEP)];
    }

    /* ======================================================================= *
     * ブレーキ操作
     * ======================================================================= */

    public void Car_Brake(float brake)//バックも含む
    {
        switch (rStatus)
        {
            case reversStatus.NOT_REVERCE:

                nowBrake = brake * rBRAKE[(int)carType];
                nowTorque = 0;

                if (brake > 0)
                {
                    //rStatus = reversStatus.REVERCE;
                    //テールランプ点灯
                    if (tailLamp < TAIL_LAMP_MAX)
                    {
                        tailLamp += 0.08f;
                    }
                    else
                    {
                        tailLamp = TAIL_LAMP_MAX;
                    }

                    //ブレーキ音
                    if (nowSpeed > 5)
                    {
                        //徐々に音が大きくなる
                        if (brakeSoundNum < BRAKE_SOUND_MAX) brakeSoundNum += BRAKE_SOUND_INC;
                        else brakeSoundNum = BRAKE_SOUND_MAX;
                    }
                    else//5キロ以下で再生させない
                    {
                        if (brakeSoundNum > 0) brakeSoundNum -= BRAKE_SOUND_DEC;
                        else brakeSoundNum = 0;
                    }

                    //リバースまでの時間
                    if (nowSpeed < 0.5f)
                    {
                        if (reverceTime > REVERCE_TIME)
                        {
                            //リバースさせる
                            rStatus = reversStatus.REVERCE;
                        }
                        else
                        {
                            reverceTime += Time.deltaTime;
                        }
                    }
                }
                else
                {
                    //テールランプ消灯
                    if (tailLamp > TAIL_LAMP_MIN)
                    {
                        tailLamp -= 0.05f;
                    }
                    else
                    {
                        tailLamp = TAIL_LAMP_MIN;
                    }

                    //ドリフト音
                    //徐々に音が小さくなる
                    if (brakeSoundNum > 0) brakeSoundNum -= BRAKE_SOUND_DEC;
                    else brakeSoundNum = 0;

                }
                break;

            //車両バック
            case reversStatus.REVERCE:

                nowBrake = 0;

                //ブレーキ（バックアクセル）を踏み込んでいたら
                if (brake > 0)
                {
                    // 最大回転数以内で回転数を上げさせる
                    if (nowRPM <= rMAX_ENGINE_RPM[(int)carType])
                    {
                        // エンジン回転数を上げる
                        nowRPM += brake * rACCELERATION[(int)carType, 0];
                    }
                    else
                    {
                        // 超えたらレブ
                        nowRPM -= rENGINE_REV[(int)carType];
                    }

                    // ターボ音を鳴らせるようにする
                    isTurboSound = true;
                }
                //ブレーキ（バックアクセル）を離していたら
                else
                {
                    //ターボ音再生
                    if (isTurboSound)
                    {
                        eS.TurboPlay(TurboSound, 0.2f);

                        //次回アクセルを踏まない限り音を鳴らさせない
                        isTurboSound = false;
                    }

                    nowRPM -= rACCELERATION[(int)carType, 0] * (nowRPM / RPMCheck(nowSpeed));
                    if (nowRPM < 1500) nowRPM = 1500;

                }

                //ブレーキ音
                if (brakeSoundNum > 0) brakeSoundNum -= BRAKE_SOUND_DEC;
                else brakeSoundNum = 0;

                //テールランプ消灯
                if (tailLamp > TAIL_LAMP_MIN)
                {
                    tailLamp -= 0.05f;
                }
                else
                {
                    tailLamp = TAIL_LAMP_MIN;
                }

                //トルクを与える
                if (nowRPM > 1500)
                {
                    nowEBrake = 0;
                    if (speedCheck()) { nowTorque = Torque_Calcu(nowRPM) * (int)rStatus; }
                    else { nowTorque = -Torque_Calcu(nowRPM) * (int)rStatus; }
                }
                else
                {
                    nowEBrake = 200;
                }

                break;
        }

        //ブレーキ音
        eS.BrakeSound(brakeSoundNum);
        //テールランプ
        for (int lampfor = 0; lampfor < LAMP_NUM; lampfor++)
        {
            LampMaterial[lampfor].GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.red * tailLamp);
        }
    }

    /* ======================================================================= *
     * ハンドル操作
     * ======================================================================= */
    public void Car_Handle(float handle)
    {
        float Grip = rHANDRING[(int)carType, (int)(nowSpeed / rMAX_SPEED[(int)carType, rMAX_GEAR[(int)carType] - 1] * 10)];

        //ドリフト移行
        if (handle > DRIFT_POS || handle < -DRIFT_POS)
        {
            driftTime += Time.deltaTime;
            if (driftTime > DRIFT_SWITCHING_TIME)
            {
                //ドリフト中
                if (driftNum < Grip * DRIFT_NUM) driftNum += DRIFT_SWITCHING_NUM;
                else driftNum = Grip * DRIFT_NUM;

                //ドリフト音
                if (nowSpeed > 50)
                {
                    //徐々に音が大きくなる
                    if (driftSoundNum < DRIFT_SOUND_MAX) driftSoundNum += DRIFT_SOUND_INC;
                    else driftSoundNum = DRIFT_SOUND_MAX;
                }
                else//50キロ以下で再生させない
                {
                    if (driftSoundNum > 0) driftSoundNum -= DRIFT_SOUND_DEC;
                    else driftSoundNum = 0;
                }
            }

            carHorizontal = driftNum * handle;
        }
        else
        {
            //グリップ中
            driftTime = 0;

            //ドリフト音
            //徐々に音が小さくなる
            if (driftSoundNum > 0) driftSoundNum -= DRIFT_SOUND_DEC;
            else driftSoundNum = 0;

            if (driftNum > Grip) driftNum -= 1;
            else driftNum = Grip;

            carHorizontal = Grip * handle;
        }

        eS.DriftSound(driftSoundNum);

    }

    /* ======================================================================= *
     * ギアシフト
     * ======================================================================= */
    public void Car_Shift(short shift)
    {
        if (nowCarGear + shift >= NEUTRAL && nowCarGear + shift <= rMAX_GEAR[(int)carType] - 1)
        {
            int nextGear = nowCarGear + shift;

            //1速からリバースもしくはリバースから1速にするとき速度が0.1km以上ならシフトチェンジをしない
            //if ((nextGear + nowCarGear == 1) && nowSpeed > 0.1f) return;
            int shiftRpm;//シフトチェンジ後の加算RPM
            if (nextGear < shift) shiftRpm = 1000;
            else shiftRpm = 0;
            //現在のギア画像を非表示にする
            gearImgs[nowCarGear].SetActive(false);

            //シフトチェンジ
            nowCarGear = (short)nextGear;

            //シフトチェンジ後のギアの画像を表示させる
            gearImgs[nowCarGear].SetActive(true);

            //ターボ音再生
            eS.TurboPlay(TurboSound, 0.2f);

            //ファイア音再生
            if (isFireSound && accuTime >= FIRE_TIME_ACCU)
            {
                System.Random cRandom = new System.Random();
                if (fireRndTime == -1) fireRndTime = cRandom.Next(FIRE_TIME_MAX);
                if (fireReTime < fireRndTime * 0.1f)
                {
                    fireReTime += Time.deltaTime;
                }
                else
                {
                    eS.FirePlay(FireSound[cRandom.Next(FireSoundNum)], 0.8f);

                    //次回アクセルを踏まない限り音を鳴らさせない
                    accuTime = 0;
                    fireRndTime = -1;
                    isTurboSound = false;
                }
            }

            //シフトチェンジ後のRPMを設定
            nowRPM = rMAX_ENGINE_RPM[(int)carType] / rMAX_SPEED[(int)carType, nowCarGear] * nowSpeed + shiftRpm;

            //越し過ぎたら減らす
            if (nowRPM > rMAX_ENGINE_RPM[(int)carType]) nowRPM = rMAX_ENGINE_RPM[(int)carType];
        }
    }

    /* ======================================================================= *
     * カメラ視点変更
     * ======================================================================= */
    public void CameraPosition()
    {
        cameraAngle = !cameraAngle;
        if (cameraAngle)//ダッシュボード
        {
            Debug.Log("dash");
            myCamera.transform.localPosition = rCAMERA_POS_FPS[(int)carType];
            myCamera.transform.localRotation = Quaternion.Euler(rCAMERA_ROT_FPS[(int)carType]);
        }
        else // 3人称
        {
            Debug.Log("3ninsho");
            myCamera.transform.localPosition = rCAMERA_POS_TPS;
            myCamera.transform.localRotation = Quaternion.Euler(rCAMERA_ROT_TPS);
        }
    }

    /* ======================================================================= *
     * 重力変更
     * ======================================================================= */
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Walls")
        {
            rB.mass = rCAR_MASS[(int)carType] * 10;
        }
    }
    void OnCollisionExit(Collision col)
    {
        if (col.gameObject.tag == "Walls")
        {
            rB.mass = rCAR_MASS[(int)carType];
        }
    }

}

