using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBulbOnce : MonoBehaviour
{
    public bool is_stage_hit = false;   //ステージに当たっているか
    private int nothit_count;           //当たってらずに何フレーム立ったか (Exitが呼ばれないため)
    private MeshRenderer material;      //光らすため

    private ElectricalPower electricalpower;    //電源そステージが当たっているかを取得

    private void Start()
    {
        material = GetComponent<MeshRenderer>();
        electricalpower = GameObject.Find("ElectricalPower").GetComponent<ElectricalPower>();
    }

    private void Update()
    {
        //どちらもステージに当たっているか
        if (electricalpower.is_stage_hit && is_stage_hit)
        {
            material.material.color = Color.white;
        }
        else
        {
            material.material.color = Color.grey;
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
