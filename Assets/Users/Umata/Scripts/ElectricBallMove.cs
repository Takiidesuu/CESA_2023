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
    }

}
