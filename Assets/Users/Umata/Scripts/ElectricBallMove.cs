using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ElectricBallMove : MonoBehaviour
{
    [Tooltip("移動速度")]
    [SerializeField] private float m_speed = 5.0f;

    [Tooltip("消滅時間")]
    [SerializeField] private float m_destroy_time = 5.0f;
    
    [Tooltip("本来のスピードに戻るまでの時間")]
    [SerializeField] private float time_to_normal_speed = 1.0f;
    [Tooltip("本来のスピードに戻るまでの速度")]
    [SerializeField] private float return_speed = 1.0f;
    
    [Tooltip("スピード限界")]
    [SerializeField] private Vector2 speed_limit = new Vector2(2.0f, 50.0f);

    private Rigidbody rb;                   //リギッドボディー
    private float m_destroy_timer;
    private GameObject player;
    public GameObject ParentGenerator;
    
    private float m_real_speed;
    
    private bool has_jumped;
    private float elapsed_time;
    
    private bool is_on_boost;
    
    LightBulbCollector check_is_cleared;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();                 //リギッドボディー取得
        player = GameObject.Find("Player");
        
        m_real_speed = m_speed;
        has_jumped = false;
        
        check_is_cleared = GameObject.FindObjectOfType<LightBulbCollector>();

        elapsed_time = 0.0f;
        is_on_boost = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!check_is_cleared.IsCleared())
        {
            var locVel = transform.InverseTransformDirection(rb.velocity);
            locVel.x = m_real_speed;
            rb.velocity = transform.TransformDirection(locVel);

            m_destroy_timer += Time.deltaTime;

            //時間経過後削除
            if(m_destroy_timer > m_destroy_time)
            {
                Destroy(this.gameObject);
            }
            
            Vector3 playerpos;
            playerpos.x = transform.position.x;
            playerpos.y = transform.position.y;
            playerpos.z = 0;
            
            //Z軸を強制的にPlayer座標に設定
            transform.position = playerpos;
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
    }
    
    private void FixedUpdate() 
    {
        if (!is_on_boost)
        {
            if (elapsed_time >= time_to_normal_speed)
            {
                if (m_real_speed != m_speed)
                {
                    m_real_speed = Mathf.MoveTowards(m_real_speed, m_speed, return_speed);
                }
                else
                {
                    m_real_speed = m_speed;
                }
            }
            else
            {
                elapsed_time += Time.deltaTime;
            }
        }
        else
        {
            elapsed_time = 0.0f;
        }
    }
    
    private void LateUpdate() 
    {
        //m_real_speed = Mathf.Clamp(m_real_speed, speed_limit.x, speed_limit.y);
    }
    
    public void ChangeSpeed(float boostSpeed)
    {
        m_real_speed = m_speed + boostSpeed;
    }
    
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "ElectricitySource")
        {
            if (m_destroy_timer > 1.0f)
            {
                Destroy(this.gameObject);
            }
        }
        
        if (collision.gameObject.tag == "FlipGate")
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
                
                this.transform.position = new_pos;
            }
        }
        
        if (collision.gameObject.tag == "SpeedBooster")
        {
            is_on_boost = true;
        }
    }
    
    private void OnTriggerStay(Collider other) 
    {
        if (other.gameObject.tag == "SpeedBooster")
        {
            is_on_boost = true;
        }
    }
    
    private void OnTriggerExit(Collider other) 
    {
        if (other.gameObject.tag == "SpeedBooster")
        {
            is_on_boost = false;
        }
    }
}