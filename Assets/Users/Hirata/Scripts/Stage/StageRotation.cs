using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class StageRotation : MonoBehaviour
{
    private bool is_rotation;

    public float rotationSpeed = 30f; // ��]���x
    private float rotationAngle = 180f; // ��]�p�x

    private float currentAngle = 0f; // ���݂̉�]�p�x
    private float targetAngle;
    
    GameObject player_obj;
    
    public bool GetRotatingStatus()
    {
        return is_rotation;
    }
    
    public void StartRotate()
    {
        if (!is_rotation)
        {
            is_rotation = true;
            targetAngle = currentAngle + rotationAngle;
            player_obj.transform.parent = this.transform;
        }
    }
    
    private void Start() 
    {
        player_obj = GameObject.FindGameObjectWithTag("Player");
        is_rotation = false;
    }

    void Update()
    {
        if (is_rotation)
        {
            if (currentAngle < targetAngle)
            {
                float rotateAmount = Mathf.Min(rotationSpeed * Time.deltaTime, targetAngle - currentAngle);
                transform.Rotate(Vector3.forward * rotateAmount);
                currentAngle += rotateAmount;
            }
            else
            {
                is_rotation = false;
                player_obj.transform.parent = null;
                
                if (targetAngle >= 360.0f)
                {
                    targetAngle = 0.0f;
                }
            }
        }
    }
}
