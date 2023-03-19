using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricalPower : MonoBehaviour
{
    private DeformStage deformstage;    //当たったステージ
    public bool is_stage_hit = false;   //ステージに当たっているか
    private int nothit_count;           //当たってらずに何フレーム立ったか (Exitが呼ばれないため)

    private GameObject ElectricBall;    //電球
    private float time;                 //前回からの秒数
    private float old_time;             //前回生成した秒数
    private float electricball_instan_time = 5;//電球を繰り返し生成する時間
    private Vector3 electricball_position;//電球を生成する位置

    private void Start()
    {
        ElectricBall = (GameObject)Resources.Load("ElectricBall");
    }

    private void Update()
    {
        if (is_stage_hit)
        {
            if (nothit_count > 5)
            {
                deformstage.IsElectricalPower(false);
                is_stage_hit = false;
            }
            else
                nothit_count++;

            if (time > electricball_instan_time)
            {
                Instantiate(ElectricBall, electricball_position, Quaternion.identity);
                old_time = Time.time;
            }
        }
        time = Time.time - old_time;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            deformstage = other.transform.root.GetComponent<DeformStage>();     //どののステージと当たっているか
            deformstage.IsElectricalPower(true);                                //電源が当たった事をステージに渡す
            electricball_position = other.ClosestPointOnBounds(this.transform.position);
            is_stage_hit = true;
            nothit_count = 0;
        }
    }
}
