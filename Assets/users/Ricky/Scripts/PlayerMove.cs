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
    
    enum SMASHJUMP
    {
        NONE,           //ジャンプできない
        CAN_JUMP,       //ジャンプできる
    }
    
    enum SMASHPOWERLEVEL
    {
        NORMAL,
        BIG,
        MAX
    }
    
    [Header("Player Param")]
    [Tooltip("加速度")]
    [SerializeField] private float acceleration_speed = 2.0f;
    [Tooltip("最高移動速度")]
    [SerializeField] private float max_speed = 500.0f;
    [Tooltip("減速速度")]
    [SerializeField] private float deceleration_speed = 5.0f;
    
    [Header("Smash Param")]
    [Tooltip("ジャンプ力")]
    [SerializeField] private float jump_power = 4.0f;
    [Tooltip("ジャンプできるように溜める秒数")]
    [SerializeField] private float smash_threshold = 2.0f;
    [Tooltip("溜める最大秒数")]
    [SerializeField] private float smash_max_time = 5.0f;
    [Tooltip("叩く力")]
    [SerializeField] private float smash_power_scalar = 3.0f;
    [Tooltip("１段階目の力の乗算")]
    [SerializeField] private float increase_power_scalar_one = 2.0f;
    [Tooltip("マックスの力の乗算")]
    [SerializeField] private float increase_power_scalar_max = 4.0f;
    private SMASHPOWERLEVEL power_level = SMASHPOWERLEVEL.NORMAL;
    
    [Header("Vibration param")]
    [Tooltip("溜めてる時の震度")]
    [SerializeField] private float hold_smash_vibration = 1.0f;
    [Tooltip("叩く時の震度")]
    [SerializeField] private float smash_vibration = 3.0f;
    [Tooltip("カメラの震度")]
    [SerializeField] private float camera_vibration = 2.0f;
 
    [Header("Prefab")]
    [Tooltip("火花")]
    [SerializeField] private GameObject spark_effect;
    
    //コンポネント
    private Rigidbody rb;                   //リギッドボディー
    private CapsuleCollider col;            //コライダー
    private Animator anim;                  //アニメーター

    private float speed;
    
    private bool recheck_input;
    private Vector2 prev_input;
    private bool is_dead;
    
    private bool is_grounded;       //地面についているか
    private GameObject ground_obj;  //ついてる地面
    private GameObject ground_obj_parent;   //ついてる地面の親（コンポネント取る用）
    LayerMask ground_layer_mask;
    private bool is_holding_smash;  //叩く力を貯めているか
    
    private GameObject camera_obj;  //カメラオブジェクト
    
    private Vector2 input_direction;        //インプット方向
    private SMASHSTATE smash_state;         //プレイヤーの叩く状態
    private float smash_power_num;          //叩く力の数値
    private SMASHJUMP can_jump_status;   //叩く力の段階

    private SoundManager soundmanager;
    
    private bool play_small_charge;
    private bool play_max_charge;

    [Tooltip("線のパーティクル")]
    [SerializeField] private ParticleSystem part_line_sys;
    [Tooltip("丸いのパーティクル")]
    [SerializeField] private ParticleSystem part_circle_sys;
    private ParticleSystem.EmissionModule part_line_effect;
    private ParticleSystem.EmissionModule part_circle_effect;
    
    public bool in_grav_field {get; private set;}
    private GameObject grav_obj;
    
    private LightBulbCollector check_is_cleared;
    
    private float y_rot_record;
    
    public bool start_game {get; set;}
    public bool taking_damage {get; set;}

    /// <summary>
    /// 平田
    /// </summary>
    Quaternion anirotate;
    private DeformStage deform_stage;
    private WallSwitch wall_switch;
    private MinMaxDeform min_max_deform;
    private bool is_flip;
    
    public void TookDamage(float damage_time)
    {
        anirotate = transform.rotation;
        anim.speed = 0.0f;
        anim.SetTrigger("takeDamage");
        
        soundmanager.PlaySoundEffect("TakeDamage");
        
        float damage_time_scaled = damage_time / 1.4f;
        StartCoroutine(StartDamageTimer(damage_time_scaled));
        InputManager.instance.VibrateController(damage_time_scaled, 0.1f);
        
        if (smash_state == SMASHSTATE.HOLDING)
        {
            smash_state = SMASHSTATE.NORMAL;
        }
    }
    
    public bool GetSmashingState()
    {
        return smash_state != SMASHSTATE.NORMAL ? true : false;
    }
    
    public GameObject GetGroundObj()
    {
        return ground_obj != null ? ground_obj : null;
    }
    
    public GameObject GetCurrentGravObj()
    {
        return grav_obj != null ? grav_obj : null;
    }

    public WallSwitch GetWallswich()
    {
        return wall_switch;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();                 //リギッドボディー取得
        col = GetComponent<CapsuleCollider>();          //コライダー取得
        anim = transform.GetChild(0).GetComponent<Animator>();                //アニメーター取得

        part_line_effect = part_line_sys.emission;
        part_circle_effect = part_circle_sys.emission;
        
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        camera_obj = GameObject.FindGameObjectWithTag("MainCamera");    //カメラオブジェクト取得
        
        ground_layer_mask = LayerMask.GetMask("Ground");
        
        //変数を初期化する
        is_grounded = false;
        is_flip = false;
        input_direction = Vector2.zero;
        speed = 0.0f;
        recheck_input = false;
        prev_input = Vector2.zero;
        is_dead = false;
        
        smash_vibration = Mathf.Clamp(smash_vibration, 1.0f, 5.0f);
        hold_smash_vibration = Mathf.Clamp(hold_smash_vibration, 0.1f, 10.0f);
        
        in_grav_field = false;

        //soundmannagerを取得
        soundmanager = GetComponent<SoundManager>();
        
        play_small_charge = true;
        play_max_charge = true;
        
        check_is_cleared = GameObject.FindObjectOfType<LightBulbCollector>();
        
        start_game = false;
    }

    // Update is called once per frame
    void Update()
    {   
        if (!check_is_cleared.IsCleared())
        {
            is_dead = GameOverManager.instance.game_over_state;
            
            // 生きている場合
            if (!is_dead)
            {
                CheckIsGrounded();
                CheckSide();
                
                if (!taking_damage && start_game && in_grav_field)
                {
                    if (ground_obj != null)
                    {
                        if (!ground_obj_parent.gameObject.GetComponent<StageRotation>().GetRotatingStatus())
                        {
                            UpdateSmash();
                            
                            if (is_grounded)
                            {
                                if (InputManager.instance.press_smash)
                                {
                                    HoldSmash();
                                }
                                else
                                {
                                    ReleaseSmash();
                                }
                                
                                if (InputManager.instance.press_flip)
                                {
                                    FlipCharacter();
                                }
                                
                                if (InputManager.instance.press_rotate)
                                {
                                    RotateGround();
                                }
                            }
                            
                            if (smash_state == SMASHSTATE.NORMAL)
                            {
                                //インプット方向を取得
                                input_direction = InputManager.instance.player_move_float;
                            }
                        }
                    }
                }
                else
                {
                    if (taking_damage)
                    {
                        speed = 0;
                        rb.velocity = Vector3.zero;
                    }
                    
                    input_direction = Vector2.zero;
                }
                
                speed = Mathf.MoveTowards(speed, input_direction.magnitude * max_speed, acceleration_speed);
                
                if (rb.velocity.magnitude > 0.0f && is_grounded && smash_state == SMASHSTATE.NORMAL)
                {
                    if (input_direction == Vector2.zero || recheck_input)
                    {
                        DecelerateSpeed();
                    }
                    else
                    {
                        if (!soundmanager.CheckIsPlaying("Walk"))
                        {
                            soundmanager.PlaySoundEffect("Walk");
                        }
                    }
                }
                else
                {
                    soundmanager.StopSoundEffect("Walk");
                }
            }
            else
            {
                anim.SetBool("isWalking", false);
            }
        }
    }
    
    void UpdateSmash()
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
                anim.SetBool("isWalking", false);
            }
            
            smash_power_num = 0.0f;
            
            part_line_effect.enabled = false;
            part_circle_effect.enabled = false;
            
            play_small_charge = true;
            play_max_charge = true;
            
            break;
            
            
            case SMASHSTATE.HOLDING:    //力を溜めてる状態
            
            part_line_effect.enabled = true;
            part_circle_effect.enabled = true;
            
            var line_color = part_line_sys.main;
            var circle_color = part_circle_sys.main;
            
            //溜めた力を加算する
            if (smash_power_num >= smash_max_time)
            {
                smash_power_num = smash_max_time;
            }
            else
            {
                smash_power_num += Time.deltaTime;
            }
            
            if (smash_power_num >= smash_threshold)
            {
                can_jump_status = SMASHJUMP.CAN_JUMP;
                
                if (smash_power_num >= smash_max_time)
                {
                    line_color.startColor = new Color(1.0f, 0.0f, 0.0f);
                    circle_color.startColor = new Color(1.0f, 0.0f, 0.0f);
                    
                    if (play_max_charge)
                    {
                        soundmanager.PlaySoundEffect("Charge_1");
                        play_max_charge = false;
                    }
                    
                    power_level = SMASHPOWERLEVEL.MAX;
                }
                else
                {
                    power_level = SMASHPOWERLEVEL.BIG;
                    line_color.startColor = new Color(0.0f, 1.0f, 0.0f);
                    circle_color.startColor = new Color(0.0f, 1.0f, 0.0f);
                    
                    if (play_small_charge)
                    {
                        soundmanager.PlaySoundEffect("Charge_0");
                        play_small_charge = false;
                    }
                }   
            }
            else
            {
                power_level = SMASHPOWERLEVEL.NORMAL;
                can_jump_status = SMASHJUMP.NONE;
                line_color.startColor = new Color(0.0f, 0.0f, 1.0f);
                circle_color.startColor = new Color(0.0f, 0.0f, 1.0f);
            }
            
            part_line_effect.rateOverTime = 90.0f * (smash_power_num / smash_threshold);
            circle_color.startSize = 8.0f * (smash_power_num / smash_threshold);
            
            InputManager.instance.VibrateController(Time.deltaTime, 0.1f);
            camera_obj.GetComponent<CameraMove>().ShakeCamera(0.1f, Time.deltaTime, this.transform.up);
            
            break;
            
            
            case SMASHSTATE.SMASHING:   //力を放ってる状態
            
            part_line_effect.enabled = false;
            part_circle_effect.enabled = false;
            
            break;
        }
    }
    
    void Move()
    {
        Vector2 norm_input = input_direction.normalized;
        Vector3 move_point = new Vector3(norm_input.x, norm_input.y, 0.0f);
        
        Debug.DrawRay(this.transform.position, move_point.normalized * 10.0f, Color.yellow);
        Debug.DrawRay(this.transform.position, transform.right * 10.0f, Color.green);
        
        float angle_difference = Mathf.Abs(Vector3.SignedAngle(transform.right, move_point.normalized, new Vector3(0, 0, 1)));
        if (!recheck_input)
        {
            var locVel = transform.InverseTransformDirection(rb.velocity);
        
            if (angle_difference < 87.0f || angle_difference > 93.0f)
            {
                prev_input = norm_input;
                
                // インプットの方向はプレイヤーの後ろ方向の場合は、プレイヤーを反転する
                if (angle_difference > 93.0f)
                {   
                    transform.Rotate(new Vector3(0.0f, 180.0f, 0.0f), Space.Self);
                }
                
                if (angle_difference < 87.0f)
                {
                    float reduce_speed_scalar = 25.0f;
                    locVel.x = speed / reduce_speed_scalar;
                    
                    anim.SetBool("isWalking", true);
                }
            }
            else
            {
                recheck_input = true;
                anim.SetBool("isWalking", false);
            }
            
            rb.velocity = transform.TransformDirection(locVel);
        }
        else
        {
            if (norm_input != prev_input)
            {
                recheck_input = false;
            }
        }
    }
    
    private void DecelerateSpeed()
    {
        rb.velocity = Vector3.MoveTowards(rb.velocity, Vector3.zero, deceleration_speed * Time.deltaTime * 4.0f);
        speed = Mathf.MoveTowards(speed, 0.0f, deceleration_speed * 0.5f);
    }
    
    private void LateUpdate() 
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }
    
    private void CheckIsGrounded()
    {
        RaycastHit hit;
        if (Physics.Raycast(this.transform.position + this.transform.up * 0.25f, this.transform.up * -1.0f, out hit, 1.5f, ground_layer_mask))
        {
            is_grounded = true;
            ground_obj = hit.transform.gameObject;
            ground_obj_parent = ground_obj.transform.root.gameObject;
            deform_stage = ground_obj_parent.GetComponent<DeformStage>();
            min_max_deform = ground_obj_parent.GetComponent<MinMaxDeform>();
        }
        else
        {
            is_grounded = false;
        }
    }
    
    private void CheckSide()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, this.transform.up * -1.0f, out hit, 2.0f, ground_layer_mask))
        {
            Vector3 center_vec = hit.point - hit.transform.root.gameObject.transform.position;
            float dot_to_center = Vector3.Dot(center_vec.normalized, transform.up);
            
            if (dot_to_center > 0)
            {
                is_flip = false;
            }
            else
            {
                is_flip = true;
            }
        }
    }
    
    private void HoldSmash()
    {   
        //地面についていたら、力を溜める可能にする
        if (is_grounded && !is_dead && !taking_damage)
        {
            if (!ground_obj_parent.GetComponent<StageRotation>().GetRotatingStatus())
            {
                smash_state = SMASHSTATE.HOLDING;
                anim.SetTrigger("holdSmash");
            }
        }
    }
    
    private void ReleaseSmash()
    {
        if (smash_state == SMASHSTATE.HOLDING)
        {
            smash_state = SMASHSTATE.SMASHING;
            
            bool isSmash = true;
            if (is_flip)
            {
                if (min_max_deform != null) 
                  if (!min_max_deform.GetMaxHit())
                    isSmash = false;
            }
            else
            {
                if (min_max_deform != null)
                    if (min_max_deform.GetMinHit())
                     isSmash = false;
            }
            
            if (isSmash)
            {
                if (smash_power_num >= smash_max_time)
                {
                    anim.SetTrigger("isSmashStrong");
                }
                else
                {
                    y_rot_record = transform.localEulerAngles.y;
                    if (y_rot_record > -1 && y_rot_record < 1)
                    {
                        transform.GetChild(0).transform.localEulerAngles = new Vector3(0, 180, 0);
                    }
                    else
                    {
                        transform.GetChild(0).transform.localEulerAngles = new Vector3(0, 0, 0);
                    }
                    
                    transform.GetChild(0).transform.localPosition = new Vector3(0, -0.659f, -7.5f);
            
                    if (smash_power_num < smash_power_scalar)
                    {
                        anim.speed = 1.5f;
                    }
                    anim.SetTrigger("isSmash");
                }
            }
            else
            {
                anim.SetTrigger("failSmash");
                anim.ResetTrigger("holdSmash");
                
                y_rot_record = transform.GetChild(0).transform.localEulerAngles.y;
                transform.GetChild(0).transform.localEulerAngles = new Vector3(0, 180, 0);
                transform.GetChild(0).transform.localPosition = new Vector3(0, -0.659f, 3.75f);
            }
        }
    }
    
    public void BeforeSmashFunc()
    {
        float hit_stop_delay = 0.1f + (smash_power_num / smash_max_time) * 0.5f;
        
        HitstopManager.instance.StartHitStop(hit_stop_delay);
        
        float hit_stop_time = 0.15f * (smash_power_num / smash_max_time);
        
        camera_obj.GetComponent<CameraMove>().ShakeCamera(0.5f, hit_stop_time, this.transform.up);
        InputManager.instance.VibrateController(hit_stop_time, 0.3f);
    }

    public void SmashFunc()
    {
        if (can_jump_status == SMASHJUMP.CAN_JUMP)
        {
            var locVel = transform.InverseTransformDirection(rb.velocity);
            locVel.y = jump_power * (smash_power_num / smash_threshold);
            rb.velocity = transform.TransformDirection(locVel);
        }
        
        //叩くSEの再生
        soundmanager.PlaySoundEffect("Strike");

        if (deform_stage)
        {
            bool isSmash = true;
            if (is_flip)
            {
                if (min_max_deform != null)
                    if (!min_max_deform.GetMaxHit())
                        isSmash = false;
            }
            else
            {
                if (min_max_deform != null)
                    if (min_max_deform.GetMinHit())
                        isSmash = false;
            }

            if (isSmash)
            {
                float increase_power_scalar = 1;
                if (power_level == SMASHPOWERLEVEL.MAX)
                {
                    increase_power_scalar = increase_power_scalar_max;
                }
                else if (power_level == SMASHPOWERLEVEL.BIG)
                {
                    increase_power_scalar = increase_power_scalar_one;
                }
                
                Vector3 spawn_point = deform_stage.AddDeformpointDown(transform.position, transform.eulerAngles.y, (smash_power_num + 1) * smash_power_scalar * increase_power_scalar, is_flip);
                spawn_point -= this.transform.up;
                Instantiate(spark_effect, spawn_point, this.transform.rotation);
            }
        }
        else
        {
            if (wall_switch != null)
            {
                wall_switch.WallMove();
            }
        }
        
        smash_state = SMASHSTATE.NORMAL;
        anim.ResetTrigger("holdSmash");
        
        float vibration_dur = 0.05f + (smash_power_num / smash_max_time) * 0.2f;
        
        camera_obj.GetComponent<CameraMove>().ShakeCamera(smash_power_num / 2.0f * camera_vibration, vibration_dur, this.transform.up);
        InputManager.instance.VibrateController(vibration_dur, (0.1f * smash_vibration) + (smash_power_num / smash_max_time * 0.5f));
    }

    public void ResetAnim()
    {
        smash_state = SMASHSTATE.NORMAL;
        taking_damage = false;
        anim.speed = 1.0f;
    }
    
    public void ResetRot()
    {
        transform.GetChild(0).transform.localEulerAngles = new Vector3(0, 90, 0);
        transform.GetChild(0).transform.localPosition = new Vector3(0, -0.659f, 0);
    }
    
    public void FlipCharacter()
    {
        FlipUpsideDown();
    }
    
    private void FlipUpsideDown()
    {
        RaycastHit hit_info;
        if (Physics.Raycast(this.transform.position + this.transform.up * 0.25f, this.transform.up * -1.0f, out hit_info, 5.0f, ground_layer_mask))
        {
            float dis = Vector3.Distance(this.transform.position, hit_info.point);
            Vector3 new_pos;
            
            while (true)
            {
                Vector3 check_pos = this.transform.position + -this.transform.up * dis;
                Collider[] hit_col = Physics.OverlapSphere(check_pos, 2.0f, ground_layer_mask);
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
            
            transform.Rotate(new Vector3(180.0f, 0.0f, 0.0f), Space.Self);
            
            this.transform.position = new_pos;
        }
    }
    
    IEnumerator StartDamageTimer(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        anim.speed = 1.0f;
        transform.rotation = anirotate;
    }
    
    private void RotateGround()
    {
        if (is_grounded)
        {
            ground_obj_parent.GetComponent<StageRotation>().StartRotate();
        }
    }
    
    private void OnCollisionEnter(Collision other) 
    {
        if (other.gameObject.layer == 6)
        {
            is_grounded = true;
            if (other.gameObject.name == "Swich")
                wall_switch = other.gameObject.GetComponent<WallSwitch>();

        }
        if (other.gameObject.layer == LayerMask.GetMask("ElectricalBall"))
        {
            Physics.IgnoreCollision(col, other.gameObject.GetComponent<Collider>());
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
            if (other.gameObject.name == "Swich")
                wall_switch = null;
        }
    }
    
    private void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.tag == "FlipGate")
        {
            other.GetComponent<FlipGate>().Flip(this.gameObject);
        }

        if (other.gameObject.tag == "GravityField")
        {
            in_grav_field = true;
            grav_obj = other.gameObject;
        }
    }
    
    private void OnTriggerStay(Collider other) 
    {
        if (other.gameObject.tag == "GravityField")
        {
            in_grav_field = true;
            grav_obj = other.gameObject;
        }
    }
    
    private void OnTriggerExit(Collider other) 
    {
        if (other.gameObject.tag == "GravityField")
        {
            in_grav_field = false;
            grav_obj = null;
        }
    }
}