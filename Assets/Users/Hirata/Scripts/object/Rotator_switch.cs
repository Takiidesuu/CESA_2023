using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator_switch : MonoBehaviour
{
    public bool player_hit = false;   //ステージに当たっているか
    private int nothit_count;           //当たってらずに何フレーム立ったか (Exitが呼ばれないため)

    private DeformStage deform_stage;    //電源がステージが当たっているかを取得
    public Material on_power_material;    //マテリアルを変更
    public Material off_power_material;    //マテリアルを変更

    private void Start()
    {
    }

    private void Update()
    {
        //ステージに当たっていなければフラグを変える
        //if (player_hit)
        //{
        //    if (nothit_count > 5)
        //    {
        //        player_hit = false;
        //        deform_stage = null;
        //    }
        //    else
        //        nothit_count++;
        //}

        //どちらもステージに当たっているか
        //if (deform_stage != null)
        //{
        //    if (deform_stage.hit_electrical && player_hit)
        //    {
        //    }
        //    else
        //    {
        //        player_hit = false;
        //    }
        //}
        //if(player_hit)
        //{
        //    gameObject.GetComponent<Renderer>().material = on_power_material;
        //}
        //else
        //{
        //    gameObject.GetComponent<Renderer>().material = off_power_material;
        //}
    }

    //private void OnTriggerStay(Collider other)
    //{
    //    if (other.gameObject.CompareTag("ElectricalBall"))
    //    {
    //        player_hit = true;
    //        nothit_count = 0;
    //    }
    //    if (other.gameObject.layer == 6 && player_hit)
    //    {
    //        deform_stage = other.transform.root.GetComponent<DeformStage>();
    //        nothit_count = 0;
    //    }
    //}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player_hit = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player_hit = false;
        }
    }
}
