using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
    
    enum MOVEDIR
    {
        NONE,
        UP,
        RIGHT,
        DOWN,
        LEFT,
    }
    
    [Header("Player Param")]
    [Tooltip("移動速度")]
    [SerializeField] private float speed = 5.0f;
    [Tooltip("減速速度")]
    [SerializeField] private float deceleration_speed = 5.0f;
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
    
    private MainInputControls input_system;
    private bool input_check_pos;
    private float move_value;
    private float input_modifier;
    private Vector2 move_dir;
    
    private bool is_grounded;       //地面についているか
    private GameObject ground_obj;  //ついてる地面
    private bool is_holding_smash;  //叩く力を貯めているか
    
    private GameObject camera_obj;  //カメラオブジェクト
    private GameObject hammer_obj;  //ハンマーオブジェクト
    
    private Vector2 input_direction;        //インプット方向
    private SMASHSTATE smash_state;         //プレイヤーの叩く状態
    private float smash_power_num;          //叩く力の数値
    private SMASHLEVEL smash_power_level;   //叩く力の段階
    
    public ParticleSystem partSystem;

    /// <summary>
    /// 平田
    /// </summary>
    private DeformStage deform_stage;
    private bool is_flip;
    
    public bool GetSmashingState()
    {
        return smash_state != SMASHSTATE.NORMAL ? true : false;
    }
    
    public GameObject GetGroundObj()
    {
        return ground_obj;
    }

    private void Awake() 
    {
        input_system = new MainInputControls();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();                 //リギッドボディー取得
        col = GetComponent<CapsuleCollider>();          //コライダー取得
        deform_stage = GameObject.FindWithTag("Stage").GetComponent<DeformStage>();

        camera_obj = GameObject.FindGameObjectWithTag("MainCamera");    //カメラオブジェクト取得
        hammer_obj = GameObject.FindGameObjectWithTag("Hammer");        //ハンマーオブジェクトを取得
        
        input_system.Player.Smash.performed += HoldSmash;
        input_system.Player.Smash.canceled += ReleaseSmash;
        input_system.Player.Flip.performed += FlipCharacter;
        input_system.Player.Rotate.performed += RotateGround;
        
        //プロト用インプット
        input_system.Prototype.ReloadScene.performed += ProtoReloadScene;
        input_system.Prototype.EndScene.performed += ProtoEndScene;
        
        //変数を初期化する
        is_grounded = false;
        is_flip = false;
        input_direction = Vector2.zero;
        input_check_pos = true;
        move_value = 0.0f;
        move_dir = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        //インプット方向を取得
        input_direction = input_system.Player.WASD.ReadValue<Vector2>();
        
        CheckIsGrounded();
        
        if (rb.velocity.magnitude > 0.0f && is_grounded && input_direction != Vector2.zero)
        {
            rb.velocity = Vector3.MoveTowards(rb.velocity, Vector3.zero, deceleration_speed * Time.deltaTime * 4.0f);
        }

        //Debug.Log(transform.localEulerAngles);
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
            else
            {
                input_check_pos = true;
                move_dir = Vector2.zero;
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
    }
    
    void Move()
    {
        Vector3 direction = new Vector3(input_direction.x, input_direction.y, 0.0f);
        
        if (direction.magnitude >= 0.015f)
        { 
            if (Mathf.Abs(direction.x) >= Mathf.Abs(direction.y))
            {
                direction.y = 0.0f;
            }
            else
            {
                direction.x = 0.0f;
            }
            
            Vector2 norm_input = direction.normalized;
            
            if (norm_input != move_dir)
            {
                if (input_check_pos)
                {
                    //左右移動
                    if (Mathf.Abs(norm_input.x) > 0.0f)
                    {
                        //右のほうにいる
                        if (Camera.main.WorldToScreenPoint(this.transform.position).x >= Screen.width / 2)
                        {
                            //下の方にいる
                            if (Camera.main.WorldToScreenPoint(this.transform.position).y <= Screen.height / 2)
                            {
                                move_value = -direction.x;
                            }
                            else    //上の方にいる
                            {
                                move_value = direction.x;
                            }
                        }
                        else    //左の方にいる
                        {
                            //下の方にいる
                            if (Camera.main.WorldToScreenPoint(this.transform.position).y <= Screen.height / 2)
                            {
                                move_value = -direction.x;
                            }
                            else
                            {
                                move_value = direction.x;
                            }
                        }
                    }
                    else    //上下移動
                    {
                        //右のほうにいる
                        if (Camera.main.WorldToScreenPoint(this.transform.position).x >= Screen.width / 2)
                        {
                            //下の方にいる
                            if (Camera.main.WorldToScreenPoint(this.transform.position).y <= Screen.height / 2)
                            {
                                move_value = -direction.y;
                            }
                            else
                            {
                                move_value = -direction.y;
                            }
                        }
                        else
                        {
                            //下の方にいる
                            if (Camera.main.WorldToScreenPoint(this.transform.position).y <= Screen.height / 2)
                            {
                                move_value = direction.y;
                            }
                            else
                            {
                                move_value = direction.y;
                            }
                        }
                    }
                    
                    input_check_pos = false;
                }
                
                move_dir = norm_input;
            }
            
            Vector3 targetDirection = new Vector3(direction.x * input_modifier, 0.0f, 0.0f);
            targetDirection = Camera.main.transform.TransformDirection(targetDirection);
            
            var locVel = transform.InverseTransformDirection(rb.velocity);
            locVel.x = move_value * speed;
            rb.velocity = transform.TransformDirection(locVel);
        }
    }
    
    private void CheckIsGrounded()
    {
        LayerMask ground_layer_mask = LayerMask.GetMask("Ground");
        RaycastHit hit;
        
        if (Physics.Raycast(this.transform.position + this.transform.up * 0.25f, this.transform.up * -1.0f, out hit, 2.0f, ground_layer_mask))
        {
            is_grounded = true;
            ground_obj = hit.transform.gameObject;
        }
        else
        {
            is_grounded = false;
        }
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
                    if (deform_stage)
                        deform_stage.AddDeformpointDown(transform, transform.eulerAngles.z,is_flip);
                break;
                case SMASHLEVEL.SMALL:
                    hammer_obj.GetComponent<HammerScript>().ThrowHammer();
                    rb.AddForce(this.transform.up * jump_power, ForceMode.Impulse);
                break;
                case SMASHLEVEL.BIG:
                
                break;
            }
            
            smash_state = SMASHSTATE.NORMAL;
        }
    }
    
    private void FlipCharacter(InputAction.CallbackContext obj)
    {
        RaycastHit hit_info;
        if (Physics.Raycast(this.transform.position + this.transform.up * 0.25f, this.transform.up * -1.0f, out hit_info, 5.0f, LayerMask.GetMask("Ground")))
        {
            float dis = Vector3.Distance(this.transform.position, hit_info.point);
            Vector3 new_pos;
            
            while (true)
            {
                Vector3 check_pos = this.transform.position + -this.transform.up * dis;
                Collider[] hit_col = Physics.OverlapSphere(check_pos, 0.5f, LayerMask.GetMask("Ground"));
                if (hit_col.Length == 0)
                {
                    new_pos = check_pos;
                    break;
                }
                else
                {
                    dis += 1.0f;
                }
            }
            
            transform.Rotate(new Vector3(180.0f, 0.0f, 0.0f), Space.World);
            this.transform.position = new_pos;
            if (is_flip) is_flip = false; else is_flip = true;
        }
    }
    
    private void RotateGround(InputAction.CallbackContext obj)
    {
        if (is_grounded)
        {
            ground_obj.transform.root.gameObject.GetComponent<StageRotation>().StartRotate();
        }
    }
    
    private void ProtoReloadScene(InputAction.CallbackContext obj)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    private void ProtoEndScene(InputAction.CallbackContext obj)
    {
        Application.Quit();
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