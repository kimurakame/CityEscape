using System.Collections;
using UnityEngine;

/// <summary>
/// �h�A�̓������Ǘ�����N���X
/// </summary>
public class Door : MonoBehaviour
{
    //�h�A���J���Ă��邩�ǂ���
    [SerializeField]
    bool isOpen = false;
    
    //��]���̃h�A���ǂ���
    [SerializeField]
    private bool IsRotatingDoor = true;
    
    //�h�A���J���鑬�x
    [SerializeField]
    private float Speed = 1f;
    
    //�ǂꂮ�炢��]���邩
    [Header("Rotation Configs")]
    [SerializeField]
    private float RotationAmount = 90f;
    
    //�O����
    [SerializeField]
    private float ForwardDirection = 0;
    
    //�X���C�h���h�A
    [Header("Sliding Configs")]
    [SerializeField]
    private Vector3 SlideDirection = Vector3.back;
    
    //�ǂꂮ�炢�X���C�h���邩
    [SerializeField]
    private float SlideAmount = 1.9f;

    //��]�̊J�n�p�x
    private Vector3 StartRotation;

    //��]�̊J�n�ʒu
    private Vector3 StartPosition;

    //��]����
    private Vector3 Forward;

    //�A�j���[�V�����p�R���[�`��
    private Coroutine AnimationCoroutine;

    //���̃X�N���v�g�����Ă���C���X�^���X�����ꂽ���ɌĂ΂��
    private void Awake()
    {
        //�J�n�p�x�����݂̊p�x�ɐݒ�
        StartRotation = transform.rotation.eulerAngles;
        
        //��]�����𓮂������������ɐݒ�@���̏ꍇ�͉E���� 
        Forward = transform.right;

        //�J�n�ʒu�����݂̈ʒu�ɐݒ�
        StartPosition = transform.position;
    }

    /// <summary>
    /// �h�A���J���郁�\�b�h
    /// </summary>
    /// <param name="UserPosition"> �v���C���[�̈ʒu�@</param>
    public void Open(Vector3 UserPosition)
    {
        //�J���Ă��Ȃ���ԂȂ�
        if (!isOpen)
        {
            //���ݍĐ����̃R���[�`�������݂���ꍇ
            if (AnimationCoroutine != null)
            {
                //�R���[�`�����~�߂�
                StopCoroutine(AnimationCoroutine);
            }

            //��]���h�A�Ȃ�
            if (IsRotatingDoor)
            {
                //�ǂꂮ�炢��]���邩���v�Z����
                float dot = Vector3.Dot(Forward, (UserPosition - transform.position).normalized);
                //�f�o�b�O�p
                Debug.Log($"Dot: {dot.ToString("N3")}");
                
                //��]�A�j���[�V�����̃R���[�`�����J�n
                AnimationCoroutine = StartCoroutine(DoRotationOpen(dot));
            }
            else //�X���C�h���h�A�Ȃ�
            {
                //�X���C�h�A�j���[�V�����̃R���[�`�����J�n
                AnimationCoroutine = StartCoroutine(DoSlidingOpen()); 
            }
        }
    }

    /// <summary>
    /// �h�A����郁�\�b�h
    /// </summary>
    public void Close()
    {
        //�h�A���J���Ă����ԂȂ�
        if (isOpen)
        {
            //���ݍĐ����̃R���[�`�������݂���ꍇ
            if (AnimationCoroutine != null)
            {
                //�R���[�`�����~�߂�
                StopCoroutine(AnimationCoroutine);
            }

            //��]���h�A�Ȃ�
            if (IsRotatingDoor)
            {
                //��]�A�j���[�V�����̃R���[�`�����J�n
                AnimationCoroutine = StartCoroutine(DoRotationClose());
            }
            else�@//�X���C�h���h�A�Ȃ�
            {
                //�X���C�h�A�j���[�V�����̃R���[�`�����J�n
                AnimationCoroutine = StartCoroutine(DoSlidingClose());
            }
        }
    }

    /// <summary>
    /// ��]���h�A�p�̊J����A�j���[�V����
    /// </summary>
    /// <param name="ForwardAmount"> �ǂꂮ�炢��]���邩</param>
    /// <returns></returns>
    private IEnumerator DoRotationOpen(float ForwardAmount)
    {
        //�A�j���[�V�����J�n���̊p�x�����݂̊p�x�ɐݒ�
        Quaternion startRotation = transform.rotation;

        //�A�j���[�V�����I�����̊p�x
        Quaternion endRotation;

        /*�I�����̉�]�̌v�Z*/
        //��]����l�������������ȏ�
        if (ForwardAmount >= ForwardDirection)
        {
            endRotation = Quaternion.Euler(new Vector3(0, StartRotation.y + RotationAmount, 0));
        }
        else
        {
            endRotation = Quaternion.Euler(new Vector3(0, StartRotation.y - RotationAmount, 0));
        }

        //�J���Ă���t���O�𗧂Ă�
        isOpen = true;

        //�����鎞��
        float timeRate = 0;

        //1�b�ȓ��ɃA�j���[�V�������I������
        while (timeRate < 1)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, timeRate);

            //���̃t���[���܂ő҂�
            yield return null;

            //���̃t���[���ɂ����������ԂƑ��x�Ōo�ߎ��Ԃ��v�Z����
            timeRate += Time.deltaTime * Speed;
        }
    }

    /// <summary>
    /// �X���C�h���h�A�p�̊J����A�j���[�V����
    /// </summary>
    /// <returns></returns>
    private IEnumerator DoSlidingOpen()
    {
        //�A�j���[�V�����J�n���̈ʒu�����݈ʒu�ɐݒ肷��
        Vector3 startPosition = transform.position;

        //�A�j���[�V�����I�����̈ʒu���v�Z
        Vector3 endPosition = StartPosition + SlideAmount * SlideDirection;

        //�J���Ă���t���O�𗧂Ă�
        isOpen = true;

        //�����鎞��
        float timeRate = 0;

        //1�b�ȓ��ɃA�j���[�V�������I������
        while (timeRate < 1)
        {
            //�J�n�ʒu�ƏI���ʒu�̊ԃX���[�Y�ɓ������A���݈ʒu�ɂ���
            transform.position = Vector3.Lerp(startPosition, endPosition, timeRate);

            //���̃t���[���܂ő҂�
            yield return null;

            //���̃t���[���ɂ����������ԂƑ��x�Ōo�ߎ��Ԃ��v�Z����
            timeRate += Time.deltaTime * Speed;
        }
    }



    /// <summary>
    /// ��]���h�A�p�̕���A�j���[�V����
    /// </summary>
    /// <returns></returns>
    private IEnumerator DoRotationClose()
    {
        //�A�j���[�V�����J�n���̊p�x�����݂̊p�x�ɐݒ�
        Quaternion startRotation = transform.rotation;

        //�A�j���[�V�����I�����̊p�x�����Ƃ��Ƃ̊p�x�ɐݒ�
        Quaternion endRotation = Quaternion.Euler(StartRotation);

        //�J���Ă����Ԃ̃t���O�����낷
        isOpen = false;

        //�����鎞��
        float timeRate = 0;

        //1�b�ȓ��ɃA�j���[�V�������I������
        while (timeRate < 1)
        {
            //�p�x���J�n�ʒu�ƏI���ʒu�̊ԃX���[�Y�ɓ�����
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, timeRate);
            //���̃t���[���܂ő҂�
            yield return null;

            //���̃t���[���ɂ����������ԂƑ��x�Ōo�ߎ��Ԃ��v�Z����
            timeRate += Time.deltaTime * Speed;
        }
    }

    /// <summary>
    /// �X���C�h���h�A�p�̕���A�j���[�V����
    /// </summary>
    /// <returns></returns>
    private IEnumerator DoSlidingClose()
    {
        //���̃A�j���[�V�����̊J�n�ʒu�����ݒn�ɐݒ�
        Vector3 startPosition = transform.position;
        
        //��]�I���ʒu�����Ƃ��Ƃ̊J�n���̈ʒu�ɐݒ�
        Vector3 endPosition = StartPosition;

        //�J���Ă����Ԃ̃t���O�����낷
        isOpen = false;

        //�����鎞��
        float timeRate = 0;

        //1�b�ȓ��ɃA�j���[�V�������I������
        while (timeRate < 1)
        {
            //�J�n�ʒu�ƏI���ʒu�̊ԃX���[�Y�ɓ������A���݈ʒu�ɂ���
            transform.position = Vector3.Lerp(startPosition, endPosition, timeRate);
            //���̃t���[���܂ő҂�
            yield return null;
            //���̃t���[���ɂ����������ԂƑ��x�Ōo�ߎ��Ԃ��v�Z����
            timeRate += Time.deltaTime * Speed;
        }
    }

    public bool IsOpen() {

        return isOpen;
    }
}
