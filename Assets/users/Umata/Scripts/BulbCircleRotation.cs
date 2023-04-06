using UnityEngine;

public class BulbCircleRotation : MonoBehaviour
{
    public bool OnPower = false;
    public float rotationSpeed = 10.0f;
    public float easingFactor = 0.1f;
    public GameObject parentObj;

    private Quaternion targetRotation;

    void Start()
    {
        targetRotation = transform.rotation;
    }

    void Update()
    {
        if (OnPower)
        {
            float time = Time.time;
            float xAngle = Mathf.Sin(time * rotationSpeed) * 90.0f;
            float yAngle = Mathf.Cos(time * rotationSpeed) * 90.0f;

            Quaternion target = Quaternion.Euler(xAngle, yAngle, 0.0f);
            targetRotation = Quaternion.Lerp(transform.rotation, target, easingFactor);

            transform.rotation = targetRotation;
        }
    }
}
