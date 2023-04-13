using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricalPower : MonoBehaviour
{
    private DeformStage deformstage;    //当たったステージ
    public bool is_stage_hit = false;   //ステージに当たっているか
    private int nothit_count;           //当たらずに何フレーム立ったか (Exitが呼ばれないため)
    public float electricball_instan_time = 5;//電球を繰り返し生成する時間
    public float start_time;    //開始ラグ
    public GameObject ElectricBall;    //電球
    private float hit_elapsed_time;    //継続ヒット時間
    private float time;                 //前回からの秒数
    private float old_time;             //前回生成した秒数
    private Vector3 electricball_position;//電球を生成する位置

    private void Start()
    {
        
    }

    private void Update()
    {
        //ステージとの接触が無くなった場合タイマーをリセット
        if (is_stage_hit)
        {
            if (nothit_count > 5)
            {
                deformstage.IsElectricalPower(false);
                is_stage_hit = false;
            }
            else
                nothit_count++;

            if (time > electricball_instan_time && hit_elapsed_time > start_time)
            {
                Instantiate(ElectricBall, electricball_position, Quaternion.identity);
                old_time = Time.time;
            }
        }
        else
        {
            hit_elapsed_time = 0;
        }
        time = Time.time - old_time;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            hit_elapsed_time += Time.deltaTime;
            deformstage = other.transform.root.GetComponent<DeformStage>();     //どののステージと当たっているか
            deformstage.IsElectricalPower(true);                                //電源が当たった事をステージに渡す
            electricball_position = other.ClosestPointOnBounds(this.transform.position);
            is_stage_hit = true;
            nothit_count = 0;
        }
    }
}
