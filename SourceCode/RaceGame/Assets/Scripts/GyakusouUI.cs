using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GyakusouUI : MonoBehaviour
{

    /*-----------------------------------スクリプト-----------------------------------*/
    private SoundManager sM;
    private GameMainManager gmM;

    /*-----------------------------------サウンド-----------------------------------*/
    private AudioClip soundWhoop;// SE:サイレン

    /*---------------インスペクターに表示させるオブジェクトや変数等---------------*/
    [Header("カウントダウンさせるためのスプライト")]
    [SerializeField]
    private List<Sprite> CountDownImage;
    [Header("カウントダウンさせるための前方のゲームオブジェクト（半透明）")]
    [SerializeField]
    private GameObject FrontImage;
    [Header("カウントダウンさせるための後方のゲームオブジェクト")]
    [SerializeField]
    private GameObject BackImage;
    [Header("「逆走」画像")]
    [SerializeField]
    private GameObject GyakusouImg;

    /*---------------列挙型---------------*/
    private enum IMG_WH
    {
        IMG_WEIGHT, IMG_HEIGHT
    }

    /*-----------------------------------配列-----------------------------------*/

    private readonly float[,] rIMG_SIZE = new float[,]//各画像の解像度
    {
        { 102, 220},//画像:１
        { 216, 220},//画像:２
        { 209, 220},//画像:３
    };

    /*-----------------------------------定数-----------------------------------*/

    const short DEFAULT_NUM_SIZE = 1;// デフォルトのカウントダウン画像のサイズ
    const float EXP_SPEED = 0.01f;// 拡大速度
    const float REDU_SPEED = 0.21f;// 縮小速度
    const float REDU_TIME = 0.8f;// 戻る時間
    const float TEXT_FLASHING_SPEED = 0.1f;// 文字の点滅速度
    const float ALPHA_MAX = 1.0f;// アルファ値の最大
    const float ALPHA_MIN = 0.2f;// アルファ値の最小

    const short COUND_DOWN_MAX = 3;//カウント数

    /*---------------変数---------------*/
    private float totalTime = 3;// カウントダウンの時間
    private float frontImageScale = 0;// 前方イメージの大きさ
    private float backImageScale = 0;// 後方イメージの大きさ

    private float textAlpha = 1;// テキスト点滅のための数字

    private int countSoundNum;

    private bool flashingFlg;// 点滅で濃ゆくなるか薄くなるかのフラグ

    private bool first = true;

    /* ======================================================================= *
     * アプデ
     * 動的なもので使用
     * ======================================================================= */
    void Start()
    {
        //画像を初期（３）にする
        FrontImage.gameObject.GetComponent<Image>().sprite =
        BackImage.gameObject.GetComponent<Image>().sprite = CountDownImage[COUND_DOWN_MAX - 1];

        countSoundNum = (int)totalTime;

        //現在のアルファ値を取得
        textAlpha = GyakusouImg.GetComponent<CanvasRenderer>().GetAlpha();

        //スクリプト読み込み
        sM = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        gmM = GameObject.Find("GameMainManager").GetComponent<GameMainManager>();

        //サウンド読み込み
        soundWhoop = Resources.Load("Sounds/beepbeep", typeof(AudioClip)) as AudioClip;// SE:サイレン 

        first = false;
        gameObject.SetActive(false);
    }

    /* ======================================================================= *
     * 画像表示時に初期化させる
     * ======================================================================= */
    void OnEnable()
    {
        //時間を戻す
        totalTime = COUND_DOWN_MAX+1;

        frontImageScale = 0;// 前方イメージの大きさ
        backImageScale = 0;// 後方イメージの大きさ
        
        //画像を初期（３）にする
        FrontImage.gameObject.GetComponent<Image>().sprite =
            BackImage.gameObject.GetComponent<Image>().sprite = CountDownImage[COUND_DOWN_MAX - 1];

        //解像度変更
        var rtf = FrontImage.GetComponent<RectTransform>();
        var rtb = BackImage.GetComponent<RectTransform>();
        rtf.sizeDelta = rtb.sizeDelta = new Vector2(rIMG_SIZE[(int)COUND_DOWN_MAX - 1, (int)IMG_WH.IMG_WEIGHT], rIMG_SIZE[(int)COUND_DOWN_MAX - 1, (int)IMG_WH.IMG_HEIGHT]);

        if(!first)sM.WhoopPlay(true);// SE:ポン
    }

    void OnDisable()
    {
        sM.WhoopPlay(false);// SE:ポン
        FrontImage.SetActive(false);
        BackImage.SetActive(false);
    }

    /* ======================================================================= *
     * フィックストアプデ
     * 動的なもので使用
     * ======================================================================= */
    void FixedUpdate()
    {
        //カウントダウンさせる
        if (totalTime != 0) totalTime -= Time.deltaTime;

        TextFlashing();

        if(totalTime > COUND_DOWN_MAX)
        {

        }
        //カウントダウンのイメージを表示
        else if (totalTime >= 0)
        {
            //毎秒確認
            float everySecond = (1 - (totalTime - (int)totalTime));

            FrontImage.SetActive(true);
            BackImage.SetActive(true);

            //画像の大きさ変更
            if (everySecond < REDU_TIME)
            {
                //縮小時間まで前方、後方を拡大
                frontImageScale = backImageScale += EXP_SPEED;

            }
            else if (everySecond < 0.90f)
            {
                frontImageScale += EXP_SPEED;// 前方は拡大し続ける
                backImageScale -= REDU_SPEED;// 後方は縮小する
            }
            else
            {
                //前方、後方の初期化
                frontImageScale = backImageScale = 0;

                if ((int)totalTime > 0)
                {
                    //画像を変える
                    FrontImage.gameObject.GetComponent<Image>().sprite =
                    BackImage.gameObject.GetComponent<Image>().sprite = CountDownImage[(int)totalTime - 1];

                    var rtf = FrontImage.GetComponent<RectTransform>();
                    var rtb = BackImage.GetComponent<RectTransform>();

                    //var sizeDeltaF = rtf.sizeDelta;
                    //var sizeDeltaB = rtb.sizeDelta;
                    rtf.sizeDelta = rtb.sizeDelta = new Vector2(rIMG_SIZE[(int)totalTime - 1, (int)IMG_WH.IMG_WEIGHT], rIMG_SIZE[(int)totalTime - 1, (int)IMG_WH.IMG_HEIGHT]);
                    // = sizeDelta;
                }
                else
                {
                    //画像を消去する
                    FrontImage.SetActive(false);
                    BackImage.SetActive(false);
                }
            }

            //画像の拡大縮小
            FrontImage.transform.localScale = new Vector2(DEFAULT_NUM_SIZE + frontImageScale, DEFAULT_NUM_SIZE + frontImageScale);
            BackImage.transform.localScale = new Vector2(DEFAULT_NUM_SIZE + backImageScale, DEFAULT_NUM_SIZE + backImageScale);
        }
        else
        {
            gmM.carStatus = GameMainManager.Status.GAMEOVER;
        }
    }

    /* ======================================================================= *
     * 画像点滅
     * ======================================================================= */
    void TextFlashing()
    {
        //点滅：濃ゆくなる
        if (flashingFlg && textAlpha <= ALPHA_MAX)
        {
            textAlpha += TEXT_FLASHING_SPEED;//アルファ値を増やす
        }
        else
        {
            flashingFlg = false;//上限に来たら減らす
        }

        //点滅：薄くなる
        if (!flashingFlg && textAlpha > ALPHA_MIN)
        {
            textAlpha -= TEXT_FLASHING_SPEED;//アルファ値を減らす
        }
        else
        {
            flashingFlg = true;//上限に来たら増やす
        }

        //アルファ値を設定する
        GyakusouImg.GetComponent<CanvasRenderer>().SetAlpha(textAlpha);
    }
}
