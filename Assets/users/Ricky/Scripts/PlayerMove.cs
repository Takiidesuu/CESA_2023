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
    [Tooltip("加速度")]
    [SerializeField] private float acceleration_speed = 2.0f;
    [Tooltip("最高移動速度")]
    [SerializeField] private float max_speed = 500.0f;
    [Tooltip("減速速度")]
    [SerializeField] private float deceleration_speed = 5.0f;
    
    [Header("Smash Param")]
    [Tooltip("ジャンプ力")]
    [SerializeField] private float jump_power = 4.0f;
    [Tooltip("溜め段階変わり時間")]
    [SerializeField] private float smash_threshold = 1.0f;
    
    [SerializeField] private GameObject blackPanel;
    
    //コンポネント
    private Rigidbody rb;                   //リギッドボディー
    private CapsuleCollider col;            //コライダー
    private Animator anim;                  //アニメーター

    private bool input_check_pos;
    private float move_value;
    private float input_modifier;
    private Vector2 move_dir;
    private float speed;
    public bool is_dead;
    
    private bool is_grounded;       //地面についているか
    private GameObject ground_obj;  //ついてる地面
    private GameObject ground_obj_parent;   //ついてる地面の親（コンポネント取る用）
    private bool is_holding_smash;  //叩く力を貯めているか
    
    private GameObject camera_obj;  //カメラオブジェクト
    
    private Vector2 input_direction;        //インプット方向
    private SMASHSTATE smash_state;         //プレイヤーの叩く状態
    private float smash_power_num;          //叩く力の数値
    private SMASHLEVEL smash_power_level;   //叩く力の段階
    
    public ParticleSystem partSystem;
    
    private float target_rot;

    /// <summary>
    /// 平田
    /// </summary>
    private DeformStage deform_stage;
    private bool is_flip;
    
    public void GameOver()
    {
        is_dead = true;
    }
    
    public bool GetSmashingState()
    {
        return smash_state != SMASHSTATE.NORMAL ? true : false;
    }
    
    public GameObject GetGroundObj()
    {
        return ground_obj;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();                 //リギッドボディー取得
        col = GetComponent<CapsuleCollider>();          //コライダー取得
        anim = transform.GetChild(0).GetComponent<Animator>();                //アニメーター取得
        
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
        
        blackPanel = blackPanel.transform.GetChild(0).gameObject;
        
        target_rot = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!is_dead)
        {
            //インプット方向を取得
            input_direction = InputManager.instance.player_move_float;
            
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
        
        ReleaseSmash();
        
        if (InputManager.instance.press_flip)
        {
            FlipCharacter();
        }
        
        if (InputManager.instance.press_rotate)
        {
            RotateGround();
        }
        
        anim.SetBool("isWalking", input_direction != Vector2.zero);
    }
    
    void FixedUpdate() 
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
                        
                        var emis = partSystem.emission;
                        emis.enabled = false;
                        
                        break;
                        case SMASHSTATE.HOLDING:    //力を溜めてる状態
                        
                        var emisss = partSystem.emission;
                        emisss.enabled = true;
                        
                        var mainColor = partSystem.main;
                        
                        //溜めた力を加算する
                        if (smash_power_num >= smash_threshold * 2.0f)
                        {
                            smash_power_num = smash_threshold * 2.0f;
                        }
                        else
                        {
                            smash_power_num += Time.deltaTime;
                        }
                        
                        //溜めた力によって、力の段階を変える
                        if (smash_power_num >= smash_threshold * 2.0f)
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
                        
                        var emiss = partSystem.emission;
                        emiss.enabled = false;
                        
                        break;
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
        }
        else
        {
            is_grounded = false;
        }
    }
    
    private void HoldSmash()
    {   
        //地面についていたら、力を溜める可能にする
        if (is_grounded && !is_dead)
        {
            if (!ground_obj_parent.GetComponent<StageRotation>().GetRotatingStatus())
            {
                smash_state = SMASHSTATE.HOLDING;
            }
        }
    }
    
    private void ReleaseSmash()
    {
        if (smash_state == SMASHSTATE.HOLDING)
        {
            smash_state = SMASHSTATE.SMASHING;
            anim.SetTrigger("isSmash");
        }
    }
    
    public void SmashFunc()
    {
        switch (smash_power_level)
        {
            case SMASHLEVEL.NONE:
                StartCoroutine(SmashGround(0.0f, 1, 1.5f, 0.0f));
            break;
            case SMASHLEVEL.SMALL:
                //へこむ処理
                StartCoroutine(SmashGround(0.0f, 2, 2.5f, 0.0f));
                rb.AddForce(this.transform.up * jump_power, ForceMode.Impulse);
                break;
            case SMASHLEVEL.BIG:

                RaycastHit hit;
                if (Physics.Raycast(this.transform.position, this.transform.up, out hit, 80.0f, LayerMask.GetMask("Ground")))
                {
                    //へこむ処理（位置はhitを使う）
                    //へこむ処理（位置はhitを使う）  でフォームを適用させるステージがなってない
                    //飛んだフラグbool追加 ステージに着地した際にAddDeformPointDownを3回分するので何とかなるかと
                    StartCoroutine(SmashGround(0.0f, 3, 5.0f, 180.0f));
                    rb.AddForce(this.transform.up * 80.0f, ForceMode.Impulse);
                }
                
                StartCoroutine(SmashGround(0.4f, 3, 5.0f, 0.0f));
                break;
        }
    }
    
    IEnumerator SmashGround(float fdelay, int ipower, float fcam_power, float fangle)
    {
        yield return new WaitForSeconds(fdelay);
        
        if (deform_stage)
        {
            for (int i = 0; i < ipower; i++)
                deform_stage.AddDeformpointDown(transform.position, transform.eulerAngles.y + fangle, transform.eulerAngles.z, is_flip);
        }
        
        camera_obj.GetComponent<CameraMove>().ShakeCamera(fcam_power, 0.2f);
        smash_state = SMASHSTATE.NORMAL;
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
    }
}