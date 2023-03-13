using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageRotation : MonoBehaviour
{
    public bool is_rotation;

    public float rotationSpeed = 30f; // ‰ñ“]‘¬“x
    private float rotationAngle = 180f; // ‰ñ“]Šp“x

    private float currentAngle = 0f; // Œ»İ‚Ì‰ñ“]Šp“x

    void Update()
    {
        if (is_rotation)
        {
            if (currentAngle < rotationAngle)
            {
                float rotateAmount = Mathf.Min(rotationSpeed * Time.deltaTime, rotationAngle - currentAngle);
                transform.Rotate(Vector3.forward * rotateAmount);
                currentAngle += rotateAmount;
            }
        }
    }
}
