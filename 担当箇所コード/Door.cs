using System.Collections;
using UnityEngine;

/// <summary>
/// ドアの動きを管理するクラス
/// </summary>
public class Door : MonoBehaviour
{
    //ドアが開いているかどうか
    [SerializeField]
    bool isOpen = false;
    
    //回転式のドアかどうか
    [SerializeField]
    private bool IsRotatingDoor = true;
    
    //ドアを開ける速度
    [SerializeField]
    private float Speed = 1f;
    
    //どれぐらい回転するか
    [Header("Rotation Configs")]
    [SerializeField]
    private float RotationAmount = 90f;
    
    //前方向
    [SerializeField]
    private float ForwardDirection = 0;
    
    //スライド式ドア
    [Header("Sliding Configs")]
    [SerializeField]
    private Vector3 SlideDirection = Vector3.back;
    
    //どれぐらいスライドするか
    [SerializeField]
    private float SlideAmount = 1.9f;

    //回転の開始角度
    private Vector3 StartRotation;

    //回転の開始位置
    private Vector3 StartPosition;

    //回転方向
    private Vector3 Forward;

    //アニメーション用コルーチン
    private Coroutine AnimationCoroutine;

    //このスクリプトがついているインスタンスが作られた時に呼ばれる
    private void Awake()
    {
        //開始角度を現在の角度に設定
        StartRotation = transform.rotation.eulerAngles;
        
        //回転方向を動かしたい方向に設定　この場合は右方向 
        Forward = transform.right;

        //開始位置を現在の位置に設定
        StartPosition = transform.position;
    }

    /// <summary>
    /// ドアを開けるメソッド
    /// </summary>
    /// <param name="UserPosition"> プレイヤーの位置　</param>
    public void Open(Vector3 UserPosition)
    {
        //開いていない状態なら
        if (!isOpen)
        {
            //現在再生中のコルーチンが存在する場合
            if (AnimationCoroutine != null)
            {
                //コルーチンを止める
                StopCoroutine(AnimationCoroutine);
            }

            //回転式ドアなら
            if (IsRotatingDoor)
            {
                //どれぐらい回転するかを計算する
                float dot = Vector3.Dot(Forward, (UserPosition - transform.position).normalized);
                //デバッグ用
                Debug.Log($"Dot: {dot.ToString("N3")}");
                
                //回転アニメーションのコルーチンを開始
                AnimationCoroutine = StartCoroutine(DoRotationOpen(dot));
            }
            else //スライド式ドアなら
            {
                //スライドアニメーションのコルーチンを開始
                AnimationCoroutine = StartCoroutine(DoSlidingOpen()); 
            }
        }
    }

    /// <summary>
    /// ドアを閉じるメソッド
    /// </summary>
    public void Close()
    {
        //ドアが開いている状態なら
        if (isOpen)
        {
            //現在再生中のコルーチンが存在する場合
            if (AnimationCoroutine != null)
            {
                //コルーチンを止める
                StopCoroutine(AnimationCoroutine);
            }

            //回転式ドアなら
            if (IsRotatingDoor)
            {
                //回転アニメーションのコルーチンを開始
                AnimationCoroutine = StartCoroutine(DoRotationClose());
            }
            else　//スライド式ドアなら
            {
                //スライドアニメーションのコルーチンを開始
                AnimationCoroutine = StartCoroutine(DoSlidingClose());
            }
        }
    }

    /// <summary>
    /// 回転式ドア用の開けるアニメーション
    /// </summary>
    /// <param name="ForwardAmount"> どれぐらい回転するか</param>
    /// <returns></returns>
    private IEnumerator DoRotationOpen(float ForwardAmount)
    {
        //アニメーション開始時の角度を現在の角度に設定
        Quaternion startRotation = transform.rotation;

        //アニメーション終了時の角度
        Quaternion endRotation;

        /*終了時の回転の計算*/
        //回転する値が動かす方向以上
        if (ForwardAmount >= ForwardDirection)
        {
            endRotation = Quaternion.Euler(new Vector3(0, StartRotation.y + RotationAmount, 0));
        }
        else
        {
            endRotation = Quaternion.Euler(new Vector3(0, StartRotation.y - RotationAmount, 0));
        }

        //開いているフラグを立てる
        isOpen = true;

        //かかる時間
        float timeRate = 0;

        //1秒以内にアニメーションを終了する
        while (timeRate < 1)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, timeRate);

            //次のフレームまで待つ
            yield return null;

            //このフレームにかかった時間と速度で経過時間を計算する
            timeRate += Time.deltaTime * Speed;
        }
    }

    /// <summary>
    /// スライド式ドア用の開けるアニメーション
    /// </summary>
    /// <returns></returns>
    private IEnumerator DoSlidingOpen()
    {
        //アニメーション開始時の位置を現在位置に設定する
        Vector3 startPosition = transform.position;

        //アニメーション終了時の位置を計算
        Vector3 endPosition = StartPosition + SlideAmount * SlideDirection;

        //開いているフラグを立てる
        isOpen = true;

        //かかる時間
        float timeRate = 0;

        //1秒以内にアニメーションを終了する
        while (timeRate < 1)
        {
            //開始位置と終了位置の間スムーズに動かし、現在位置にする
            transform.position = Vector3.Lerp(startPosition, endPosition, timeRate);

            //次のフレームまで待つ
            yield return null;

            //このフレームにかかった時間と速度で経過時間を計算する
            timeRate += Time.deltaTime * Speed;
        }
    }



    /// <summary>
    /// 回転式ドア用の閉じるアニメーション
    /// </summary>
    /// <returns></returns>
    private IEnumerator DoRotationClose()
    {
        //アニメーション開始時の角度を現在の角度に設定
        Quaternion startRotation = transform.rotation;

        //アニメーション終了時の角度をもともとの角度に設定
        Quaternion endRotation = Quaternion.Euler(StartRotation);

        //開いている状態のフラグをおろす
        isOpen = false;

        //かかる時間
        float timeRate = 0;

        //1秒以内にアニメーションを終了する
        while (timeRate < 1)
        {
            //角度を開始位置と終了位置の間スムーズに動かす
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, timeRate);
            //次のフレームまで待つ
            yield return null;

            //このフレームにかかった時間と速度で経過時間を計算する
            timeRate += Time.deltaTime * Speed;
        }
    }

    /// <summary>
    /// スライド式ドア用の閉じるアニメーション
    /// </summary>
    /// <returns></returns>
    private IEnumerator DoSlidingClose()
    {
        //このアニメーションの開始位置を現在地に設定
        Vector3 startPosition = transform.position;
        
        //回転終了位置をもともとの開始時の位置に設定
        Vector3 endPosition = StartPosition;

        //開いている状態のフラグをおろす
        isOpen = false;

        //かかる時間
        float timeRate = 0;

        //1秒以内にアニメーションを終了する
        while (timeRate < 1)
        {
            //開始位置と終了位置の間スムーズに動かし、現在位置にする
            transform.position = Vector3.Lerp(startPosition, endPosition, timeRate);
            //次のフレームまで待つ
            yield return null;
            //このフレームにかかった時間と速度で経過時間を計算する
            timeRate += Time.deltaTime * Speed;
        }
    }

    public bool IsOpen() {

        return isOpen;
    }
}
