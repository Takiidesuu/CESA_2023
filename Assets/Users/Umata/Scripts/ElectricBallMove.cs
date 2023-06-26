using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ElectricBallMove : MonoBehaviour
{
    public bool BackBuilding = false;

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
    
    [Tooltip("スピードエフェクトプレハブ")]
    [SerializeField] private GameObject speed_effect;

    private Rigidbody rb;                   //リギッドボディー
    private float m_destroy_timer;
    private GameObject player;
    public GameObject ParentGenerator;
    
    private float m_real_speed;
    
    //帯電エフェクト
    private GameObject m_electric_effect;
    private Vector3 m_electric_startsize;
    public GameObject m_destroy_effect;

    private bool has_jumped;
    private float elapsed_time;
    
    private bool is_on_boost;
    
    LightBulbCollector check_is_cleared;
    
    private SoundManager soundManager;
    
    private bool is_dead;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();                 //リギッドボディー取得
        player = GameObject.Find("Player");
        
        m_real_speed = m_speed;
        has_jumped = false;
        
        check_is_cleared = GameObject.FindObjectOfType<LightBulbCollector>();
        m_electric_effect = transform.Find("ElectricEffect").gameObject;
        m_electric_startsize = m_electric_effect.transform.localScale;
        elapsed_time = time_to_normal_speed;
        is_on_boost = false;
        
        speed_effect.SetActive(false);
        soundManager = GetComponent<SoundManager>();
        
        is_dead = false;
    }

    // Update is called once per frame
    void Update()
    {   
        if ((!check_is_cleared.IsCleared() && !GameOverManager.instance.game_over_state && player.GetComponent<PlayerMove>().start_game && !is_dead) || BackBuilding)
        {
            var locVel = transform.InverseTransformDirection(rb.velocity);
            locVel.x = m_real_speed;
            rb.velocity = transform.TransformDirection(locVel);

            m_destroy_timer += Time.deltaTime;
            //エフェクトの大きさを経過時間に応じて小さくする
            Vector3 effect_scale = m_electric_startsize - (m_electric_startsize * (m_destroy_timer / m_destroy_time));

            m_electric_effect.transform.localScale = effect_scale;

            if (!is_dead)
            {
                //時間経過後削除
                if(m_destroy_timer > m_destroy_time)
                {
                    Instantiate(m_destroy_effect,transform.position,Quaternion.identity,transform.parent);
                    StartCoroutine(DestroySequence());
                    is_dead = true;
                }
            }
            
            Vector3 playerpos;
            playerpos.x = transform.position.x;
            playerpos.y = transform.position.y;
            if (!BackBuilding)
                playerpos.z = 0;
            else
                playerpos.z = transform.position.z;


            Vector3 playerRot;
            playerRot.x = 0;
            playerRot.y = 0;
            playerRot.z = transform.rotation.z;

            //Z軸を強制的にPlayer座標に設定
            transform.position = playerpos;

            if (soundManager != null)
            {
                if (!soundManager.CheckIsPlaying("Move"))
                {
                    soundManager.PlaySoundEffect("Move");
                }
            }
        }
        else
        {
            if (GameObject.FindObjectOfType<LightBulbClearTrigger>() == null)
            {
                rb.velocity = Vector3.MoveTowards(rb.velocity, Vector3.zero, Time.deltaTime * 30.0f);
            }
            soundManager.StopSoundEffect("Move");
        }
    }
    
    private void FixedUpdate() 
    {
        bool is_speeding = false;
        
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
                is_speeding = true;
                elapsed_time += Time.deltaTime;
            }
        }
        else
        {
            is_speeding = true;
            elapsed_time = 0.0f;
        }
        
        speed_effect.SetActive(is_speeding);
    }
    
    private void LateUpdate() 
    {   
        RaycastHit hit;
        if (Physics.Raycast(this.transform.position, this.transform.up * -1, out hit, 2.0f, LayerMask.GetMask("Ground")))
        {
            if (CheckInCircle(hit))
            {
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, 180.0f, transform.eulerAngles.z);
            }
            else
            {
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0.0f, transform.eulerAngles.z);
            }
        }
    }
    
    private bool CheckInCircle(RaycastHit hit)
    {
        Vector3 center_vec = hit.point - hit.transform.root.gameObject.transform.position;
        float dot_to_center = Vector3.Dot(center_vec.normalized, transform.up);
        
        if (dot_to_center < 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
    public float GetSpeed()
    {
        return m_real_speed;
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
            collision.gameObject.GetComponent<FlipGate>().Flip(this.gameObject);
        }
        
        if (collision.gameObject.tag == "SpeedBooster")
        {
            is_on_boost = true;
        }
    }

    public void FlipUpsideDown()
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

    public void ChangeRealSpeed(float speed)
    {
        m_speed = speed;
    }
    
    IEnumerator DestroySequence()
    {
        yield return new WaitForSeconds(Time.deltaTime * 2);
        GetComponent<SphereCollider>().enabled = false;
        
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        
        soundManager.PlaySoundEffect("Die");
        
        yield return new WaitForSeconds(2);
        
        Destroy(this.gameObject);
    }
}