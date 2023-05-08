using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallMove : MonoBehaviour
{       
    private WallSwitch wallSwitch;            //switch�̃X�N���v�g

    public Vector3 movePosition;              //��x�̈ړ���
    public int separationCount;               //����ɕ����邩  
    public float oneMoveTime;                 //��x�̈ړ�����
    public float untilReturnTime;             //�߂�܂ł̎��� 
    public float returnTime;                  //�߂�̈ړ�����

    private Vector3 startPosition;            //�����ʒu
    private Vector3 velocity = Vector3.zero;  //�ړ��ʕۑ�
    private int count;                        //���݉�
    private float untilTime;                  //�Ō�Ɉړ������Ă���̎���

    void Start()
    {
        wallSwitch = transform.parent.Find("Swich").GetComponent<WallSwitch>();
        startPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        //�Ōォ�牟�������Ԃ��߂�����߂�
        if (untilReturnTime < untilTime) 
        {
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, startPosition, ref velocity, returnTime);
            //�߂�r���ɃJ�E���g��X�V
            Vector3 pos = transform.localPosition - startPosition;
            if (pos.x != 0)
                count = (int)(pos.x / (movePosition.x / separationCount));
            else if (pos.y != 0)
                count = (int)(pos.y / (movePosition.y / separationCount));
        }
        else //����ȊO�͉񐔂��ƂɍX�V
        {
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, startPosition + (movePosition / separationCount * count), ref velocity, oneMoveTime);
        }

        untilTime += Time.deltaTime;
    }

    //�X�C�b�`���������ړ�
    public void OnceWallMove()
    {
        if (wallSwitch.GetIsHit())
        {
            if (separationCount > count)
                count++;
            untilTime = 0;
        }
    }
}
