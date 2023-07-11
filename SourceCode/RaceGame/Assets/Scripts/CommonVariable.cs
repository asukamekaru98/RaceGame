using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonVariable : MonoBehaviour
{
    /*---------------列挙型---------------*/
    public enum GameMode
    {
        TIMEATTACK, BATTLE
    }
    public GameMode gameMode;

    //1P車
    public enum CarType1P
    {
        Civic, Evo, Celica, Skyline
    }
    public CarType1P carType1P;

    //2P車
    public enum CarType2P
    {
        Civic, Evo, Celica, Skyline
    }
    public CarType2P carType2P;

    /*---------------定数---------------*/
    //タイムアタック用の時間
    public readonly float TIMEATTACK_TIME = 300;
    //バトルモード用の時間
    public readonly float BATTLE_TIME = 600;

    /*---------------変数---------------*/
    public float remainigTime;//残りタイム

    public short Rank1P, Rank2P;//1P,2Pランキング

    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(this);//全シーンで共用
        Rank1P = 1;
        Rank2P = 2;
    }

    // Update is called once per frame
    void Update()
    {
        //アプリを落とす
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
    void Awake()
    {
        // PC向けビルドだったらサイズ変更
        if (Application.platform == RuntimePlatform.WindowsPlayer ||
        Application.platform == RuntimePlatform.OSXPlayer ||
        Application.platform == RuntimePlatform.LinuxPlayer)
        {
            Screen.SetResolution(1920, 1080, false);
        }
    }

}
