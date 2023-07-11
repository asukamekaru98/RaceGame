using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class rankUI : MonoBehaviour
{

    /*-----------------------------------スクリプト-----------------------------------*/
    //ゲームメインマネージャー
    private GameMainManager gM;
    private CommonVariable cV;

    /*---------------列挙型---------------*/
    private enum RANKUIStatus
    {
        SETPOS,         // 画像位置
        RANK_EXP,       // RANKを拡大
        RANK_LIGHT_EXP, // RANKの薄い方を拡大
        PTB_FLASH,      // 「PUSH THE BUTTON」の点滅
        BG_WHITE,       // 背景（半透明白）の出現
    }
    RANKUIStatus rankUIStatus;

    /*---------------インスペクターに表示させるオブジェクトや変数等---------------*/
    [SerializeField]
    private GameObject RANK1P, RANK2P, RANK1ST, RANK1ST_LIGHT, RANK2ND, RANK2ND_LIGHT,PUSH_TO_TXT,BG_IMG;

    /*-----------------------------------定数-----------------------------------*/
    const float RANK_POS = 480;//RANK UIのポジション
    const float RANK_MAX_SCALE = 1;//RANK UIの最大拡大位置
    const float RANK_SCALE_SPEED = 0.1f;//RANK UIの拡大速度
    const float RANK_APPER_MAX_SCALE = 6;//RANK UIの薄い方の最大拡大位置

    private const float ALPHA_MAX = 1.0f;// アルファ値の最大
    private const float ALPHA_MIN = 0.2f;// アルファ値の最小

    private const float BG_IMG_SPEED = 0.02f;// 背景が薄くなる速度

    private const float TEXT_BLING_SPEED = 0.05f;// 文字の点滅速度

    /*-----------------------------------変数-----------------------------------*/
    private bool isblingFlg;// 点滅で濃ゆくなるか薄くなるかのフラグ
    private float pushImgAlpha;// イメージ(PUSH TO A BUTTON)点滅のためのアルファ値
    private float bGWhiteAlpha;// バックグラウンド（半透明白）のアルファ値

    /* ======================================================================= *
     * 初期化
     * ======================================================================= */
    void Start()
    {
        //ゲームメインマネージャーの読み込み
        gM = GameObject.Find("GameMainManager").GetComponent<GameMainManager>();
        cV = GameObject.Find("CommonVariable").GetComponent<CommonVariable>();

        rankUIStatus = RANKUIStatus.SETPOS;//ロードから始める

        bGWhiteAlpha = 0;
        pushImgAlpha = ALPHA_MAX;

        RANK1ST.SetActive(false);
        RANK1ST_LIGHT.SetActive(false);
        RANK2ND.SetActive(false);
        RANK2ND_LIGHT.SetActive(false);
        PUSH_TO_TXT.SetActive(false);
        BG_IMG.SetActive(false);
    }

    /* ======================================================================= *
     * ランクUIのアニメーション
     * ======================================================================= */
    public bool RankUIAnime()
    {

        switch (rankUIStatus)
        {
            case RANKUIStatus.SETPOS:

                //UIの表示、POS設定
                RANK1P.SetActive(false);
                RANK2P.SetActive(false);

                RANK1ST.SetActive(true);
                RANK1ST_LIGHT.SetActive(true);
                RANK2ND.SetActive(true);
                RANK2ND_LIGHT.SetActive(true);

                if (cV.Rank1P == 1)
                {
                    RANK1ST.transform.localPosition = RANK1ST_LIGHT.transform.localPosition = new Vector3(-RANK_POS, 0, 0);
                    RANK2ND.transform.localPosition = RANK2ND_LIGHT.transform.localPosition = new Vector3(RANK_POS, 0, 0);
                }
                else if (cV.Rank2P == 1)
                {
                    RANK1ST.transform.localPosition = RANK1ST_LIGHT.transform.localPosition = new Vector3(RANK_POS, 0, 0);
                    RANK2ND.transform.localPosition = RANK2ND_LIGHT.transform.localPosition = new Vector3(-RANK_POS, 0, 0);
                }

                //次に移る
                rankUIStatus = RANKUIStatus.RANK_EXP;

                break;

            case RANKUIStatus.RANK_EXP:

                //RANK UIを拡大
                if (RANK_MAX_SCALE > RANK1ST.transform.localScale.x)
                {
                    RANK1ST.transform.localScale = RANK1ST_LIGHT.transform.localScale += new Vector3(RANK_SCALE_SPEED, RANK_SCALE_SPEED, 0);
                    RANK2ND.transform.localScale = RANK2ND_LIGHT.transform.localScale += new Vector3(RANK_SCALE_SPEED, RANK_SCALE_SPEED, 0);
                }
                else
                {
                    RANK1ST.transform.localScale = RANK1ST_LIGHT.transform.localScale = new Vector3(RANK_MAX_SCALE, RANK_MAX_SCALE, 0);
                    RANK2ND.transform.localScale = RANK2ND_LIGHT.transform.localScale = new Vector3(RANK_MAX_SCALE, RANK_MAX_SCALE, 0);

                    //次に移る
                    rankUIStatus = RANKUIStatus.RANK_LIGHT_EXP;
                }

                break;
            case RANKUIStatus.RANK_LIGHT_EXP:

                //RANK UI薄い方を拡大
                if (RANK_APPER_MAX_SCALE > RANK1ST_LIGHT.transform.localScale.x)
                {
                    RANK1ST_LIGHT.transform.localScale += new Vector3(RANK_SCALE_SPEED, RANK_SCALE_SPEED, 0);
                    RANK2ND_LIGHT.transform.localScale += new Vector3(RANK_SCALE_SPEED, RANK_SCALE_SPEED, 0);
                }
                else
                {
                    RANK1ST_LIGHT.transform.localScale = new Vector3(RANK_APPER_MAX_SCALE, RANK_APPER_MAX_SCALE, 0);
                    RANK2ND_LIGHT.transform.localScale = new Vector3(RANK_APPER_MAX_SCALE, RANK_APPER_MAX_SCALE, 0);

                    //次に移る
                    rankUIStatus = RANKUIStatus.PTB_FLASH;
                    
                    RANK1ST_LIGHT.SetActive(false);
                    RANK2ND_LIGHT.SetActive(false);
                    PUSH_TO_TXT.SetActive(true);
                    BG_IMG.SetActive(true);
                }

                break;
            case RANKUIStatus.PTB_FLASH:
                
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
                    rankUIStatus = RANKUIStatus.BG_WHITE;
                    pushImgAlpha = ALPHA_MAX;
                }

                //アルファ値を設定する
                PUSH_TO_TXT.GetComponent<CanvasRenderer>().SetAlpha(pushImgAlpha);

                break;

            case RANKUIStatus.BG_WHITE:
                
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

                BG_IMG.GetComponent<Image>().color = new Color(WHITE, WHITE, WHITE, bGWhiteAlpha);

                break;
        }
        return false;
    }

}