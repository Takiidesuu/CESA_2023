using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityControl : MonoBehaviour
{
    private const float gravity_number = 9.81f;
    
    [Tooltip("重力")]
    [SerializeField] private float gravity_power = 5.0f;
    [Tooltip("重力の加算")]
    [SerializeField] private float gravity_increase_power = 2.0f;
    [Tooltip("回転速度")]
    [SerializeField] private float rotation_speed = 20.0f;
    [Tooltip("戻るまでの時間")]
    [SerializeField] private float time_to_warp = 3.0f;
    
    [Tooltip("火花")]
    [SerializeField] private GameObject fire_effect;

    private Rigidbody rb;

    private float real_gravity_power;
    private Vector3 gravity_dir;
    private LayerMask ground_layer_mask;
    
    private bool on_ground;
    private bool in_gravfield;
    private float increase_gravity_scalar;
    
    private float time_in_air;
    private bool going_back_to_ground;
    
    private GameObject last_ground_obj;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        real_gravity_power = gravity_power;
        
        ground_layer_mask = LayerMask.GetMask("Ground");
        on_ground = false;
        in_gravfield = false;
        
        increase_gravity_scalar = 1.0f;
        
        if (this.gameObject.tag == "Player")
        {
            time_in_air = time_to_warp;
            going_back_to_ground = true;
        }
        else
        {
            going_back_to_ground = false;
        }
        
        last_ground_obj = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (!in_gravfield)
        {
            time_in_air += Time.deltaTime;
            
            if (time_in_air > time_to_warp - Time.deltaTime && time_in_air < time_to_warp + Time.deltaTime)
            {
                StartCoroutine(WarpBackToGround());
            }
        }
        else
        {
            if (this.gameObject.tag == "Player")
            {
                this.transform.GetChild(0).gameObject.SetActive(true);
                this.transform.GetChild(2).gameObject.SetActive(false);
            }
            
            time_in_air = 0.0f;
        }
    }

    private void FixedUpdate()
    {
        if (in_gravfield)
        {
            gravity_dir = CheckFloorAngle();
        }
        else
        {
            if (time_in_air < time_to_warp)
            {
                gravity_dir = new Vector3(0.0f, -1.0f, 0.0f);
            }
        }

        // Finds desired rotation relative to surface normal
        var targetRotation = Quaternion.FromToRotation(transform.up, gravity_dir * -1.0f) * transform.rotation;

        // Apply rotation and gravity
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotation_speed * Time.deltaTime);
        
        if (Physics.Raycast(this.transform.position, gravity_dir, 3.0f, ground_layer_mask))
        {
            on_ground = true;
            
            if (going_back_to_ground)
            {
                if (this.gameObject.tag == "Player")
                {
                    this.GetComponent<PlayerMove>().start_game = true;
                    Vector3 spawn_pos = this.transform.position - this.transform.up * 0.5f;
                    GameObject first = Instantiate(fire_effect, spawn_pos, this.transform.rotation);
                    first.transform.Rotate(new Vector3(0, 0, 45), Space.World);
                    GameObject second = Instantiate(fire_effect, spawn_pos, this.transform.rotation);
                    second.transform.Rotate(new Vector3(0, 0, -45), Space.World);
                    
                    Invoke("Shake", 0.05f);
                }
                
                going_back_to_ground = false;
            }
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
            float gravity_speed_scalar = 15.0f;
            increase_gravity_scalar += Time.deltaTime / gravity_speed_scalar * gravity_increase_power;
        }

        Vector3 real_grav_dir = gravity_dir * gravity_number * real_gravity_power * increase_gravity_scalar;
        rb.AddForce(real_grav_dir);
    }
    
    void Shake()
    {
        GameObject.Find("Main Camera").GetComponent<CameraMove>().ShakeCamera(2.0f, 0.1f, this.transform.up);
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
        
        float ray_length = 5.0f;

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
            GameObject found_obj = null;
            
            if (hit_front.transform != null)
            {
                hit_dir += hit_front.normal;
                found_obj = hit_front.transform.root.gameObject;
            }
            if (hit_centre.transform != null)
            {
                hit_dir += hit_centre.normal;
                found_obj = hit_centre.transform.root.gameObject;
            }
            if (hit_back.transform != null)
            {
                hit_dir += hit_back.normal;
                found_obj = hit_back.transform.root.gameObject;
            }
            
            if (found_obj)
            {
                if (last_ground_obj != found_obj)
                {
                    last_ground_obj = found_obj;
                }
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
            
            ray_length = 30.0f;
            
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
                        
                        if (last_ground_obj != ray.transform.root.gameObject)
                        {
                            last_ground_obj = ray.transform.root.gameObject;
                        }
                        
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
                                
                                if (last_ground_obj != ground_hit.transform.root.gameObject)
                                {
                                    last_ground_obj = ground_hit.transform.root.gameObject;
                                }
                                
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
    
    IEnumerator WarpBackToGround()
    {
        gravity_dir = Vector3.zero;
        real_gravity_power = 0.0f;
        rb.velocity = Vector3.zero;
        
        if (this.gameObject.tag == "Player")
        {
            this.transform.GetChild(0).gameObject.SetActive(false);
            this.transform.GetChild(2).gameObject.SetActive(true);
        }
        
        yield return new WaitForSeconds(1.0f);
        
        going_back_to_ground = true;
        
        if (last_ground_obj == null)
        {
            GameObject[] stage_obj = GameObject.FindGameObjectsWithTag("Stage");
            
            float distance_to_compare = Mathf.Infinity;
            
            foreach (var current in stage_obj)
            {
                if (current.name.Contains("Stage"))
                {
                    float distance_to_current = Vector3.Distance(this.transform.position, current.transform.position);
                    if (distance_to_current < distance_to_compare)
                    {
                        distance_to_compare = distance_to_current;
                        last_ground_obj = current;
                    }
                }
            }
        }
        
        increase_gravity_scalar = 1.0f;
        
        real_gravity_power = gravity_power * 3.0f;
        gravity_dir = last_ground_obj.transform.position - this.transform.position;
        gravity_dir.Normalize();
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }
    
    private void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.tag == "GravityField")
        {
            in_gravfield = true;
        }
    }
    
    private void OnTriggerStay(Collider other) 
    {
        if (other.gameObject.tag == "GravityField")
        {
            in_gravfield = true;
        }
    }
    
    private void OnTriggerExit(Collider other) 
    {
        if (other.gameObject.tag == "GravityField")
        {
            in_gravfield = false;
        }
    }
}