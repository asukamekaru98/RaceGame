using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class needleScript : MonoBehaviour
{
    private float nowNeedleRota;
    private float minNumRotaZ;
    private float maxNumRotaZ;
    private float maxNeedleNum;

    // Use this for initialization
    public void getStart(float min, float max, float needle)
    {
        minNumRotaZ = min;
        maxNumRotaZ = max;
        maxNeedleNum = needle;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Debug.Log(transform.name + ":" + nowNeedleRota);
       // nowNeedleRota = 124.8f - (0.0226222f * nowNeedleRota);

        this.transform.rotation = Quaternion.Euler(0, 0, minNumRotaZ - (((minNumRotaZ - maxNumRotaZ) / maxNeedleNum) * nowNeedleRota));
    }

    //値の取得
    public void getNum(float getNum) { nowNeedleRota = getNum;}
}
