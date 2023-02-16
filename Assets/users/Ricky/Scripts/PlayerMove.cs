using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Tooltip("移動速度")]
    [SerializeField] private float speed = 5.0f;
    [Tooltip("ジャンプ力")]
    [SerializeField] private float jump_power = 4.0f;
    [Tooltip("回転の滑らかさ")]
    [SerializeField] private float turn_smooth_time = 1.0f;
    
    //コンポネント
    private Rigidbody rb;           //リギッドボディー
    private CapsuleCollider col;    //コライダー
    
    private bool is_grounded;       //地面についているか
    
    private GameObject camera_obj;  //カメラオブジェクト
    
    private float turn_smooth_velocity; //回転速度
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();     //リギッドボディー取得
        col = GetComponent<CapsuleCollider>();     //コライダー取得
        camera_obj = GameObject.FindGameObjectWithTag("MainCamera");
    }

    // Update is called once per frame
    void Update()
    {
        if (is_grounded && Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }
    
    void FixedUpdate() 
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0.0f, vertical).normalized;
        
        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + camera_obj.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turn_smooth_velocity, turn_smooth_time);
            transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);
            
            Vector3 move_dir = Quaternion.Euler(0.0f, targetAngle, 0.0f) * Vector3.forward;
            rb.velocity = new Vector3(move_dir.normalized.x * speed, rb.velocity.y, move_dir.normalized.z * speed);
        }
    }
    
    void Jump()
    {
        rb.AddForce(Vector3.up * jump_power * 2.0f, ForceMode.Impulse);
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