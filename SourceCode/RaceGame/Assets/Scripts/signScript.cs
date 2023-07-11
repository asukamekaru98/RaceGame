using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class signScript : MonoBehaviour
{
    /*-----------------------------------スクリプト-----------------------------------*/
    private SoundManager sM;//サウンドマネージャー
    private CommonVariable cV;//共通変数

    /*-----------------------------------画像-----------------------------------*/
    private readonly Sprite[] IMAGE_SIGN = new Sprite[7];//画像

    /*-----------------------------------サウンド-----------------------------------*/
    private AudioClip soundBlingSign;//点滅サイン
    private AudioClip soundSign;//普通のサイン

    /*---------------インスペクターに表示させるオブジェクトや変数等---------------*/

    [Header("サインを表示させるオブジェクト")]
    [SerializeField]
    private GameObject collGameObj;

    /*-----------------------------------定数-----------------------------------*/

    private const short RIGHT_ARROW = 0;
    private const short LEFT_ARROW = 180;

    private const float EXPANDING_SPEED = 0.05f;//拡大速度
    private const float TEXT_BLING_SPEED = 0.1f;// 文字の点滅速度

    private const float ALPHA_MAX = 1.0f;// アルファ値の最大
    private const float ALPHA_MIN = 0.2f;// アルファ値の最小

    private const float IMAGE_WH_MAX = 0.3f;//画像の最大の大きさ
    /*-----------------------------------変数-----------------------------------*/
    private string witchSign;//どのサインか、区域のオブジェクトから取得、保存

    private float imageAlpha;// テキスト点滅のための数字
    private float imageScale;// テキスト拡大のための数字

    private bool isFlip;//反転させるか
    private bool isActive;//サインを実行させるか
    private bool isblink;//サインを実行させるか
    private bool isblingFlg;// 点滅で濃ゆくなるか薄くなるかのフラグ
    private bool oneShotSoundBling;//一度だけ音を鳴らす (点滅サイン)

    private float Exp;//拡大率

    /* ======================================================================= *
     * 初期化
     * ======================================================================= */
    void Start()
    {
        //スクリプト読み込み
        cV = GameObject.Find("CommonVariable").GetComponent<CommonVariable>();
        sM = GameObject.Find("SoundManager").GetComponent<SoundManager>();

        //サウンド読み込み
        soundSign = Resources.Load("Sounds/sign", typeof(AudioClip)) as AudioClip;//ポーン
        soundBlingSign = Resources.Load("Sounds/blingSign", typeof(AudioClip)) as AudioClip;//ピン

        IMAGE_SIGN[0] = Resources.Load("Sprites/shortCurve", typeof(Sprite)) as Sprite;
        IMAGE_SIGN[1] = Resources.Load("Sprites/steepCurve", typeof(Sprite)) as Sprite;
        IMAGE_SIGN[2] = Resources.Load("Sprites/smallCurve", typeof(Sprite)) as Sprite;
        IMAGE_SIGN[3] = Resources.Load("Sprites/steepWindingCurve", typeof(Sprite)) as Sprite;
        IMAGE_SIGN[4] = Resources.Load("Sprites/smallWindingCurve", typeof(Sprite)) as Sprite;
        IMAGE_SIGN[5] = Resources.Load("Sprites/hairpinCurve", typeof(Sprite)) as Sprite;
        IMAGE_SIGN[6] = Resources.Load("Sprites/caution", typeof(Sprite)) as Sprite;

        imageAlpha = ALPHA_MIN;//アルファの初期化
        imageScale = 0;//スケールの初期化
        isActive = false;//画像を非表示にさせる

        if (cV.gameMode == CommonVariable.GameMode.BATTLE) Exp = 0.5f;
        else Exp = 1;

    }

    /* ======================================================================= *
     * アプデ
     * ======================================================================= */
    void Update()
    {
        collGameObj.SetActive(isActive);//画像の表示or非表示
    }

    /* ======================================================================= *
     * アプデ
     * ======================================================================= */
    void FixedUpdate()
    {
        //サインの反転をする
        if (isFlip) collGameObj.transform.rotation = Quaternion.Euler(0, LEFT_ARROW, 0);
        else collGameObj.transform.rotation = Quaternion.Euler(0, RIGHT_ARROW, 0);

        if (isActive)
        {
            //点滅させるか
            if (isblink)
            {
                //点滅アニメーション
                blinkAnimation();
                //
                imageScale = IMAGE_WH_MAX;
            }
            else
            {
                //拡大アニメーション
                expandingAnimation();
            }
        }
        else
        {
            //初期化
            imageScale = 0;
            imageAlpha = ALPHA_MIN;//アルファの初期化
        }

        collGameObj.transform.localScale = new Vector3(imageScale * Exp, imageScale * Exp, 1);
    }

    /* ======================================================================= *
     * 点滅アニメーション
     * ======================================================================= */
    void blinkAnimation()
    {
        //点滅：濃ゆくなる
        if (isblingFlg && imageAlpha <= ALPHA_MAX)
        {
            imageAlpha += TEXT_BLING_SPEED;//アルファ値を増やす
        }
        else
        {
            //SEを鳴らす
            if (oneShotSoundBling)
            {
                sM.SEPlay(soundBlingSign, false, 1);// SE:ピン
            }

            oneShotSoundBling =
            isblingFlg = false;//上限に来たら減らす
        }

        //点滅：薄くなる
        if (!isblingFlg && imageAlpha > ALPHA_MIN)
        {
            imageAlpha -= TEXT_BLING_SPEED;//アルファ値を減らす
        }
        else
        {
            oneShotSoundBling =
            isblingFlg = true;//上限に来たら増やす
        }

        //アルファ値を設定する
        collGameObj.GetComponent<CanvasRenderer>().SetAlpha(imageAlpha);
        collGameObj.transform.localScale = new Vector3(imageScale * Exp, imageScale * Exp, 1);
    }

    /* ======================================================================= *
     * 拡大アニメーション
     * ======================================================================= */
    void expandingAnimation()
    {
        if (imageScale < IMAGE_WH_MAX)
        {
            imageScale += EXPANDING_SPEED;
        }
        else
        {
            if (oneShotSoundBling)
            {
                //音を鳴らす
                sM.SEPlay(soundSign, false, 1);// SE:ポーン

                //一度だけ鳴らすようにする
                oneShotSoundBling = false;
            }
            imageScale = IMAGE_WH_MAX;
        }
    }

    /* ======================================================================= *
     * 当たり判定
     * ======================================================================= */
    void OnTriggerEnter(Collider other)
    {
        //サインの始まりに来たら
        if (other.gameObject.tag == "StartSign")
        {
            //この区域のサインを獲得
            witchSign = other.transform.parent.gameObject.GetComponent<singSwitchScript>().attribute();
            isFlip = other.transform.parent.gameObject.GetComponent<singSwitchScript>().isFlipHorizontal();

            //画像の表示化
            isActive = true;
            //音を鳴らせるようにする
            //oneShotSoundBling = true;

            //画像を変える
            switch (witchSign)
            {
                case "SHORT_CURVE":
                    isblink = false;//点滅しない
                    oneShotSoundBling = true;
                    collGameObj.gameObject.GetComponent<Image>().sprite = IMAGE_SIGN[0];
                    break;

                case "STEEP_CURVE":
                    isblink = false;//点滅しない
                    oneShotSoundBling = true;
                    collGameObj.gameObject.GetComponent<Image>().sprite = IMAGE_SIGN[1];
                    break;

                case "SMALL_CURVE":
                    isblink = false;//点滅しない
                    oneShotSoundBling = true;
                    collGameObj.gameObject.GetComponent<Image>().sprite = IMAGE_SIGN[2];
                    break;

                case "STEEP_WINDING_CURVE":
                    isblink = false;//点滅しない
                    oneShotSoundBling = true;
                    collGameObj.gameObject.GetComponent<Image>().sprite = IMAGE_SIGN[3];
                    break;

                case "SMALL_WINDING_CURVE":
                    isblink = false;//点滅しない
                    oneShotSoundBling = true;
                    collGameObj.gameObject.GetComponent<Image>().sprite = IMAGE_SIGN[4];
                    break;

                case "HAIRPIN_CURVE":
                    isblink = true;//点滅する
                    collGameObj.gameObject.GetComponent<Image>().sprite = IMAGE_SIGN[5];
                    break;

                case "CAUTION":
                    isblink = true;//点滅する
                    collGameObj.gameObject.GetComponent<Image>().sprite = IMAGE_SIGN[6];
                    break;
            }
        }

        //サインの終わりにきたら
        if (other.gameObject.tag == "EndSign")
        {
            //画像の非表示化
            isActive = false;
        }
    }
}
