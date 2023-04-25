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
    
    private bool on_ground;
    private float increase_gravity_scalar;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        
        ground_layer_mask = LayerMask.GetMask("Ground");
        on_ground = false;
        
        increase_gravity_scalar = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        gravity_dir = CheckFloorAngle();

        // Finds desired rotation relative to surface normal
        var targetRotation = Quaternion.FromToRotation(transform.up, gravity_dir * -1.0f) * transform.rotation;

        // Apply rotation and gravity
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotation_speed * Time.deltaTime);
        
        if (Physics.Raycast(this.transform.position, gravity_dir, 1.0f, ground_layer_mask))
        {
            on_ground = true;
        }
        else
        {
            on_ground = false;
        }
        
        if (on_ground)
        {
            increase_gravity_scalar = 1.0f;
        }
        else
        {
            increase_gravity_scalar += Time.deltaTime;
        }

        rb.AddForce(gravity_dir * 9.81f * gravity_power);
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
        
        float ray_length = 20.0f;

        found_ground = Physics.Raycast(feet_pos + this.transform.right * 0.2f, dir_offset_front, out hit_front, ray_length, ground_layer_mask);

        if (!found_ground)
        {
            found_ground = Physics.Raycast(feet_pos, this.transform.up * -1.0f, out hit_centre, ray_length, ground_layer_mask);
        }
        else
        {
            Physics.Raycast(feet_pos, this.transform.up * -1.0f, out hit_centre, ray_length, ground_layer_mask);
        }

        if (!found_ground)
        {
            found_ground = Physics.Raycast(feet_pos - this.transform.right * 0.2f, dir_offset_back, out hit_back, ray_length, ground_layer_mask);
        }
        else
        {
            Physics.Raycast(feet_pos - this.transform.right * 0.2f, dir_offset_back, out hit_back, ray_length, ground_layer_mask);
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
            
            ray_length = 40.0f;
            
            Physics.Raycast(this.transform.position, transform.up, out check_ground[0], ray_length, ground_layer_mask);
            Physics.Raycast(this.transform.position, front_dir, out check_ground[1], ray_length, ground_layer_mask);
            Physics.Raycast(this.transform.position, back_dir, out check_ground[2], ray_length, ground_layer_mask);
            Physics.Raycast(this.transform.position, transform.forward, out check_ground[3], ray_length, ground_layer_mask);
            Physics.Raycast(this.transform.position, transform.forward * -1.0f, out check_ground[4], ray_length, ground_layer_mask);
            Physics.Raycast(this.transform.position, front_dir_down, out check_ground[5], ray_length, ground_layer_mask);
            Physics.Raycast(this.transform.position, back_dir_down, out check_ground[6], ray_length, ground_layer_mask);
            Physics.Raycast(this.transform.position, transform.up * -1.0f, out check_ground[7], ray_length, ground_layer_mask);
            
            float check_distance = ray_length;
            
            foreach (var ray in check_ground)
            {
                if (ray.transform != null)
                {
                    found_ground = true;
                    
                    if (Vector3.Distance(ray.point, this.transform.position) < check_distance)
                    {
                        hit_dir = this.transform.position - ray.point;
                        check_distance = Vector3.Distance(ray.point, this.transform.position);
                    }
                }
            }
                
            if (!found_ground)
            {
                float sphere_size = 100.0f;
                int max_colliders = 10;
                Collider[] col_info = new Collider[max_colliders];
                int col_num = Physics.OverlapSphereNonAlloc(this.transform.position, sphere_size, col_info, ground_layer_mask);
                float distance_check = sphere_size;
                foreach (var current in col_info)
                {
                    RaycastHit ground_hit;
                    if (current != null)
                    {
                        Vector3 dir_to_ground = (current.gameObject.transform.root.position - this.transform.position).normalized;
                        if (Physics.Raycast(this.transform.position, dir_to_ground, out ground_hit, sphere_size, ground_layer_mask))
                        {
                            float distance_to_ground = Vector3.Distance(this.transform.position, ground_hit.point);
                            if (distance_to_ground < distance_check)
                            {
                                hit_dir = this.transform.position - ground_hit.point;
                                distance_check = distance_to_ground;
                            }
                        }
                    }
                }
            }
        }

        Debug.DrawLine(transform.position, transform.position + (hit_dir.normalized * 30.0f), Color.blue);

        return hit_dir.normalized * -1.0f;
    }
}