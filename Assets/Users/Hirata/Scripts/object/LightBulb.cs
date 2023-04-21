using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBulb : MonoBehaviour
{
    public bool is_stage_hit = false;   //ステージに当たっているか
    private int nothit_count;           //当たってらずに何フレーム立ったか (Exitが呼ばれないため)

    private DeformStage deform_stage;    //電源がステージが当たっているかを取得
    private LightBulbChangeMaterial changeMaterial;    //マテリアルを変更
    private SoundManager soundManager;

    private void Start()
    {
        changeMaterial = GetComponent<LightBulbChangeMaterial>();
        soundManager = GetComponent<SoundManager>();    
    }

    private void Update()
    {
        //ステージに当たっていなければフラグを変える
        if (is_stage_hit)
        {
            if (nothit_count > 5)
            {
                changeMaterial.OnPower = false;
                is_stage_hit = false;
                deform_stage = null;
            }
            else
                nothit_count++;
        }

        //どちらもステージに当たっているか
        if (deform_stage != null)
        {
            if (deform_stage.hit_electrical && is_stage_hit)
            {
            }
            else
            {
                is_stage_hit = false;
                changeMaterial.OnPower = false;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("ElectricalBall"))
        {
            changeMaterial.OnPower = true;
            is_stage_hit = true;
            nothit_count = 0;
        }
        if (other.gameObject.layer == 6 && is_stage_hit)
        {
            deform_stage = other.transform.root.GetComponent<DeformStage>();
            nothit_count = 0;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("ElectricalBall"))
        {
            soundManager.PlaySoundEffect("Hit");
        }
    }
}
