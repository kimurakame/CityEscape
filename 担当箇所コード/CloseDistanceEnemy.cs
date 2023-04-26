using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// �ߋ����G�̃N���X
/// </summary>
public class CloseDistanceEnemy : MonoBehaviour
{
    //�v���C���[�����m������A���߂�ꂽ�͈͓��𓮂����肷�邽�߁@(Inspector��Őݒ肷��K�v����)
    [SerializeField]
    NavMeshAgent agent;

    //�v���C���[��Transform(�ʒu) (Inspector��Őݒ肷��K�v����)
    [SerializeField]
    Transform player;

    //�n�ʁA�v���C���[�̃��C���[�}�X�N (Inspector��Őݒ肷��K�v����)
    [SerializeField]
    LayerMask whatIsGround, whatIsPlayer;

    //�̗�
    float health;

    #region �p�g���[���p
    [SerializeField]
    Vector3 walkPoint;

    //�ړ��|�C���g�ݒ肳�ꂽ���ǂ���
    [SerializeField]
    bool walkPointSet;

    //�ړ��|�C���g�܂ł̋���
    [SerializeField]
    float walkPointRange;

    #endregion

    #region �X�e�[�g

    //����͈� (Inspector��Őݒ肷��K�v����)
    [SerializeField]
    float sightRange;

    //�U���͈� (Inspector��Őݒ肷��K�v����)
    [SerializeField]
    float attackRange;

    //�v���C���[������ɓ��������ǂ���
    bool playerInSightRange; 
    
    //�v���C���[���U���͈͓����ǂ���
    bool playerInAttackRange;

    #endregion

    #region �U���p

    //�U���������ǂ���
    bool alreadyAttacked;

    //�U�����@(Inspector��Őݒ肷��K�v����)
    [SerializeField]
    GameObject launcher;

    //�U����^���邽�߂̓����蔻��I�u�W�F�N�g 
    [SerializeField]
    GameObject CollisionDetection;

    //�U���J�n�O�̎���
    [SerializeField]
    float timeBeforeAttack = 0.5f;

    //�U���J�n��̎���
    [SerializeField]
    float timeAfterAttack = 1f;

    //���̍U�����ǂ���
    bool firstTimeAttack = true;

    #endregion    
    
    //�����邩�ǂ���
    bool isMove;

    //���Ԍo�ߌv�Z�p
    float time = 0f;


    //���̃X�N���v�g�����Ă���C���X�^���X�����ꂽ���ɌĂ΂��
    private void Awake()
    {
        //�v���C���[�̈ʒu���擾
        player = GameObject.Find("Player").transform;

        //GameObject�ɂ��Ă���AI�R���|�l���g���擾
        agent = GetComponent<NavMeshAgent>();
    }

    //���t���[���Ă΂�郁�\�b�h
    private void Update()
    {
        //�����Ă����Ȃ�
        if (isMove)
        {
            //�v���C���[��������ɂ��邩�ǂ����m�F����
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);

            //�v���C���[���U���͈͓��ɂ��邩�ǂ����m�F����
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

            //�����v���C���[�͎�����ł��U���͈͓��ɂł������Ă��Ȃ��ꍇ�̓p�g���[���𑱂���
            if (!playerInSightRange && !playerInAttackRange) Patroling();

            //���߂ăv���C���[���U������ꍇ
            if (firstTimeAttack)
            {
                //�����v���C���[�͎�����ɂ��邯�ǍU���͈͓��ɂ��Ȃ��ꍇ�̓v���C���[��ǂ�������
                if (playerInSightRange && !playerInAttackRange) ChasePlayer();
            }

            //�����v���C���[���U���͈͓��ɂ��Ă�������ɂ���ꍇ�v���C���[���U������
            if (playerInAttackRange && playerInSightRange) AttackPlayer();
        }
        //�U����̃C���^�[�o��
        else
        {
            time += Time.deltaTime;
            //�U����̃C���^�[�o�����o�߂�����
            if (time > timeAfterAttack)
            {
                //�����Ă�����
                isMove = true;

                //�o�ߎ��ԃ��Z�b�g
                time = 0;
            }
        }
    }


    /// <summary>
    /// �p�g���[���@�ړ��|�C���g��T���A�ݒ肵�A�|�C���g�܂ōs��
    /// </summary>
    private void Patroling()
    {
        //�������Ă������Ȃ�
        if (isMove)
        {
            //�����ړ��|�C���g���ݒ�̏ꍇ�ړ��|�C���g��T��
            if (!walkPointSet) SearchWalkPoint();

            //�����ړ��|�C���g�ݒ肳��Ă���ꍇ�ړ��|�C���g�܂ōs��
            if (walkPointSet) agent.SetDestination(walkPoint);

            //���ݒn����ړ��|�C���g�܂ł̋������v�Z����
            Vector3 distanceToWalkPoint = transform.position - walkPoint;

            //�ړ��|�C���g�ɓ��������ꍇ�ړ��|�C���g���ݒ�ɂ���
            if (distanceToWalkPoint.magnitude < 1f)  walkPointSet = false;
        }
    }

    /// <summary>
    /// �ړ��|�C���g��T��
    /// </summary>
    private void SearchWalkPoint()
    {
        //���������Ă����ꍇ
        if (isMove)
        {
            //�����_���ɓ����Ă������͈͓��Ƀ|�C���g��ݒ�
            float randomZ = Random.Range(-walkPointRange, walkPointRange);
            float randomX = Random.Range(-walkPointRange, walkPointRange);

            //�ړ��|�C���g���쐬
            walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

            //�ő勗��
            float maxDistance = 2f;

            //�ړ��|�C���g���n�ʂɂȂ��Ă���Ȃ�ړ��|�C���g��ݒ�ςɂ���
            if (Physics.Raycast(walkPoint, -transform.up, maxDistance, whatIsGround)) walkPointSet = true;
        }
    }

    /// <summary>
    /// �v���C���[��ǂ�������
    /// </summary>
    private void ChasePlayer()
    {
        //���������Ă����Ȃ�
        if (isMove)
        {
            //�ړ��|�C���g���v���C���[�̈ʒu�ɂ���
            agent.SetDestination(player.position);
        }
    }

    /// <summary>
    /// �v���C���[���U������
    /// </summary>
    private void AttackPlayer()
    {
        //�U�����G�������Ȃ��悤�ɂ���
        agent.SetDestination(transform.position);

        //���̍U���̃t���O�����낷
        firstTimeAttack = false;

        //�v���C���[�Ɍ�����
        transform.LookAt(player);

        //1�b�㎟�̍U���܂ł�����Ƒ҂Ƃ������\�b�h���Ă�
        Invoke(nameof(WaitAttack), 1);
    }

    /// <summary>
    /// �v���C���[�����삩��o���ꍇ�A�������͍U���͈͂���o���ꍇ�U����Ԃ����Z�b�g����
    /// </summary>
    private void ResetAttack()
    {
        //���߂Ă̍U���t���O�𗧂Ă�
        firstTimeAttack = true;

        //�U�������Ƃ����t���O�����낷
        alreadyAttacked = false;
    }

    /// <summary>
    /// �_���[�W���󂯂�
    /// </summary>
    /// <param name="damage"> �^����ꂽ�_���[�W</param>
    public void TakeDamage(int damage)
    {
        //�^����ꂽ���̃_���[�W��̗͂������
        health -= damage;

        //�|����Ă���������܂ł̎���
        float timeUntilDestroyed = 0.5f;

        //�����̗͂�0�ȉ��Ȃ�A������鎞�Ԍo�ߌ�ɂ��̓G������
        if (health <= 0) Invoke(nameof(DestroyEnemy), timeUntilDestroyed);
    }

    /// <summary>
    /// �G������
    /// </summary>
    private void DestroyEnemy()
    {
        //�������O�ɍU����1�񎎂݂�
        Instantiate(CollisionDetection, transform.position, Quaternion.identity);
        
        //�Q�[�����炱�̓G������
        Destroy(gameObject);
    }

    /// <summary>
    /// �U���㎟�̍U���܂ő҂�
    /// </summary>
    private void WaitAttack()
    {
        //�����܂��U�����Ă��Ȃ��Ȃ�
        if (!alreadyAttacked)
        {
            //�U�����J��o��
            Instantiate(CollisionDetection, launcher.transform.position, Quaternion.identity);
            
            //�U�������t���O�𗧂Ă�
            alreadyAttacked = true;

            //�U���C���^�[�o����U�������Z�b�g����
            Invoke(nameof(ResetAttack), timeAfterAttack);
        }

        //�����Ȃ�����
        isMove = false;
    }
}
