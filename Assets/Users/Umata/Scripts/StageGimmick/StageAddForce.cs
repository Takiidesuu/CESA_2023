using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageAddForce : MonoBehaviour
{
    public float VibrationPower = 1.0f;
    public float VibrationTick = 0.5f;
    public bool isVibration = false;

    private Vector3 originalPosition;
    private float timer = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.position;
    }

    void Update()
    {
        if (isVibration)
        {
            timer += Time.deltaTime;

            float x = Mathf.Sin(timer * Mathf.PI / VibrationTick) * VibrationPower;
            float y = Mathf.Cos(timer * Mathf.PI / VibrationTick) * VibrationPower;

            transform.position = originalPosition + new Vector3(x, y, 0);

            if (timer >= VibrationTick)
            {
                isVibration = false;
                timer = 0.0f;
                transform.position = originalPosition;
            }
        }
    }
}
