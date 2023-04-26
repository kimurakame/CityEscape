using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �e�۔��̓������Ǘ�����N���X
/// </summary>
public class Box : MonoBehaviour
{
    //���̒e�۔����J���Ă����Ԃł��邩�ǂ���
    [SerializeField]
    bool isOpen = false;

    //�����Ă���e�ۂ̐��@(Inspector��Őݒ肷��K�v����)
    [SerializeField]
    int bulletQuantity;

    //��΂�����
    float force = 20f;

    /// <summary>
    /// �e�۔����J����
    /// </summary>
    public void BoxOpen()
    {
        //�����܂��J���Ă��Ȃ��Ȃ�
        if (!isOpen)
        {
            //Rigidbody�擾
            Rigidbody rb = GetComponentInChildren<Rigidbody>();
            
            //��ɔ�΂�����
            rb.AddForce(transform.up * force, ForceMode.Impulse);           
        }
    }

    public bool IsOpen() {

        return isOpen;
    } 
}
