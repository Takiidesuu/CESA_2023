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

    [Tooltip("坂での加速時間")]
    [SerializeField] private float m_accelerator_time = 1.0f;

    [Tooltip("坂での加速量")]
    [SerializeField] private float m_accelerator_speed = 2.0f;

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
        playerpos.z = 0;
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
        if (collision.gameObject.tag == "SpeedBooster")
        {
            BoostSpeed(m_accelerator_time,m_accelerator_speed);
        }

    }
    public void BoostSpeed(float boostTime, float boostSpeed)
    {
        StartCoroutine(BoostSpeedCoroutine(boostTime, boostSpeed));
    }

    private IEnumerator BoostSpeedCoroutine(float boostTime, float boostSpeed)
    {
        float originalSpeed = m_speed; // 元の速度を保存する

        m_speed += boostSpeed; // 速度を増加させる

        yield return new WaitForSeconds(boostTime); // 指定時間待つ

        // 元の速度に戻るまでの時間
        float decelerationTime = 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < decelerationTime)
        {
            float t = elapsedTime / decelerationTime;
            m_speed = Mathf.Lerp(m_speed, originalSpeed, t); // 現在の速度から元の速度に徐々に戻す
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        m_speed = originalSpeed; // 元の速度に戻す
    }
}
