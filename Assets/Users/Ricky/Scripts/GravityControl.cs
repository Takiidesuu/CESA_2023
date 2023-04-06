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
    
    private LayerMask ground_layer_mask;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        
        ground_layer_mask = LayerMask.GetMask("Ground");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        gravity_dir = CheckFloorAngle();

        // Finds desired rotation relative to surface normal
        var targetRotation = Quaternion.FromToRotation(transform.up, gravity_dir) * transform.rotation;

        // Apply rotation and gravity
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotation_speed * Time.deltaTime);

        rb.AddForce(gravity_dir.normalized * -9.81f * gravity_power);
    }

    private Vector3 CheckFloorAngle()
    {
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
            RaycastHit[] check_ground = new RaycastHit[8];
            
            Vector3 hit_direction = Vector3.zero;
            
            Vector3 front_dir = this.transform.up + this.transform.forward * 0.25f;
            front_dir.Normalize();
            Vector3 back_dir = this.transform.up + this.transform.forward * -0.25f;
            back_dir.Normalize();
            Vector3 front_dir_down = this.transform.up * -1.0f + this.transform.forward * 0.25f;
            front_dir_down.Normalize();
            Vector3 back_dir_down = this.transform.up * -1.0f + this.transform.forward * -0.25f;
            back_dir_down.Normalize();
            
            Physics.Raycast(this.transform.position, transform.up, out check_ground[0], 20.0f, LayerMask.GetMask("Ground"));
            Physics.Raycast(this.transform.position, front_dir, out check_ground[1], 20.0f, LayerMask.GetMask("Ground"));
            Physics.Raycast(this.transform.position, back_dir, out check_ground[2], 20.0f, LayerMask.GetMask("Ground"));
            Physics.Raycast(this.transform.position, transform.forward, out check_ground[3], 20.0f, LayerMask.GetMask("Ground"));
            Physics.Raycast(this.transform.position, transform.forward * -1.0f, out check_ground[4], 20.0f, LayerMask.GetMask("Ground"));
            Physics.Raycast(this.transform.position, front_dir_down, out check_ground[5], 20.0f, LayerMask.GetMask("Ground"));
            Physics.Raycast(this.transform.position, back_dir_down, out check_ground[6], 20.0f, LayerMask.GetMask("Ground"));
            Physics.Raycast(this.transform.position, transform.up * -1.0f, out check_ground[7], 20.0f, LayerMask.GetMask("Ground"));
            
            float check_distance = 40.0f;
            
            foreach (var ray in check_ground)
            {
                if (ray.transform != null)
                {
                    found_ground = true;
                    
                    if (Vector3.Distance(ray.point, this.transform.position) < check_distance)
                    {
                        check_distance = Vector3.Distance(ray.point, this.transform.position);
                        
                        hit_dir = ray.point - this.transform.position;
                        hit_dir.Normalize();
                        hit_dir *= -1.0f;
                    }
                }
            }
            
                
            if (!found_ground)
            {
                float sphere_size = 50.0f;
                Collider[] col_info = Physics.OverlapSphere(this.transform.position, sphere_size, ground_layer_mask);
                float distance_check = sphere_size;
                foreach (var current in col_info)
                {
                    RaycastHit ground_hit;
                    if (Physics.Raycast(this.transform.position, current.transform.position, out ground_hit, 50.0f, ground_layer_mask))
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
        }

        Debug.DrawLine(transform.position, transform.position + (hit_dir.normalized * 30.0f), Color.blue);

        return hit_dir.normalized;
    }
}