using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class debug_collider : MonoBehaviour {

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name); // ログを表示する
    }
}
