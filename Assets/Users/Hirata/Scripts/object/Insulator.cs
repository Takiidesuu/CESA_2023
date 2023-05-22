using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Insulator : MonoBehaviour
{
    private int hit_count;           //�������Ă炸�ɉ��t���[���������� (Exit���Ă΂�Ȃ�����)
    private MeshRenderer material;          //���炷����
    
    private SoundManager soundManager;

    private void Start()
    {
        material = GetComponent<MeshRenderer>();
        soundManager = GetComponent<SoundManager>();
    }

    private void Update()
    {
        //�ǂ�����X�e�[�W�ɓ������Ă��邩
        if (hit_count < 50)
        {
            material.material.color = Color.yellow;
        }
        else
        {
            material.material.color = Color.green;
        }
        hit_count++;
    }

    private void OnTriggerStay(Collider other)
    {
        //�G���L�{�[���ɓ�����Ώ���
        if (other.gameObject.CompareTag("ElectricalBall"))
        {
            soundManager.PlaySoundEffect("ElectricHit");
            hit_count = 0;
            Destroy(other.gameObject);
        }
    }
}