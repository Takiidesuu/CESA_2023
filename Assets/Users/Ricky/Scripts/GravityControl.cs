using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityControl : MonoBehaviour
{
    [Tooltip("回転速度")]
    [SerializeField] private float rotation_speed = 20.0f;
    
    [HideInInspector] public GravityOrbit gravity;
    
    private Rigidbody rb;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void FixedUpdate() 
    {
        //ターゲットとなる惑星があった場合
        if (gravity)
        {
            Vector3 gravity_up = Vector3.zero;
            
            if (gravity.fixed_direction)
            {
                gravity_up = gravity.transform.up;
            }
            else
            {
                gravity_up = (transform.position - gravity.transform.position).normalized;
            }
            
            Vector3 local_up = transform.up;
            
            Quaternion target_rotation = Quaternion.FromToRotation(local_up, gravity_up) * transform.rotation;
            
            transform.up = Vector3.Lerp(transform.up, gravity_up, rotation_speed * Time.deltaTime);
            
            rb.AddForce((-gravity_up * gravity.gravity) * rb.mass);
        }
    }
}
