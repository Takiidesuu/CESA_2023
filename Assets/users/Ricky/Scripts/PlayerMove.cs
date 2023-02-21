using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    enum SMASHSTATE
    {
        NORMAL = 0,     //通常状態
        HOLDING,        //力を溜めてる状態
        SMASHING,       //力を放ってる状態
    }
    
    enum SMASHLEVEL
    {
        NONE,       //溜めなし
        SMALL,      //溜め小
        BIG,        //溜め大
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
    private ConstantForce constantForce;    //カスタム重力用のコンスタントフォース
    
    private MainInputControls input_system;
    
    private bool is_grounded;       //地面についているか
    private bool is_holding_smash;  //叩く力を貯めているか
    
    private GameObject camera_obj;  //カメラオブジェクト
    private GameObject hammer_obj;  //ハンマーオブジェクト
    
    private Vector2 input_direction;        //インプット方向
    private SMASHSTATE smash_state;         //プレイヤーの叩く状態
    private float turn_smooth_velocity;     //回転速度
    private float smash_power_num;          //叩く力の数値
    private SMASHLEVEL smash_power_level;   //叩く力の段階
    
    public ParticleSystem partSystem;
    
    private void Awake() 
    {
        input_system = new MainInputControls();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();             //リギッドボディー取得
        col = GetComponent<CapsuleCollider>();      //コライダー取得
        
        camera_obj = GameObject.FindGameObjectWithTag("MainCamera");    //カメラオブジェクト取得
        hammer_obj = GameObject.FindGameObjectWithTag("Hammer");        //ハンマーオブジェクトを取得
        
        input_system.Player.Smash.performed += HoldSmash;
        input_system.Player.Smash.canceled += ReleaseSmash;
        
        //変数を初期化する
        is_grounded = false;
        input_direction = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        //インプット方向を取得
        input_direction = input_system.Player.WASD.ReadValue<Vector2>();
    }
    
    void FixedUpdate() 
    {
        //叩く状態によって、更新を変える
        switch (smash_state)
        {
            case SMASHSTATE.NORMAL:     //通常状態
            
            //インプット方向があったら、移動させる
            if (input_direction != Vector2.zero)
            {
                Move();
            }
            
            smash_power_num = 0.0f;
            
            var emis = partSystem.emission;
            emis.enabled = false;
            
            break;
            case SMASHSTATE.HOLDING:    //力を溜めてる状態
            
            var emisss = partSystem.emission;
            emisss.enabled = true;
            
            var mainColor = partSystem.main;
            
            //溜めた力を加算する
            if (smash_power_num >= 100.0f)
            {
                smash_power_num = 100.0f;
            }
            else
            {
                smash_power_num += Time.deltaTime * 33.0f;
            }
            
            //溜めた力によって、力の段階を変える
            if (smash_power_num >= 100.0f)
            {
                smash_power_level = SMASHLEVEL.BIG;
                mainColor.startColor = new Color(1.0f, 0.0f, 0.0f);
                emisss.rateOverTime = 100.0f;
            }
            else if (smash_power_num >= smash_threshold)
            {
                smash_power_level = SMASHLEVEL.SMALL;
                mainColor.startColor = new Color(0.0f, 1.0f, 0.0f);
                emisss.rateOverTime = 50.0f;
            }
            else
            {
                smash_power_level = SMASHLEVEL.NONE;
                mainColor.startColor = new Color(0.0f, 0.0f, 1.0f);
                emisss.rateOverTime = 10.0f;
            }
            
            break;
            case SMASHSTATE.SMASHING:   //力を放ってる状態
            
            break;
        }
        
        GravityForce();
    }
    
    void Move()
    {
        Vector3 direction = new Vector3(input_direction.x, 0.0f, input_direction.y).normalized;
        
        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + camera_obj.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turn_smooth_velocity, turn_smooth_time);
            transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);
            
            Vector3 move_dir = Quaternion.Euler(0.0f, targetAngle, 0.0f) * Vector3.forward;
            rb.velocity = new Vector3(move_dir.normalized.x * speed, rb.velocity.y, rb.velocity.z);
        }
    }
    
    void GravityForce()
    {
        
    }
    
    private void HoldSmash(InputAction.CallbackContext obj)
    {   
        //地面についていたら、力を溜める可能にする
        if (is_grounded && hammer_obj.GetComponent<HammerScript>().GetThrowState())
        {
            smash_state = SMASHSTATE.HOLDING;
        }
    }
    
    private void ReleaseSmash(InputAction.CallbackContext obj)
    {
        if (smash_state == SMASHSTATE.HOLDING)
        {
            switch (smash_power_level)
            {
                case SMASHLEVEL.NONE:
                rb.AddForce(Vector3.up * jump_power, ForceMode.Impulse);
                break;
                case SMASHLEVEL.SMALL:
                hammer_obj.GetComponent<HammerScript>().ThrowHammer();
                break;
                case SMASHLEVEL.BIG:
                
                break;
            }
            
            smash_state = SMASHSTATE.NORMAL;
        }
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
    
    private void OnEnable() 
    {
        input_system.Enable();
    }
    
    private void OnDisable() 
    {
        input_system.Disable();
    }
}