using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBulbOnce : MonoBehaviour
{
    public bool is_stage_hit = false;   //ステージに当たっているか
    private MeshRenderer material;      //光らすため

    private void Start()
    {
        material = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        //どちらもステージに当たっているか
        if (is_stage_hit)
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
        if (other.gameObject.CompareTag("ElectricalBall"))
        {
            is_stage_hit = true;
        }
    }
}
