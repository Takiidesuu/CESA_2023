using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Insulator : MonoBehaviour
{
    private int hit_count;           //当たってらずに何フレーム立ったか (Exitが呼ばれないため)
    private MeshRenderer material;          //光らすため

    private void Start()
    {
        material = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        //どちらもステージに当たっているか
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
        //エレキボールに当たれば消す
        if (other.gameObject.CompareTag("ElectricalBall"))
        {
            hit_count = 0;
            Destroy(other.gameObject);
        }
    }
}