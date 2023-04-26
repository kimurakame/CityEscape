using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 近距離敵のクラス
/// </summary>
public class CloseDistanceEnemy : MonoBehaviour
{
    //プレイヤーを感知したり、決められた範囲内を動いたりするため　(Inspector上で設定する必要あり)
    [SerializeField]
    NavMeshAgent agent;

    //プレイヤーのTransform(位置) (Inspector上で設定する必要あり)
    [SerializeField]
    Transform player;

    //地面、プレイヤーのレイヤーマスク (Inspector上で設定する必要あり)
    [SerializeField]
    LayerMask whatIsGround, whatIsPlayer;

    //体力
    float health;

    #region パトロール用
    [SerializeField]
    Vector3 walkPoint;

    //移動ポイント設定されたかどうか
    [SerializeField]
    bool walkPointSet;

    //移動ポイントまでの距離
    [SerializeField]
    float walkPointRange;

    #endregion

    #region ステート

    //視野範囲 (Inspector上で設定する必要あり)
    [SerializeField]
    float sightRange;

    //攻撃範囲 (Inspector上で設定する必要あり)
    [SerializeField]
    float attackRange;

    //プレイヤーが視野に入ったかどうか
    bool playerInSightRange; 
    
    //プレイヤーが攻撃範囲内かどうか
    bool playerInAttackRange;

    #endregion

    #region 攻撃用

    //攻撃したかどうか
    bool alreadyAttacked;

    //攻撃元　(Inspector上で設定する必要あり)
    [SerializeField]
    GameObject launcher;

    //攻撃を与えるための当たり判定オブジェクト 
    [SerializeField]
    GameObject CollisionDetection;

    //攻撃開始前の時間
    [SerializeField]
    float timeBeforeAttack = 0.5f;

    //攻撃開始後の時間
    [SerializeField]
    float timeAfterAttack = 1f;

    //初の攻撃がどうか
    bool firstTimeAttack = true;

    #endregion    
    
    //動けるかどうか
    bool isMove;

    //時間経過計算用
    float time = 0f;


    //このスクリプトがついているインスタンスが作られた時に呼ばれる
    private void Awake()
    {
        //プレイヤーの位置を取得
        player = GameObject.Find("Player").transform;

        //GameObjectについているAIコンポネントを取得
        agent = GetComponent<NavMeshAgent>();
    }

    //毎フレーム呼ばれるメソッド
    private void Update()
    {
        //動いていいなら
        if (isMove)
        {
            //プレイヤーが視野内にいるかどうか確認する
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);

            //プレイヤーが攻撃範囲内にいるかどうか確認する
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

            //もしプレイヤーは視野内でも攻撃範囲内にでも入っていない場合はパトロールを続ける
            if (!playerInSightRange && !playerInAttackRange) Patroling();

            //初めてプレイヤーを攻撃する場合
            if (firstTimeAttack)
            {
                //もしプレイヤーは視野内にいるけど攻撃範囲内にいない場合はプレイヤーを追いかける
                if (playerInSightRange && !playerInAttackRange) ChasePlayer();
            }

            //もしプレイヤーが攻撃範囲内にいてかつ視野内にいる場合プレイヤーを攻撃する
            if (playerInAttackRange && playerInSightRange) AttackPlayer();
        }
        //攻撃後のインターバル
        else
        {
            time += Time.deltaTime;
            //攻撃後のインターバルが経過したら
            if (time > timeAfterAttack)
            {
                //動いてもいい
                isMove = true;

                //経過時間リセット
                time = 0;
            }
        }
    }


    /// <summary>
    /// パトロール　移動ポイントを探し、設定し、ポイントまで行く
    /// </summary>
    private void Patroling()
    {
        //今動いてもいいなら
        if (isMove)
        {
            //もし移動ポイント未設定の場合移動ポイントを探す
            if (!walkPointSet) SearchWalkPoint();

            //もし移動ポイント設定されている場合移動ポイントまで行く
            if (walkPointSet) agent.SetDestination(walkPoint);

            //現在地から移動ポイントまでの距離を計算する
            Vector3 distanceToWalkPoint = transform.position - walkPoint;

            //移動ポイントに到着した場合移動ポイント未設定にする
            if (distanceToWalkPoint.magnitude < 1f)  walkPointSet = false;
        }
    }

    /// <summary>
    /// 移動ポイントを探す
    /// </summary>
    private void SearchWalkPoint()
    {
        //もし動いていい場合
        if (isMove)
        {
            //ランダムに動いてもいい範囲内にポイントを設定
            float randomZ = Random.Range(-walkPointRange, walkPointRange);
            float randomX = Random.Range(-walkPointRange, walkPointRange);

            //移動ポイントを作成
            walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

            //最大距離
            float maxDistance = 2f;

            //移動ポイントが地面になっているなら移動ポイントを設定済にする
            if (Physics.Raycast(walkPoint, -transform.up, maxDistance, whatIsGround)) walkPointSet = true;
        }
    }

    /// <summary>
    /// プレイヤーを追いかける
    /// </summary>
    private void ChasePlayer()
    {
        //もし動いていいなら
        if (isMove)
        {
            //移動ポイントをプレイヤーの位置にする
            agent.SetDestination(player.position);
        }
    }

    /// <summary>
    /// プレイヤーを攻撃する
    /// </summary>
    private void AttackPlayer()
    {
        //攻撃中敵が動かないようにする
        agent.SetDestination(transform.position);

        //初の攻撃のフラグをおろす
        firstTimeAttack = false;

        //プレイヤーに向かう
        transform.LookAt(player);

        //1秒後次の攻撃までちょっと待つというメソッドを呼ぶ
        Invoke(nameof(WaitAttack), 1);
    }

    /// <summary>
    /// プレイヤーが視野から出た場合、もしくは攻撃範囲から出た場合攻撃状態をリセットする
    /// </summary>
    private void ResetAttack()
    {
        //初めての攻撃フラグを立てる
        firstTimeAttack = true;

        //攻撃したというフラグをおろす
        alreadyAttacked = false;
    }

    /// <summary>
    /// ダメージを受ける
    /// </summary>
    /// <param name="damage"> 与えられたダメージ</param>
    public void TakeDamage(int damage)
    {
        //与えられた分のダメージを体力から引く
        health -= damage;

        //倒されてから消されるまでの時間
        float timeUntilDestroyed = 0.5f;

        //もし体力が0以下なら、消される時間経過後にこの敵を消す
        if (health <= 0) Invoke(nameof(DestroyEnemy), timeUntilDestroyed);
    }

    /// <summary>
    /// 敵を消す
    /// </summary>
    private void DestroyEnemy()
    {
        //消される前に攻撃を1回試みる
        Instantiate(CollisionDetection, transform.position, Quaternion.identity);
        
        //ゲームからこの敵を消す
        Destroy(gameObject);
    }

    /// <summary>
    /// 攻撃後次の攻撃まで待つ
    /// </summary>
    private void WaitAttack()
    {
        //もしまだ攻撃していないなら
        if (!alreadyAttacked)
        {
            //攻撃を繰り出す
            Instantiate(CollisionDetection, launcher.transform.position, Quaternion.identity);
            
            //攻撃したフラグを立てる
            alreadyAttacked = true;

            //攻撃インターバル後攻撃をリセットする
            Invoke(nameof(ResetAttack), timeAfterAttack);
        }

        //動けなくする
        isMove = false;
    }
}
