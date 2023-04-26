using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 弾丸箱の動きを管理するクラス
/// </summary>
public class Box : MonoBehaviour
{
    //この弾丸箱が開いている状態であるかどうか
    [SerializeField]
    bool isOpen = false;

    //入っている弾丸の数　(Inspector上で設定する必要あり)
    [SerializeField]
    int bulletQuantity;

    //飛ばす勢い
    float force = 20f;

    /// <summary>
    /// 弾丸箱を開ける
    /// </summary>
    public void BoxOpen()
    {
        //もしまだ開いていないなら
        if (!isOpen)
        {
            //Rigidbody取得
            Rigidbody rb = GetComponentInChildren<Rigidbody>();
            
            //上に飛ばす動き
            rb.AddForce(transform.up * force, ForceMode.Impulse);           
        }
    }

    public bool IsOpen() {

        return isOpen;
    } 
}
