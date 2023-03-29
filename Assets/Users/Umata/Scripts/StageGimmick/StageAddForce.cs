using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageAddForce : MonoBehaviour
{

    [SerializeField] private float VibrationPower = 1f; // 振動の力を表す調整可能なパラメータ
    [SerializeField] private float VibrationTick = 0.1f; // 元の位置に戻るまでの秒数を表す調整可能なパラメータ
    [SerializeField] private bool isVibration = false; // 振動を開始するかを判定するbool型の変数

    private Rigidbody rb;
    private Vector3 defaultPos;
    private Vector3 vibrationDir;
    private float vibrationTime;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        defaultPos = transform.position;
        vibrationDir = Random.insideUnitSphere.normalized;
        vibrationTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (isVibration)
        {
            Vibrate();
        }
    }
    void Vibrate()
    {
        if (vibrationTime < VibrationTick)
        {
            rb.AddForce(vibrationDir * VibrationPower, ForceMode.Acceleration);
            vibrationTime += Time.deltaTime;
        }
        else
        {
            rb.MovePosition(defaultPos);
            isVibration = false;
            vibrationTime = 0f;
        }
    }
}
