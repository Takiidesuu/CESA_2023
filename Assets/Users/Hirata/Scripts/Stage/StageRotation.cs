using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class StageRotation : MonoBehaviour
{
    private bool is_reverse_rotation;
    private bool wait_rotation;
    private bool is_rotation;

    private float wait_rotation_time = 0;

    private float reversSpeed = 500f;
    private float rotationSpeed = 250f; // ��]���x

    private float reverseAngle = 10f;
    private float rotationAngle = 190f; // ��]�p�x

    private float LastAngle;

    private float easeCount;
    private float rotationCount = 1000;

    GameObject player_obj;
    
    public bool GetRotatingStatus()
    {
        if (is_reverse_rotation || is_rotation)
            return true;
        else
            return false;
    }
    
    public void StartRotate()
    {
        if (!is_rotation && !is_reverse_rotation)
        {
            is_reverse_rotation = true;
            player_obj.transform.parent = this.transform;
            LastAngle = transform.rotation.eulerAngles.z;
        }
    }
    
    private void Start() 
    {
        player_obj = GameObject.FindGameObjectWithTag("Player");
        is_reverse_rotation = false;
        is_rotation = false;
    }

    void Update()
    {
        //時計回りに回す
        if (is_reverse_rotation)
        {
            if (rotationCount >= easeCount)
            {
                float progress = easeCount / rotationCount;
                float rotateAmount = LastAngle - Ease(progress) * reverseAngle;
                transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, rotateAmount);

                easeCount += Time.deltaTime * reversSpeed;
                if (easeCount > rotationCount)
                {
                    easeCount = rotationCount;
                    transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, LastAngle - reverseAngle);
                    is_reverse_rotation = false;
                    wait_rotation = true;
                    LastAngle = transform.rotation.eulerAngles.z;
                    easeCount = 0;
                }
            }
        }

        //少し待つ
        if (wait_rotation)
        {
            StartCoroutine(Wait());
            wait_rotation = false;
        }

        //逆時計回りに回す
        if (is_rotation)
        {
            if (rotationCount >= easeCount)
            {
                float progress = easeCount / rotationCount;
                float rotateAmount = LastAngle + Ease(progress) * rotationAngle;
                transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, rotateAmount);

                easeCount += Time.deltaTime * rotationSpeed;
                if (easeCount > rotationCount)
                {
                    easeCount = rotationCount;
                    transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, LastAngle + 190);
                    is_rotation = false;
                    easeCount = 0;
                    player_obj.transform.parent = null;
                }
            }
        }


    }
    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(wait_rotation_time);
        is_rotation = true;
    }

    float Ease(float x)
    {
        float c4 = (2 * Mathf.PI) / 3;

        return x == 0
          ? 0
          : x == 1
          ? 1
          : Mathf.Pow(2, -10 * x) * Mathf.Sin((float)((x * 10 - 0.75) * c4)) + 1;
    }
}
