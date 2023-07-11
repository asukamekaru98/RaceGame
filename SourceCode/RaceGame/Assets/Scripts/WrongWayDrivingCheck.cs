using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrongWayDrivingCheck : MonoBehaviour
{
    //新旧パスナンバー
    public int newPassesNum, oldPassesNum;

    /* ======================================================================= *
     * 初期化
     * ======================================================================= */
    void Start()
    {
        newPassesNum = oldPassesNum = 0;
    }

    /* ======================================================================= *
     * 逆走判定
     * ======================================================================= */
    public bool WWDCheck()
    {
        if (oldPassesNum > newPassesNum) return true;//逆走中
        
        return false;//逆走していない
    }

    /* ======================================================================= *
     * 当たり判定
     * ======================================================================= */
    void OnTriggerEnter(Collider other)
    {
        //サインの始まりに来たら
        if (other.gameObject.tag == "PassesCheck")
        {
            //前のパスナンバーを古くする
            oldPassesNum = newPassesNum;

            newPassesNum = other.GetComponent<PassesCheckPointScript>().myCheckPoint;
            
        }
    }
}
