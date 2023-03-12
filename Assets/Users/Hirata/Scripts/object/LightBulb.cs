using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBulb : MonoBehaviour
{
    public bool is_stage_hit = false;   //ステージに当たっているか
    private int nothit_count;           //当たってらずに何フレーム立ったか (Exitが呼ばれないため)
    private MeshRenderer material;          //光らすため

    private ElectricalPower electricalpower;    //電源そステージが当たっているかを取得

    private void Start()
    {
        material = GetComponent<MeshRenderer>();
        electricalpower = GameObject.Find("ElectricalPower").GetComponent<ElectricalPower>();
    }

    private void Update()
    {
        //ステージに当たっていなければフラグを変える
        if (is_stage_hit)
        {
            if (nothit_count > 5)
            {
                is_stage_hit = false;
            }
            else
                nothit_count++;
        }

        //どちらもステージに当たっているか
        if (electricalpower.is_stage_hit && is_stage_hit)
        {
            material.material.color = Color.white;
        }
        else
        {
            material.material.color = Color.black;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            is_stage_hit = true;
            nothit_count = 0;
        }
    }
}
