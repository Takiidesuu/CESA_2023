using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ElectricBallMove : MonoBehaviour
{

    [Tooltip("移動速度")]
    [SerializeField] private float m_speed = 5.0f;

    [Tooltip("減速速度")]
    [SerializeField] private float deceleration_speed = 5.0f;

    [Tooltip("消滅時間")]
    [SerializeField] private float m_destroy_time = 5.0f;

    [Tooltip("回転の滑らかさ")]
    [SerializeField] private float turn_smooth_time = 1.0f;

    private Rigidbody rb;                   //リギッドボディー
    private float m_destroy_timer;
    private GameObject player;
    public GameObject ParentGenerator;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();                 //リギッドボディー取得
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 move_vec;
        move_vec.x = m_speed;
        move_vec.y = 0;
        move_vec.z = 0;

        transform.position += transform.rotation * move_vec;
        m_destroy_timer += Time.deltaTime;

        //時間経過後削除
        if(m_destroy_timer > m_destroy_time)
        {
            Destroy(this.gameObject);
        }
        transform.position.Set(transform.position.x, transform.position.y,0);

        Vector3 playerpos;
        playerpos.x = transform.position.x;
        playerpos.y = transform.position.y;
        playerpos.z = player.transform.position.z;
        //Z軸を強制的にPlayer座標に設定
        transform.position = playerpos;
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
    }

}
