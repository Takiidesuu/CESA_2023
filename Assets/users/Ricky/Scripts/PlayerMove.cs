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
    
    enum MOVEDIR
    {
        NONE,
        UP,
        RIGHT,
        DOWN,
        LEFT,
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
    [Header("叩く力")]
    [SerializeField] private float smash_power_scalar = 3.0f;
 
    [Header("プレハブ")]
    [Tooltip("火花")]
    [SerializeField] private GameObject spark_effect;
    
    private GameObject blackPanel;
    
    //コンポネント
    private Rigidbody rb;                   //リギッドボディー
    private CapsuleCollider col;            //コライダー
    private Animator anim;                  //アニメーター

    private bool input_check_pos;
    private float move_value;
    private float input_modifier;
    private Vector2 move_dir;
    private float speed;
    private bool is_dead;
    
    private bool is_grounded;       //地面についているか
    private GameObject ground_obj;  //ついてる地面
    private GameObject ground_obj_parent;   //ついてる地面の親（コンポネント取る用）
    private bool is_holding_smash;  //叩く力を貯めているか
    
    private GameObject camera_obj;  //カメラオブジェクト
    
    private Vector2 input_direction;        //インプット方向
    private SMASHSTATE smash_state;         //プレイヤーの叩く状態
    private float smash_power_num;          //叩く力の数値
    private SMASHJUMP can_jump_status;   //叩く力の段階

    private SoundManager soundmanager;

    private ParticleSystem part_line_effect;
    private ParticleSystem part_circle_effect;
    
    public bool in_grav_field {get; private set;}
    private GameObject grav_obj;
    
    private float target_rot;
    
    private LightBulbCollector check_is_cleared;

    /// <summary>
    /// 平田
    /// </summary>
    private DeformStage deform_stage;
    private WallSwitch wall_switch;
    private MinMaxDeform min_max_deform;
    private bool is_flip;
    
    public void TookDamage()
    {
        anim.SetTrigger("takeDamage");
        
        if (smash_state == SMASHSTATE.HOLDING)
        {
            smash_state = SMASHSTATE.NORMAL;
        }
    }
    
    public void GameOver()
    {
        is_dead = true;
        blackPanel.SetActive(true);
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
        
        part_line_effect = transform.GetChild(2).GetComponent<ParticleSystem>();
        part_circle_effect = transform.GetChild(3).GetComponent<ParticleSystem>();
        
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        camera_obj = GameObject.FindGameObjectWithTag("MainCamera");    //カメラオブジェクト取得
        
        //変数を初期化する
        is_grounded = false;
        is_flip = false;
        input_direction = Vector2.zero;
        input_check_pos = true;
        move_value = 0.0f;
        move_dir = Vector2.zero;
        speed = 0.0f;
        is_dead = false;
        
        blackPanel = GameObject.Find("BlackPanel");
        blackPanel.SetActive(false);
        
        target_rot = 0.0f;
        
        in_grav_field = false;

        //soundmannagerを取得
        soundmanager = GetComponent<SoundManager>();
        
        check_is_cleared = GameObject.FindObjectOfType<LightBulbCollector>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!check_is_cleared.IsCleared())
        {
            if (!is_dead)
            {
                if (!TakingDamage())
                {
                    //インプット方向を取得
                    input_direction = InputManager.instance.player_move_float;
                }
                
                CheckIsGrounded();
                
                speed = Mathf.MoveTowards(speed, input_direction.magnitude * max_speed, acceleration_speed);
                
                if (rb.velocity.magnitude > 0.0f && is_grounded && input_direction == Vector2.zero)
                {
                    rb.velocity = Vector3.MoveTowards(rb.velocity, Vector3.zero, deceleration_speed * Time.deltaTime * 4.0f);
                    speed = Mathf.MoveTowards(speed, 0.0f, deceleration_speed * 0.5f);
                }
            }
            else
            {
                RectTransform rTrans = blackPanel.GetComponent<RectTransform>();
                if (rTrans.localPosition != Vector3.zero)
                {
                    rTrans.localPosition = new Vector3(rTrans.localPosition.x + 10.0f, rTrans.localPosition.y, 0.0f);
                }
                else
                {
                    SceneManager.LoadScene("GameOverScene");
                }
            }
            
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
            
            anim.SetBool("isWalking", input_direction != Vector2.zero);

            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        }
    }
    
    void FixedUpdate() 
    {   
        if (!check_is_cleared.IsCleared())
        {
            if (!is_dead)
            {
                if (ground_obj != null)
                {
                    if (!ground_obj_parent.gameObject.GetComponent<StageRotation>().GetRotatingStatus())
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
                            
                            var emis = part_line_effect.emission;
                            emis.enabled = false;
                            emis = part_circle_effect.emission;
                            emis.enabled = false;
                            
                            break;
                            case SMASHSTATE.HOLDING:    //力を溜めてる状態
                            
                            var emisss = part_line_effect.emission;
                            emisss.enabled = true;
                            var emiss_circle = part_circle_effect.emission;
                            emiss_circle.enabled = true;
                            
                            var line_color = part_line_effect.main;
                            var circle_color = part_circle_effect.main;
                            
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
                                }
                                else
                                {
                                    line_color.startColor = new Color(0.0f, 1.0f, 0.0f);
                                    circle_color.startColor = new Color(0.0f, 1.0f, 0.0f);
                                }   
                            }
                            else
                            {
                                can_jump_status = SMASHJUMP.NONE;
                                line_color.startColor = new Color(0.0f, 0.0f, 1.0f);
                                circle_color.startColor = new Color(0.0f, 0.0f, 1.0f);
                            }
                            
                            emisss.rateOverTime = 90.0f * (smash_power_num / smash_threshold);
                            circle_color.startSize = 8.0f * (smash_power_num / smash_threshold);
                            
                            break;
                            case SMASHSTATE.SMASHING:   //力を放ってる状態
                            
                            var emissss = part_line_effect.emission;
                            emissss.enabled = false;
                            emissss = part_circle_effect.emission;
                            emissss.enabled = false;
                            
                            break;
                        }
                    }
                }
            }
        }
    }
    
    void Move()
    {
        if (input_direction.magnitude >= 0.01f)
        { 
            if (Mathf.Abs(input_direction.x) >= Mathf.Abs(input_direction.y))
            {
                input_direction.y = 0.0f;
            }
            else
            {
                input_direction.x = 0.0f;
            }
            
            Vector2 norm_input = input_direction.normalized;
            
            if (norm_input != move_dir)
            {
                if (input_check_pos)
                {
                    float temp_rot = target_rot;
                    
                    //左右移動
                    if (Mathf.Abs(norm_input.x) > 0.0f)
                    {
                        //右のほうにいる
                        if (Camera.main.WorldToScreenPoint(this.transform.position).x >= Screen.width / 2)
                        {
                            //下の方にいる
                            if (Camera.main.WorldToScreenPoint(this.transform.position).y <= Screen.height / 2)
                            {
                                if (norm_input.x > 0.0f)
                                {
                                    if (this.transform.up.y >= 0.0f)
                                    {
                                        temp_rot = 0.0f;
                                    }
                                    else
                                    {
                                        temp_rot = 180.0f;
                                    }
                                }
                                else
                                {
                                    if (this.transform.up.y >= 0.0f)
                                    {
                                        temp_rot = 180.0f;
                                    }
                                    else
                                    {
                                        temp_rot = 0.0f;
                                    }
                                }
                            }
                            else    //上の方にいる
                            {
                                if (norm_input.x > 0.0f)
                                {
                                    if (this.transform.up.y >= 0.0f)
                                    {
                                        temp_rot = 0.0f;
                                    }
                                    else
                                    {
                                        temp_rot = 180.0f;
                                    }
                                }
                                else
                                {
                                    if (this.transform.up.y >= 0.0f)
                                    {
                                        temp_rot = 180.0f;
                                    }
                                    else
                                    {
                                        temp_rot = 0.0f;
                                    }
                                }
                            }
                        }
                        else    //左の方にいる
                        {
                            //下の方にいる
                            if (Camera.main.WorldToScreenPoint(this.transform.position).y <= Screen.height / 2)
                            {
                                if (norm_input.x > 0.0f)
                                {
                                    if (this.transform.up.y >= 0.0f)
                                    {
                                        temp_rot = 180.0f;
                                    }
                                    else
                                    {
                                        temp_rot = 0.0f;
                                    }
                                }
                                else
                                {
                                    if (this.transform.up.y >= 0.0f)
                                    {
                                        temp_rot = 0.0f;
                                    }
                                    else
                                    {
                                        temp_rot = 180.0f;
                                    }
                                }
                            }
                            else
                            {
                                if (norm_input.x > 0.0f)
                                {
                                    if (this.transform.up.y >= 0.0f)
                                    {
                                        temp_rot = 0.0f;
                                    }
                                    else
                                    {
                                        temp_rot = 180.0f;
                                    }
                                }
                                else
                                {
                                    if (this.transform.up.y >= 0.0f)
                                    {
                                        temp_rot = 180.0f;
                                    }
                                    else
                                    {
                                        temp_rot = 0.0f;
                                    }
                                }
                            }
                        }
                        
                        move_value = Mathf.Abs(input_direction.x);
                    }
                    else    //上下移動
                    {
                        //右のほうにいる
                        if (Camera.main.WorldToScreenPoint(this.transform.position).x >= Screen.width / 2)
                        {
                            //下の方にいる
                            if (Camera.main.WorldToScreenPoint(this.transform.position).y <= Screen.height / 2)
                            {
                                if (norm_input.y > 0.0f)
                                {
                                    if (this.transform.up.y >= 0.0f)
                                    {
                                        temp_rot = 0.0f;
                                    }
                                    else
                                    {
                                        temp_rot = 180.0f;
                                    }
                                }
                                else
                                {
                                    if (this.transform.up.y >= 0.0f)
                                    {
                                        temp_rot = 180.0f;
                                    }
                                    else
                                    {
                                        temp_rot = 0.0f;
                                    }
                                }
                            }
                            else
                            {
                                if (norm_input.y > 0.0f)
                                {
                                    if (this.transform.up.y >= 0.0f)
                                    {
                                        temp_rot = 180.0f;
                                    }
                                    else
                                    {
                                        temp_rot = 0.0f;
                                    }
                                }
                                else
                                {
                                    if (this.transform.up.y >= 0.0f)
                                    {
                                        temp_rot = 0.0f;
                                    }
                                    else
                                    {
                                        temp_rot = 180.0f;
                                    }
                                }
                            }
                        }
                        else    //左のほうにいる
                        {
                            //下の方にいる
                            if (Camera.main.WorldToScreenPoint(this.transform.position).y <= Screen.height / 2)
                            {
                                if (norm_input.y > 0.0f)
                                {
                                    if (this.transform.up.y >= 0.0f)
                                    {
                                        temp_rot = 180.0f;
                                    }
                                    else
                                    {
                                        temp_rot = 0.0f;
                                    }
                                }
                                else
                                {
                                    if (this.transform.up.y >= 0.0f)
                                    {
                                        temp_rot = 0.0f;
                                    }
                                    else
                                    {
                                        temp_rot = 180.0f;
                                    }
                                }
                            }
                            else
                            {
                                if (norm_input.y > 0.0f)
                                {
                                    if (this.transform.up.y >= 0.0f)
                                    {
                                        temp_rot = 0.0f;
                                    }
                                    else
                                    {
                                        temp_rot = 180.0f;
                                    }
                                }
                                else
                                {
                                    if (this.transform.up.y >= 0.0f)
                                    {
                                        temp_rot = 180.0f;
                                    }
                                    else
                                    {
                                        temp_rot = 0.0f;
                                    }
                                }
                            }
                        }
                        
                        move_value = Mathf.Abs(input_direction.y);
                    }
                    
                    if (temp_rot != target_rot)
                    {
                        transform.Rotate(new Vector3(0.0f, 180.0f, 0.0f), Space.Self);
                        target_rot = temp_rot;
                    }
                    
                    input_check_pos = false;
                }
                
                speed /= 2.0f;
                move_dir = norm_input;
            }
            
            var locVel = transform.InverseTransformDirection(rb.velocity);
            locVel.x = move_value * (speed / 20.0f);
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
            ground_obj_parent = ground_obj.transform.root.gameObject;
            deform_stage = ground_obj_parent.GetComponent<DeformStage>();
            min_max_deform = ground_obj_parent.GetComponent<MinMaxDeform>();
        }
        else
        {
            is_grounded = false;
        }
    }
    
    private void HoldSmash()
    {   
        //地面についていたら、力を溜める可能にする
        if (is_grounded && !is_dead && !TakingDamage())
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
            
            if (smash_power_num >= smash_max_time)
            {
                anim.SetTrigger("isSmashStrong");
            }
            else
            {
                if (smash_power_num < smash_power_scalar)
                {
                    anim.speed = 1.5f;
                }
                anim.SetTrigger("isSmash");
            }
        }
    }
    
    public void SmashFunc()
    {
        if (can_jump_status == SMASHJUMP.CAN_JUMP)
        {
            var locVel = transform.InverseTransformDirection(rb.velocity);
            locVel.y = jump_power * smash_power_num / smash_threshold * 0.5f;
            rb.velocity = transform.TransformDirection(locVel);
        }
        
        //叩くSEの再生
        soundmanager.PlaySoundEffect("Strike");

        if (deform_stage)
        {
            bool isSmash = true;
            if (is_flip)
            {
                if (!min_max_deform.GetMaxHit())
                    isSmash = false;
            }
            else
            {
                if (min_max_deform.GetMinHit())
                    isSmash = false;
            }

            if (isSmash)
            {
                deform_stage.AddDeformpointDown(transform.position, transform.eulerAngles.y, smash_power_num + 1 * smash_power_scalar, is_flip);
            }
            
            camera_obj.GetComponent<CameraMove>().ShakeCamera(smash_power_num / 2.0f, 0.2f);
        }
        else
        {
            if (wall_switch != null) 
            {
                wall_switch.WallMove();
            }
        }
        
        if (smash_power_num < smash_power_scalar)
        {
            anim.speed = 1.0f;
        }
        
        float hit_stop_delay = smash_power_num / 10.0f;
        HitstopManager.instance.StartHitStop(hit_stop_delay);
        Invoke("SpawnSparks", hit_stop_delay);
        
        smash_state = SMASHSTATE.NORMAL;
        anim.ResetTrigger("holdSmash");
        
        InputManager.instance.VibrateController(0.2f, smash_power_num / smash_max_time * 1.5f);
    }
    
    private void SpawnSparks()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.up * -1.0f, out hit, 5.0f, LayerMask.GetMask("Ground")))
        {
            Instantiate(spark_effect, hit.point, Quaternion.identity);
        }
    }
    
    private void FlipCharacter()
    {
        FlipUpsideDown(false);
    }
    
    private void FlipUpsideDown(bool rotate_other_side)
    {
        RaycastHit hit_info;
        if (Physics.Raycast(this.transform.position + this.transform.up * 0.25f, this.transform.up * -1.0f, out hit_info, 5.0f, LayerMask.GetMask("Ground")))
        {
            float dis = Vector3.Distance(this.transform.position, hit_info.point);
            Vector3 new_pos;
            
            while (true)
            {
                Vector3 check_pos = this.transform.position + -this.transform.up * dis;
                Collider[] hit_col = Physics.OverlapSphere(check_pos, 2.0f, LayerMask.GetMask("Ground"));
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
            
            if (target_rot == 180.0f)
            {
                target_rot = 0.0f;
            }
            else
            {
                target_rot = 180.0f;
            }
            
            this.transform.position = new_pos;
            if (is_flip) is_flip = false; else is_flip = true;
        }
    }
    
    private bool TakingDamage()
    {
        bool result = false;
        
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("tookDamage") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            result = true;
        }
        
        return result;
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
            if (other.gameObject.name == "WallSwich")
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
        }
    }
    
    private void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.tag == "FlipGate")
        {
            FlipUpsideDown(true);
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