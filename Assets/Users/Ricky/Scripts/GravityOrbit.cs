using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityOrbit : MonoBehaviour
{
    public float gravity;
    
    [HideInInspector] public bool fixed_direction;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnTriggerEnter(Collider other) 
    {
        //当たってるオブジェクトは重力の処理のスクリプトがあったら
        if (other.GetComponent<GravityControl>())
        {
            //この惑星を中心にする
            other.GetComponent<GravityControl>().gravity = this.GetComponent<GravityOrbit>();
        }
    }
}
