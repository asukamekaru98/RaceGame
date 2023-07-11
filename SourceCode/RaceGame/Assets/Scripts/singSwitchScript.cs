using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class singSwitchScript : MonoBehaviour
{
    /*---------------インスペクターに表示させるオブジェクトや変数等---------------*/
    [Header("警告の種類")]
    public SignType signType;
    public enum SignType
    {
        SHORT_CURVE,// 短いカーブ 緑の矢印
        STEEP_CURVE,// 急なカーブ 黄色い矢印
        SMALL_CURVE,// 緩いカーブ 水色の矢印
        STEEP_WINDING_CURVE,// 急なくねくね道 赤いくねくね
        SMALL_WINDING_CURVE,// 緩いくねくね道 黄色いくねくね
        HAIRPIN_CURVE,// ヘアピンカーブ 赤いUターン
        CAUTION,// 警告 黄色い感嘆符
    }

    [Header("左右反転させるか")]
    [SerializeField]
    private bool flipHorizontal;

    // 属性取得の
    public string attribute() { return signType.ToString(); }
    // 反転取得
    public bool isFlipHorizontal() { return flipHorizontal; }
}
