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
    
    [Tooltip("回転速度")]
    [SerializeField] private float rotation_speed = 5.0f;
    
    private Rigidbody rb;
    private Animator anim;
    
    private Vector3 origin_pos;
    private Vector3 target_pos;
    private Vector3 target_rotation;
    
    private float rotation_t;
    private float move_t;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = transform.GetChild(0).GetComponent<Animator>();
        
        target_pos = GameObject.FindObjectOfType<TitleEventTrigger>().gameObject.transform.position;
        target_pos.y = this.transform.position.y;
        
        origin_pos = this.transform.position;
        
        rotation_t = 0;
        move_t = 0;
        
        if (move_dir == MOVEDIR.RIGHT)
        {
            target_rotation = new Vector3(transform.localEulerAngles.x, 0.0f, transform.localEulerAngles.z);
        }
        else 
        {
            target_rotation = new Vector3(transform.localEulerAngles.x, 180.0f, transform.localEulerAngles.z);
        }
    }
    
    private void FixedUpdate() 
    {
        this.transform.position = Vector3.Lerp(origin_pos, target_pos, move_t);
        transform.localEulerAngles = Vector3.Lerp(target_rotation, new Vector3(transform.localEulerAngles.x, 90.0f, transform.localEulerAngles.z), rotation_t);
        
        if (move_t < 1)
        {
            this.transform.eulerAngles = target_rotation;
            
            anim.SetBool("isWalking", true);
            
            if (move_t < 1)
            {
                move_t += Time.deltaTime / 10.0f * move_speed;
            }
            else
            {
                move_t = 1;
            }
            
            if (InputManager.instance.press_select)
            {
                move_t = 1;
            }
        }
        else
        {   
            anim.SetBool("isWalking", false);
            
            if (rotation_t < 1)
            {
                rotation_t += Time.deltaTime * rotation_speed;
            }
            else
            {
                rotation_t = 1;
                
            }
            
            if (InputManager.instance.press_select)
            {
                rotation_t = 1;
            }
        }
    }
}
