using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityControl : MonoBehaviour
{
    [Tooltip("重力")]
    [SerializeField] private float gravity_power = 5.0f;
    [Tooltip("回転速度")]
    [SerializeField] private float rotation_speed = 20.0f;

    private Rigidbody rb;

    private Vector3 gravity_dir;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Vector3 set_ground_dir = CheckFloorAngle();
        gravity_dir = set_ground_dir;

        // Finds desired rotation relative to surface normal
        var targetRotation = Quaternion.FromToRotation(transform.up, gravity_dir) * transform.rotation;

        // Apply rotation and gravity
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotation_speed * Time.deltaTime);

        rb.AddForce(gravity_dir.normalized * -9.81f * gravity_power);
    }

    private Vector3 CheckFloorAngle()
    {
        LayerMask ground_layer_mask = LayerMask.GetMask("Ground");

        RaycastHit hit_front;
        RaycastHit hit_centre;
        RaycastHit hit_back;

        Vector3 feet_pos = this.transform.position - this.transform.up * 0.2f;
        Vector3 dir_offset_front = this.transform.up * -1.0f + this.transform.right * 0.2f;
        dir_offset_front.Normalize();
        Vector3 dir_offset_back = this.transform.up * -1.0f - this.transform.right * 0.2f;
        dir_offset_back.Normalize();

        bool found_ground = false;

        found_ground = Physics.Raycast(feet_pos + this.transform.right * 0.2f, dir_offset_front, out hit_front, 10.0f, ground_layer_mask);

        if (!found_ground)
        {
            found_ground = Physics.Raycast(feet_pos, this.transform.up * -1.0f, out hit_centre, 10.0f, ground_layer_mask);
        }
        else
        {
            Physics.Raycast(feet_pos, this.transform.up * -1.0f, out hit_centre, 10.0f, ground_layer_mask);
        }

        if (!found_ground)
        {
            found_ground = Physics.Raycast(feet_pos - this.transform.right * 0.2f, dir_offset_back, out hit_back, 10.0f, ground_layer_mask);
        }
        else
        {
            Physics.Raycast(feet_pos - this.transform.right * 0.2f, dir_offset_back, out hit_back, 10.0f, ground_layer_mask);
        }

        Debug.DrawRay(feet_pos + this.transform.right * 0.2f, dir_offset_front, Color.red);
        Debug.DrawRay(feet_pos, this.transform.up * -1.0f, Color.red);
        Debug.DrawRay(feet_pos - this.transform.right * 0.2f, dir_offset_back, Color.red);

        Vector3 hit_dir = transform.up;

        if (found_ground)
        {
            if (hit_front.transform != null)
            {
                hit_dir += hit_front.normal;
            }
            if (hit_centre.transform != null)
            {
                hit_dir += hit_centre.normal;
            }
            if (hit_back.transform != null)
            {
                hit_dir += hit_back.normal;
            }
        }
        else
        {
            float sphere_size = 40.0f;
            Collider[] col_info = Physics.OverlapSphere(this.transform.position, sphere_size, ground_layer_mask);
            float distance_check = sphere_size;
            foreach (var current in col_info)
            {
                RaycastHit ground_hit;
                if (Physics.Raycast(this.transform.position, current.transform.position, out ground_hit, 40.0f, ground_layer_mask))
                {
                    float distance_to_ground = Vector3.Distance(this.transform.position, ground_hit.point);
                    if (distance_to_ground < distance_check)
                    {
                        hit_dir = ground_hit.point - this.transform.position;
                        distance_check = distance_to_ground;
                    }
                }
            }
        }

        Debug.DrawLine(transform.position, transform.position + (hit_dir.normalized * 5f), Color.blue);

        return hit_dir.normalized;
    }
}