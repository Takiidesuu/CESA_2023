using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageAddForce : MonoBehaviour
{
    [SerializeField] private float VibrationPower = 1f;
    [SerializeField] private float VibrationTick = 0.1f;
    [SerializeField] private bool isVibration = false;

    private Vector3 defaultPos;
    private Vector3 vibrationDir;
    private float vibrationTime;

    private Vector3 velocity = Vector3.zero;
    private float smoothTime = 0.3f;
    // Start is called before the first frame update
    void Start()
    {
        defaultPos = transform.position;
        vibrationDir = Random.insideUnitSphere.normalized;
        vibrationTime = 0f;
    }

    void Update()
    {
        if (isVibration)
        {
            Vibrate();
        }
    }

    // Update is called once per frame
    void Vibrate()
    {
        if (vibrationTime < VibrationTick)
        {
            transform.position += vibrationDir * VibrationPower * Time.deltaTime;
            vibrationTime += Time.deltaTime;
        }
        else
        {
            Vector3 targetPos = defaultPos;
            Vector3 currentPos = transform.position;
            transform.position = Vector3.SmoothDamp(currentPos, targetPos, ref velocity, smoothTime);
            isVibration = false;
            vibrationTime = 0f;
        }
    }
}
