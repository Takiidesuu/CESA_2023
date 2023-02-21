using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerScript : MonoBehaviour
{
    private Rigidbody rb;       //リギッドボディー
    
    private GameObject player_obj;   //プレイヤーオブジェクト
   
    private float move_scalar;   //動くスピードのスキャラ―
    
    private GameObject player_hand_pos;    //プレイヤーの手の位置を示すオブジェクト
    
    private bool ready_to_throw;     //投げられたか
    
    public bool GetThrowState()
    {
        return ready_to_throw;
    }
    
    public void ThrowHammer()
    {
        var dir = player_obj.transform.forward.normalized;
        
        rb.AddForce(dir * 100.0f, ForceMode.Impulse);
        
        ready_to_throw = false;
        move_scalar = 2.0f;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        
        player_obj = GameObject.FindGameObjectWithTag("Player");
        player_hand_pos = player_obj.transform.GetChild(0).gameObject;
        
        Physics.IgnoreCollision(player_obj.GetComponent<Collider>(), this.GetComponent<Collider>(), true);
        
        move_scalar = 2.0f;
        
        ready_to_throw = false;
    }

    // Update is called once per frame
    void Update()
    {   
        if (Vector3.Distance(this.transform.position, player_hand_pos.transform.position) < 1.5f)
        {
            ready_to_throw = true;
            this.transform.position = player_hand_pos.transform.position;
        }
        else
        {
            if (rb.velocity == Vector3.zero)
            {
                move_scalar += Time.deltaTime * 4.0f;
                this.transform.position = Vector3.MoveTowards(this.transform.position, player_hand_pos.transform.position, Time.deltaTime * 80.0f * move_scalar);
            }
            else
            {   
                rb.velocity = Vector3.MoveTowards(rb.velocity, Vector3.zero, Time.deltaTime * 80.0f);
            }
        }
       
    }
}
