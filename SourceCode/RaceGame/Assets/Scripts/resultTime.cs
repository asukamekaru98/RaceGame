using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class resultTime : MonoBehaviour
{
    /*-----------------------------------スクリプト-----------------------------------*/
    //ゲームメインマネージャー
    private GameMainManager gM;

    /*---------------列挙型---------------*/
    private enum UIStatus
    {
        LOAD,           // 画像読み込み
        FINISHANIME,    // 「FINISH」のスライド出現
        BG_BLACK,       // 背景（半透明黒）の出現
        S1ANIME,        // 「SECTION1」のスライド出現
        S2ANIME,        // 「SECTION2」のスライド出現
        S3ANIME,        // 「SECTION3」のスライド出現
        S4ANIME,        // 「SECTION4」のスライド出現
        TIMEANIME,      // 「TOTAL TIME」のスライド出現
        MAXSPEEDANIME,  // 「最高速度」のスライド出現
        PUSHUIANIME,    // 「PUSH TO A BUTTON」のスライド出現
        PUSHUIBLINK,    // 「PUSH TO A BUTTON」の点滅
        BG_WHITE,       // 背景（半透明白）の出現
    }
    UIStatus statusUI;

    /*---------------インスペクターに表示させるオブジェクトや変数等---------------*/
    [Header("非表示になるオブジェクト")]
    [SerializeField]
    private GameObject AwayText;
    [SerializeField]
    private GameObject SectionTimeImg;

    /*-----------------------------------定数-----------------------------------*/
    private GameObject finishUIFront, finishUIBack;// 「FINISH」のテキストUI
    private GameObject bGImgBlack, bGImgWhite;// 背景img
    private GameObject section1UI, section2UI, section3UI, section4UI;// セクションタイムUI
    private GameObject totalTimeUI;// トータルタイムUI
    private GameObject maxSpeedUI;// 最大速度UI
    private GameObject pushTxtUI;// 「PUSH TO A BUTTON」のテキストUI

    private Text section1Txt;
    private Text section2Txt;
    private Text section3Txt;
    private Text section4Txt;
    private Text totaltimeTxt;
    private Text maxSpeedTxt;

    private Sprite FinishImg, GameOverImg;

    private const float FIRST_END_POS_NUM = -300;//数字系UIの最初の位置
    private const float LAST_END_POS_NUM = -510;//数字系UIの最後の位置

    private const float FIRST_END_POS_TXTS = 210;//テキスト系UIの最初の位置
    private const float LAST_END_POS_TXTS = 0;//テキスト系UIの最後の位置

    private const float FIRST_MOVE_SPEED = 100;//最初の移動速度
    private const float LAST_MOVE_SPEED = 90;//最後の移動速度

    private const float TEXT_BLING_SPEED = 0.05f;// 文字の点滅速度

    private const float BG_IMG_SPEED = 0.02f;// 背景が薄くなる速度

    private const float FRONT_FINISH_IMG_MAX = 1;// 最初の画像の最大の大きさ
    private const float BACK_FINISH_IMG_MAX = 40;// 最後の画像の最大の大きさ

    private const float FIRST_FINISH_IMG_SPEED = 0.05f;// 最初の画像の拡大速度
    private const float LAST_FINISH_IMG_SPEED = 0.5f;// 最後の画像の拡大速度

    private const float LAST_FINISH_IMG_ALPHA_SPEED = 0.05f;// 最後の画像の薄くなる速度

    private const float ALPHA_MAX = 1.0f;// アルファ値の最大
    private const float ALPHA_MIN = 0.2f;// アルファ値の最小

    /*-----------------------------------変数-----------------------------------*/
    private bool isLRFlg = true;
    private bool isblingFlg;// 点滅で濃ゆくなるか薄くなるかのフラグ

    private float pushImgAlpha;// イメージ(PUSH TO A BUTTON)点滅のためのアルファ値
    private float bGWhiteAlpha;// バックグラウンド（半透明白）のアルファ値
    private float bGBlackAlpha;// バックグラウンド（半透明黒）のアルファ値
    private float finishImgAlpha;// イメージ(FINISH)のアルファ値



    /* ======================================================================= *
     * 初期化
     * ======================================================================= */
    void Start()
    {
        //ゲームメインマネージャーの読み込み
        gM = GameObject.Find("GameMainManager").GetComponent<GameMainManager>();

        statusUI = UIStatus.LOAD;//ロードから始める

        pushImgAlpha = ALPHA_MAX;//テキスト(PUSH TO A BUTTON)を最大の濃ゆさにする

        bGBlackAlpha =   //バックグラウンド（半透明黒）を見えなくさせる
        bGWhiteAlpha = 0;//バックグラウンド（半透明白）を見えなくさせる

        finishImgAlpha = 1;//フィニッシュイメージを最大の濃ゆさにする

        //各オブジェクトのローディング
        finishUIFront = GameObject.Find("ImgFinishFront");
        finishUIBack = GameObject.Find("ImgFinishBack");
        bGImgBlack = GameObject.Find("BackGroundImage_BLACK");
        bGImgWhite = GameObject.Find("BackGroundImage_WHITE");
        section1UI = GameObject.Find("section1");
        section2UI = GameObject.Find("section2");
        section3UI = GameObject.Find("section3");
        section4UI = GameObject.Find("section4");
        totalTimeUI = GameObject.Find("totaltime");
        maxSpeedUI = GameObject.Find("masspeed");
        pushTxtUI = GameObject.Find("pushbutton");

        FinishImg = Resources.Load("Sprites/finish", typeof(Sprite)) as Sprite;
        GameOverImg = Resources.Load("Sprites/gameover", typeof(Sprite)) as Sprite;

        section1Txt = section1UI.transform.Find("section1time").gameObject.GetComponent<Text>();
        section2Txt = section2UI.transform.Find("section2time").gameObject.GetComponent<Text>();
        section3Txt = section3UI.transform.Find("section3time").gameObject.GetComponent<Text>();
        section4Txt = section4UI.transform.Find("section4time").gameObject.GetComponent<Text>();
        totaltimeTxt = totalTimeUI.transform.Find("totaltimetime").gameObject.GetComponent<Text>();
        maxSpeedTxt = maxSpeedUI.transform.Find("maxspeedspeed").gameObject.GetComponent<Text>();

        finishUIFront.SetActive(false);
        finishUIBack.SetActive(false);
    }

    /* ======================================================================= *
     * アプデ
     * ======================================================================= */
    private void Update()
    {
        switch (gM.carStatus)
        {
            case GameMainManager.Status.GAMEOVER:
            case GameMainManager.Status.GOAL:
                AwayText.SetActive(false);
                SectionTimeImg.SetActive(false);
                break;
        }
    }

    /* ======================================================================= *
     * リサルトタイムのアニメーション
     * ======================================================================= */
    public bool ResultTimeAnime(bool isStatus)
    {
        switch (statusUI)
        {
            /*--------------------------- = 画像読み込み = ---------------------------*/
            case UIStatus.LOAD:

                Vector3 Pos;

                section1Txt.text = gM.sectionTimes[0, 0].ToString("00") + "'" + gM.sectionTimes[0, 1].ToString("00") + "\"" + gM.sectionTimes[0, 2].ToString("000");
                section2Txt.text = gM.sectionTimes[1, 0].ToString("00") + "'" + gM.sectionTimes[1, 1].ToString("00") + "\"" + gM.sectionTimes[1, 2].ToString("000");
                section3Txt.text = gM.sectionTimes[2, 0].ToString("00") + "'" + gM.sectionTimes[2, 1].ToString("00") + "\"" + gM.sectionTimes[2, 2].ToString("000");
                section4Txt.text = gM.sectionTimes[3, 0].ToString("00") + "'" + gM.sectionTimes[3, 1].ToString("00") + "\"" + gM.sectionTimes[3, 2].ToString("000");
                totaltimeTxt.text = gM.totalTimes[0, 0].ToString("00") + "'" + gM.totalTimes[0, 1].ToString("00") + "\"" + gM.totalTimes[0, 2].ToString("000");
                maxSpeedTxt.text = gM.maxSpeed.ToString("#") + "k/m";

                finishUIFront.SetActive(true);
                finishUIBack.SetActive(true);

                var fuf = finishUIFront.GetComponent<RectTransform>();
                var fub = finishUIBack.GetComponent<RectTransform>();

                if (isStatus)
                {
                    finishUIFront.GetComponent<Image>().sprite = FinishImg;
                    finishUIBack.GetComponent<Image>().sprite = FinishImg;
                    fuf.sizeDelta = fub.sizeDelta = new Vector2(1000, 205);
                }
                else
                {
                    finishUIFront.GetComponent<Image>().sprite = GameOverImg;
                    finishUIBack.GetComponent<Image>().sprite = GameOverImg;
                    fuf.sizeDelta = fub.sizeDelta = new Vector2(906, 126);
                }

                statusUI = UIStatus.FINISHANIME;
                break;
            /*--------------------------- = FINISHテキストのアニメーション = ---------------------------*/
            case UIStatus.FINISHANIME:

                //前後のイメージを拡大
                if (finishUIFront.transform.localScale.x < FRONT_FINISH_IMG_MAX)
                {
                    finishUIFront.transform.localScale = finishUIBack.transform.localScale += new Vector3(FIRST_FINISH_IMG_SPEED, FIRST_FINISH_IMG_SPEED, 0);
                }
                //前のイメージを拡大し続ける（薄い方）
                else if (finishUIFront.transform.localScale.x < BACK_FINISH_IMG_MAX)
                {
                    finishUIFront.transform.localScale += new Vector3(LAST_FINISH_IMG_SPEED, LAST_FINISH_IMG_SPEED, 0);
                }
                //後ろのイメージを薄くして、前のイメージを消す
                else if (finishImgAlpha > 0)
                {
                    //前のイメージを消す
                    finishUIFront.SetActive(false);
                    //アルファ値を増やす
                    finishImgAlpha -= LAST_FINISH_IMG_ALPHA_SPEED;
                    //アルファ値を設定する
                    finishUIBack.GetComponent<CanvasRenderer>().SetAlpha(finishImgAlpha);
                }
                else
                {
                    //イメージを消す
                    Destroy(finishUIFront);
                    Destroy(finishUIBack);
                    //次のアニメーションに移る
                    statusUI = UIStatus.BG_BLACK;
                }

                break;

            /*--------------------------- = バックグラウンド（半透明黒）の表示 = ---------------------------*/
            case UIStatus.BG_BLACK:

                float BLACK = 0;

                //濃ゆくなる(半透明にさせる)
                if (bGBlackAlpha <= ALPHA_MAX / 2)
                {
                    bGBlackAlpha += BG_IMG_SPEED;//アルファ値を増やす
                }
                else
                {
                    //次のアニメーションに移る
                    statusUI = UIStatus.S1ANIME;
                }

                bGImgBlack.GetComponent<Image>().color = new Color(BLACK, BLACK, BLACK, bGBlackAlpha);

                break;

            /*--------------------------- = SECTION1のアニメーション = ---------------------------*/
            case UIStatus.S1ANIME:

                Pos = section1UI.transform.localPosition;
                section1UI.transform.localPosition = new Vector3(SlideAnime(FIRST_END_POS_NUM, LAST_END_POS_NUM, FIRST_MOVE_SPEED, LAST_MOVE_SPEED, Pos.x), Pos.y, Pos.z);

                //次のアニメーションに移る
                if (Pos.x == LAST_END_POS_NUM && !isLRFlg)
                {
                    isLRFlg = true;//右に移動させるための初期化
                    statusUI = UIStatus.S2ANIME;
                }
                break;

            /*--------------------------- = SECTION2のアニメーション = ---------------------------*/
            case UIStatus.S2ANIME:

                Pos = section2UI.transform.localPosition;
                section2UI.transform.localPosition = new Vector3(SlideAnime(FIRST_END_POS_NUM, LAST_END_POS_NUM, FIRST_MOVE_SPEED, LAST_MOVE_SPEED, Pos.x), Pos.y, Pos.z);

                //次のアニメーションに移る
                if (Pos.x == LAST_END_POS_NUM && !isLRFlg)
                {
                    isLRFlg = true;//右に移動させるための初期化
                    statusUI = UIStatus.S3ANIME;
                }
                break;

            /*--------------------------- = SECTION3のアニメーション = ---------------------------*/
            case UIStatus.S3ANIME:
                Pos = section3UI.transform.localPosition;
                section3UI.transform.localPosition = new Vector3(SlideAnime(FIRST_END_POS_NUM, LAST_END_POS_NUM, FIRST_MOVE_SPEED, LAST_MOVE_SPEED, Pos.x), Pos.y, Pos.z);

                //次のアニメーションに移る
                if (Pos.x == LAST_END_POS_NUM && !isLRFlg)
                {
                    isLRFlg = true;//右に移動させるための初期化
                    statusUI = UIStatus.S4ANIME;
                }
                break;

            /*--------------------------- = SECTION4のアニメーション = ---------------------------*/
            case UIStatus.S4ANIME:

                Pos = section4UI.transform.localPosition;
                section4UI.transform.localPosition = new Vector3(SlideAnime(FIRST_END_POS_NUM, LAST_END_POS_NUM, FIRST_MOVE_SPEED, LAST_MOVE_SPEED, Pos.x), Pos.y, Pos.z);

                //次のアニメーションに移る
                if (Pos.x == LAST_END_POS_NUM && !isLRFlg)
                {
                    isLRFlg = true;//右に移動させるための初期化
                    statusUI = UIStatus.TIMEANIME;
                }
                break;

            /*--------------------------- = TIMEのアニメーション = ---------------------------*/
            case UIStatus.TIMEANIME:
                Pos = totalTimeUI.transform.localPosition;
                totalTimeUI.transform.localPosition = new Vector3(SlideAnime(FIRST_END_POS_NUM, LAST_END_POS_NUM, FIRST_MOVE_SPEED, LAST_MOVE_SPEED, Pos.x), Pos.y, Pos.z);

                //次のアニメーションに移る
                if (Pos.x == LAST_END_POS_NUM && !isLRFlg)
                {
                    isLRFlg = true;//右に移動させるための初期化
                    statusUI = UIStatus.MAXSPEEDANIME;
                }
                break;

            /*--------------------------- = 最高速のアニメーション = ---------------------------*/
            case UIStatus.MAXSPEEDANIME:
                Pos = maxSpeedUI.transform.localPosition;
                maxSpeedUI.transform.localPosition = new Vector3(SlideAnime(FIRST_END_POS_NUM, LAST_END_POS_NUM, FIRST_MOVE_SPEED, LAST_MOVE_SPEED, Pos.x), Pos.y, Pos.z);

                //次のアニメーションに移る
                if (Pos.x == LAST_END_POS_NUM && !isLRFlg)
                {
                    isLRFlg = true;//右に移動させるための初期化
                    statusUI = UIStatus.PUSHUIANIME;
                }
                break;

            /*--------------------------- = 「PUSH TO A BUTTON」テキストのアニメーション = ---------------------------*/
            case UIStatus.PUSHUIANIME:

                Pos = pushTxtUI.transform.localPosition;
                pushTxtUI.transform.localPosition = new Vector3(SlideAnime(FIRST_END_POS_TXTS, LAST_END_POS_TXTS, FIRST_MOVE_SPEED, LAST_MOVE_SPEED, Pos.x), Pos.y, Pos.z);

                //次のアニメーションに移る
                if (Pos.x == LAST_END_POS_TXTS && !isLRFlg)
                {
                    isLRFlg = true;//右に移動させるための初期化
                    statusUI = UIStatus.PUSHUIBLINK;
                }
                break;

            /*--------------------------- = バックグラウンド（半透明白）の表示アニメーション = ---------------------------*/
            case UIStatus.PUSHUIBLINK:

                //点滅：濃ゆくなる
                if (isblingFlg && pushImgAlpha <= ALPHA_MAX)
                {
                    pushImgAlpha += TEXT_BLING_SPEED;//アルファ値を増やす
                }
                else
                {
                    isblingFlg = false;//上限に来たら減らす
                }

                //点滅：薄くなる
                if (!isblingFlg && pushImgAlpha > ALPHA_MIN)
                {
                    pushImgAlpha -= TEXT_BLING_SPEED;//アルファ値を減らす
                }
                else
                {
                    isblingFlg = true;//上限に来たら増やす
                }

                if (Input.anyKeyDown)
                {
                    statusUI = UIStatus.BG_WHITE;
                    pushImgAlpha = ALPHA_MAX;
                }

                //アルファ値を設定する
                pushTxtUI.GetComponent<CanvasRenderer>().SetAlpha(pushImgAlpha);

                break;

            /*--------------------------- = バックグラウンド（半透明白）の表示アニメーション = ---------------------------*/
            case UIStatus.BG_WHITE:

                float WHITE = 255;

                //濃ゆくなる
                if (bGWhiteAlpha <= ALPHA_MAX)
                {
                    bGWhiteAlpha += BG_IMG_SPEED;//アルファ値を増やす
                }
                else
                {
                    return true;
                }

                bGImgWhite.GetComponent<Image>().color = new Color(WHITE, WHITE, WHITE, bGWhiteAlpha);
                break;
        }
        return false;
    }

    /* ======================================================================= *
     * アニメーション
     * スライド
     * 右に多めに移動したあと左の移動して定位置に戻る
     * ======================================================================= */
    float SlideAnime(float endPos1, float endPos2, float moveSpeed1, float moveSpeed2, float nowPos)
    {
        //右に移動
        if (isLRFlg)
        {
            if (nowPos < endPos1)
            {
                nowPos += moveSpeed1;
            }
            else
            {
                isLRFlg = false;
            }
        }
        //左に移動
        else
        {
            if (nowPos > endPos2)
            {
                nowPos -= moveSpeed2;
            }
            else
            {
                nowPos = endPos2;
            }
        }

        return nowPos;
    }
}
