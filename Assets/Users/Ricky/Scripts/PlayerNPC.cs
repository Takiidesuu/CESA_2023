using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNPC : MonoBehaviour
{
    enum MOVEDIR
    {
        RIGHT,
        LEFT
    }
    
    [Tooltip("移動速度")]
    [SerializeField] private float move_speed = 5.0f;
    [Tooltip("移動方向")]
    [SerializeField] private MOVEDIR move_dir = MOVEDIR.RIGHT;
    
    private Rigidbody rb;
    private Animator anim;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = transform.GetChild(0).GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("isWalking", rb.velocity != Vector3.zero);
    }
    
    private void FixedUpdate() 
    {
        if (move_dir == MOVEDIR.RIGHT)
        {
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 0.0f, transform.localEulerAngles.z);
        }
        else
        {
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 180.0f, transform.localEulerAngles.z);
        }
        
        var locVel = transform.InverseTransformDirection(rb.velocity);
        locVel.x = move_speed;
        rb.velocity = transform.TransformDirection(locVel);
    }
}
