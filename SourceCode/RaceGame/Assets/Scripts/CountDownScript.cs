using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountDownScript : MonoBehaviour
{
    /*-----------------------------------スクリプト-----------------------------------*/
    private SoundManager sM;

    /*-----------------------------------サウンド-----------------------------------*/
    private AudioClip soundPon;// ポン
    private AudioClip soundPoon;// ポーン

    /*---------------列挙型---------------*/
    private enum IMG_WH
    {
        IMG_WEIGHT, IMG_HEIGHT
    }

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
    [Header("点滅テキスト")]
    [SerializeField]
    private GameObject FlashingImage;

    /*-----------------------------------配列-----------------------------------*/

    private readonly float[,] rIMG_SIZE = new float[,]//各画像の解像度
    {
        { 491, 220},//画像:GO!
        { 102, 220},//画像:１
        { 216, 220},//画像:２
        { 209, 220},//画像:３
    };

    /*-----------------------------------定数-----------------------------------*/

    const short DEFAULT_NUM_SIZE = 1;// デフォルトのカウントダウン画像のサイズ
    const float EXP_SPEED = 0.05f;// 拡大速度
    const float REDU_SPEED = 0.21f;// 縮小速度
    const float REDU_TIME = 0.8f;// 戻る時間
    const float TEXT_FLASHING_SPEED = 0.05f;// 文字の点滅速度
    const float ALPHA_MAX = 1.0f;// アルファ値の最大
    const float ALPHA_MIN = 0.2f;// アルファ値の最小

    /*---------------変数---------------*/
    private float totalTime = 5;// カウントダウンの時間
    private float frontImageScale = 0;// 前方イメージの大きさ
    private float backImageScale = 0;// 後方イメージの大きさ

    private float textAlpha = 1;// テキスト点滅のための数字

    private int countSoundNum;

    private bool flashingFlg;// 点滅で濃ゆくなるか薄くなるかのフラグ

    /* ======================================================================= *
    * 初期化
    * ======================================================================= */
    void Start()
    {
        //画像を初期（３）にする
        FrontImage.gameObject.GetComponent<Image>().sprite =
        BackImage.gameObject.GetComponent<Image>().sprite = CountDownImage[3];

        countSoundNum = (int)totalTime;

        //現在のアルファ値を取得
        textAlpha = FlashingImage.GetComponent<CanvasRenderer>().GetAlpha();

        //スクリプト読み込み
        sM = GameObject.Find("SoundManager").GetComponent<SoundManager>();

        //サウンド読み込み
        soundPon = Resources.Load("Sounds/pon", typeof(AudioClip)) as AudioClip;//ポン
        soundPoon = Resources.Load("Sounds/poon", typeof(AudioClip)) as AudioClip;//ポーン
    }

    /* ======================================================================= *
     * カウントダウン
     * ======================================================================= */
    public bool CountDown()
    {
        //カウントダウンさせる
        if (totalTime != 0) totalTime -= Time.deltaTime;

        //文字を点滅させる
        if (totalTime > 1) TextFlashing();
        else Destroy(FlashingImage);//カウントダウン１秒で消去

        //カウントダウンのイメージを表示
        if (totalTime <= 4)
        {
            FrontImage.SetActive(true);
            BackImage.SetActive(true);
            
            //SEを鳴らす、１秒に付き１回鳴らしたいので、そのあれこれ
            if (totalTime <= countSoundNum)
            {
                if (countSoundNum > 1)
                {
                    sM.SEPlay(soundPon, false, 0.5f);// SE:ポン
                }
                else
                {
                    sM.SEPlay(soundPoon, false, 0.5f);// SE:ポーン
                }

                --countSoundNum;//１回だけ鳴らすための
            }

            //毎秒確認
            float everySecond = (1 - (totalTime - (int)totalTime));

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
                    Destroy(FrontImage);
                    Destroy(BackImage);
                    return true;
                }
            }

            //画像の拡大縮小
            FrontImage.transform.localScale = new Vector2(DEFAULT_NUM_SIZE + frontImageScale, DEFAULT_NUM_SIZE + frontImageScale);
            BackImage.transform.localScale = new Vector2(DEFAULT_NUM_SIZE + backImageScale, DEFAULT_NUM_SIZE + backImageScale);
        }
        return false;
    }

    //一定時間ごとに点滅
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
        FlashingImage.GetComponent<CanvasRenderer>().SetAlpha(textAlpha);
    }
}
