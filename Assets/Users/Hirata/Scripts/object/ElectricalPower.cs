using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricalPower : MonoBehaviour
{
    public bool is_stage_hit = false;   //ステージに当たっているか
    private int nothit_count;            //当たってらずに何フレーム立ったか (Exitが呼ばれないため)

    private void Update()
    {
        if (is_stage_hit)
        {
            if (nothit_count > 5)
            {
                is_stage_hit = false;
            }
            else
                nothit_count++;
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
