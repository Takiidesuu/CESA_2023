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
    private Rigidbody rb;                   //リギッドボディー
    private CapsuleCollider col;            //コライダー
    private ConstantForce gravity_force;    //重力用のコンスタントフォース
    
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

    [Tooltip("重力")]
    [SerializeField] private float downward_force = 12.0f;
    [Tooltip("重力の加速")]
    [SerializeField] private float gravity_acceleration = 2.0f;
    private float actual_gravity;

    [Tooltip("回転速度")]
    [SerializeField] private float rotation_speed = 20.0f;
    
    private float y_angle = 90.0f;
    
    public ParticleSystem partSystem;
    private Vector3 ground_dir;
    private Vector3 target_dir;

    /// <summary>
    /// 平田
    /// </summary>
    private DeformStage deform_stage;

    private void Awake() 
    {
        input_system = new MainInputControls();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();                 //リギッドボディー取得
        col = GetComponent<CapsuleCollider>();          //コライダー取得
        gravity_force = GetComponent<ConstantForce>();  //コンスタントフォース取得
        deform_stage = GameObject.FindWithTag("Stage").GetComponent<DeformStage>();

        camera_obj = GameObject.FindGameObjectWithTag("MainCamera");    //カメラオブジェクト取得
        hammer_obj = GameObject.FindGameObjectWithTag("Hammer");        //ハンマーオブジェクトを取得
        
        input_system.Player.Smash.performed += HoldSmash;
        input_system.Player.Smash.canceled += ReleaseSmash;
        
        //変数を初期化する
        is_grounded = false;
        input_direction = Vector2.zero;
        
        target_dir = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        //インプット方向を取得
        input_direction = input_system.Player.WASD.ReadValue<Vector2>();
        
        CheckIsGrounded();
    }
    
    void FixedUpdate() 
    {   
        //RotateToAdjustToGroundSlope(hit);
        
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
        
        Vector3 set_ground_dir = CheckFloorAngle();
        ground_dir = Vector3.Lerp(ground_dir, set_ground_dir, Time.deltaTime * rotation_speed);
        ground_dir = set_ground_dir;
        
        Vector3 gravity_dir;
        
        RaycastHit hit;
        if (Physics.Raycast(this.transform.position - this.transform.up * 0.25f, -ground_dir.normalized, out hit, 10.0f, LayerMask.GetMask("Ground")))
        {
            gravity_dir = this.transform.position - hit.point;
        }
        else
        {
            gravity_dir = transform.up;
        }
        
        gravity_force.relativeForce = gravity_dir.normalized * -9.81f * 3.0f;
        
        gravity_dir = Vector3.ProjectOnPlane(transform.forward, hit.normal).normalized;
        
        Vector3 new_rot = Vector3.RotateTowards(transform.up, gravity_dir, 50.0f, 0.0f);
        transform.rotation = Quaternion.LookRotation(gravity_dir);
    }
    
    private void LateUpdate() 
    {
        //this.transform.localEulerAngles = new Vector3(this.transform.localEulerAngles.x, this.transform.localEulerAngles.y, 0.0f);
    }
    
    void Move()
    {
        Vector3 direction = new Vector3(input_direction.x, 0.0f, 0.0f).normalized;
        
        if (direction.magnitude >= 0.1f)
        {   
            Vector3 targetDirection = new Vector3(direction.x, 0.0f, 0.0f);
            targetDirection = Camera.main.transform.TransformDirection(targetDirection);
            
            rb.velocity = new Vector3(targetDirection.normalized.x * speed, rb.velocity.y, rb.velocity.z);
            
            target_dir = direction;
        }
        
        transform.rotation = Quaternion.LookRotation(target_dir.normalized, transform.up);
    }
    
    private void CheckIsGrounded()
    {
        LayerMask ground_layer_mask = LayerMask.GetMask("Ground");
        RaycastHit hit;
        
        is_grounded = Physics.Raycast(this.transform.position - this.transform.up, this.transform.up * -1.0f, out hit, 1.0f, ground_layer_mask);
    }
    
    private Vector3 CheckFloorAngle()
    {
        LayerMask ground_layer_mask = LayerMask.GetMask("Ground");
        
        RaycastHit hit_front;
        RaycastHit hit_centre;
        RaycastHit hit_back;
        
        Vector3 feet_pos = this.transform.position - this.transform.up * 0.2f;
        Vector3 dir_offset_front = this.transform.up * -1.0f + this.transform.forward * 0.2f;
        dir_offset_front.Normalize();
        Vector3 dir_offset_back = this.transform.up * -1.0f - this.transform.forward * 0.2f;
        dir_offset_back.Normalize();
        
        bool found_ground = false;
        
        found_ground = Physics.Raycast(feet_pos + this.transform.forward * 0.2f, dir_offset_front, out hit_front, 10.0f, ground_layer_mask);
        
        if (!found_ground)
        {
            found_ground = Physics.Raycast(feet_pos, this.transform.up * -1.0f, out hit_centre, 10.0f, ground_layer_mask);
        }
        else
        {
            Physics.Raycast(feet_pos, this.transform.up * -1.0f, out hit_centre, 10.0f, ground_layer_mask);
        }
        
        if (!found_ground)
        {
            found_ground = Physics.Raycast(feet_pos - this.transform.forward * 0.2f, dir_offset_back, out hit_back, 10.0f, ground_layer_mask);
        }
        else
        {
            Physics.Raycast(feet_pos - this.transform.forward * 0.2f, dir_offset_back, out hit_back, 10.0f, ground_layer_mask);
        }
        
        Debug.DrawRay(feet_pos + this.transform.forward * 0.2f, dir_offset_front, Color.red);
        Debug.DrawRay(feet_pos, this.transform.up * -1.0f, Color.red);
        Debug.DrawRay(feet_pos - this.transform.forward * 0.2f, dir_offset_back, Color.red);
        
        Vector3 hit_dir = transform.up;
        
        if (found_ground)
        {
            if (hit_front.transform != null)
            {
                hit_dir += hit_front.normal;
            }
            if (hit_centre.transform != null)
            {
                hit_dir += hit_centre.normal;
            }
            if (hit_back.transform != null)
            {
                hit_dir += hit_back.normal;
            }
        }
        else
        {
            float sphere_size = 40.0f;
            Collider[] col_info = Physics.OverlapSphere(this.transform.position, sphere_size, ground_layer_mask);
            float distance_check = 40.0f;
            foreach (var current in col_info)
            {
                RaycastHit ground_hit;
                if (Physics.Raycast(this.transform.position, current.transform.position, out ground_hit, 40.0f, ground_layer_mask))
                {
                    float distance_to_ground = Vector3.Distance(this.transform.position, ground_hit.point);
                    if (distance_to_ground < distance_check)
                    {
                        hit_dir = this.transform.position - ground_hit.point;
                        distance_check = distance_to_ground;
                    }
                }
            }
        }
        
        Debug.DrawLine(transform.position, transform.position + (hit_dir.normalized * 5f), Color.blue);
        
        return hit_dir.normalized;
    }
    
    private void RotateToAdjustToGroundSlope(RaycastHit hitInfo)
    {
        Vector3 normal = hitInfo.normal;
 
        //Normal
        Debug.DrawLine(rb.position, rb.position + normal * 3, Color.cyan);
 
        Vector3 rotationInEuler = DecomposeRotationInAxis(normal);
 
        //this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.Euler(rotationInEuler), rotation_speed * Time.deltaTime);
    }
 
    private Vector3 DecomposeRotationInAxis(Vector3 normal)
    {
        Vector3 projectionOnZplane = Vector3.ProjectOnPlane(normal, this.transform.forward);
        Vector3 projectionOnXplane = Vector3.ProjectOnPlane(normal, this.transform.right);
 
        Debug.DrawLine(rb.position + normal * 3, rb.position + normal * 3 - projectionOnZplane * 3, Color.cyan);
        Debug.DrawLine(rb.position + normal * 3, rb.position + normal * 3 - projectionOnXplane * 3, Color.magenta);
 
        float xAngleRotation = Vector3.SignedAngle(projectionOnZplane, normal, this.transform.right);
        float zAngleRotation = Vector3.SignedAngle(projectionOnXplane, normal, this.transform.forward);
 
        return new Vector3(xAngleRotation, y_angle, 0.0f);
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
                deform_stage.AddDeformpointDown(transform, transform.eulerAngles.z);
                rb.AddForce(this.transform.up * jump_power, ForceMode.Impulse);
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