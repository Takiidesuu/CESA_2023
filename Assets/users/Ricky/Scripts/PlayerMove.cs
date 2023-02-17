using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    enum SMASHSTATE
    {
        NORMAL = 0,
        HOLDING,
        SMASHING,
        
        MAX
    }
    
    enum SMASHLEVEL
    {
        NONE,
        SMALL,
        BIG,
        
        MAX
    }
    
    [Header("Player Param")]
    [Tooltip("移動速度")]
    [SerializeField] private float speed = 5.0f;
    [Tooltip("回転の滑らかさ")]
    [SerializeField] private float turn_smooth_time = 1.0f;
    
    [Header("Smash Param")]
    [Tooltip("ジャンプ力")]
    [SerializeField] private float jump_power = 4.0f;
    [Tooltip("溜め小")]
    [SerializeField] private float smash_threshold = 50.0f;
    
    //コンポネント
    private Rigidbody rb;           //リギッドボディー
    private CapsuleCollider col;    //コライダー
    
    private bool is_grounded;       //地面についているか
    private bool is_holding_smash;  //叩く力を貯めているか
    
    private GameObject camera_obj;  //カメラオブジェクト
    
    private SMASHSTATE smash_state;         //プレイヤーの叩く状態
    private float turn_smooth_velocity;     //回転速度
    private float smash_power_num;          //叩く力の数値
    private SMASHLEVEL smash_power_level;   //叩く力の段階
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();             //リギッドボディー取得
        col = GetComponent<CapsuleCollider>();      //コライダー取得
        camera_obj = GameObject.FindGameObjectWithTag("MainCamera");
        
        is_grounded = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void FixedUpdate() 
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0.0f, vertical).normalized;
        
        if (direction.magnitude >= 0.1f && smash_state == SMASHSTATE.NORMAL)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + camera_obj.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turn_smooth_velocity, turn_smooth_time);
            transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);
            
            Vector3 move_dir = Quaternion.Euler(0.0f, targetAngle, 0.0f) * Vector3.forward;
            rb.velocity = new Vector3(move_dir.normalized.x * speed, rb.velocity.y, move_dir.normalized.z * speed);
        }
        
        if (is_grounded)
        {
            if (Input.GetKey(KeyCode.Space) && smash_state != SMASHSTATE.SMASHING)
            {
                is_holding_smash = true;
                smash_state = SMASHSTATE.HOLDING;
            }
            
            if (Input.GetKeyUp(KeyCode.Space) && smash_state == SMASHSTATE.HOLDING)
            {
                smash_state = SMASHSTATE.SMASHING;
            }
            
            if (smash_state == SMASHSTATE.SMASHING)
            {
                Jump();
                smash_state = SMASHSTATE.NORMAL;
            }
        }
        else
        {
            is_holding_smash = false;
        }
        
        if (is_holding_smash)
        {
            if (smash_power_num >= 100.0f)
            {
                smash_power_num = 100.0f;
            }
            else
            {
                smash_power_num += Time.deltaTime * 10.0f;
            }
        }
        else
        {
            smash_power_num = 0.0f;
        }
    }
    
    void Jump()
    {
        rb.AddForce(Vector3.up * jump_power * 10.0f * (smash_power_num * 2.0f / 100.0f), ForceMode.Impulse);
    }
    
    private void OnCollisionEnter(Collision other) 
    {
        if (other.gameObject.layer == 6)
        {
            is_grounded = true;
        }
    }
    
    private void OnCollisionStay(Collision other) 
    {
        if (other.gameObject.layer == 6)
        {
            is_grounded = true;
        }
    }
    
    private void OnCollisionExit(Collision other) 
    {
        if (other.gameObject.layer == 6)
        {
            is_grounded = false;
        }
    }
}