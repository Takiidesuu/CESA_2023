using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SlopeController : MonoBehaviour
{
    enum SLOPE_STATE
    {
        NONE,
        UP,
        DOWN
    }
    
    [Tooltip("加速度")]
    [SerializeField] private float acceleration_speed = 20.0f;
    [Tooltip("減速度")]
    [SerializeField] private float deceleration_speed = 10.0f;
    
    [Tooltip("加速される時間")]
    [SerializeField] private float acceleration_time = 0.2f;
    [Tooltip("減速される時間")]
    [SerializeField] private float deceleration_time = 0.2f;
    
    private float slopeRayLength = 10f;

    private Rigidbody rb;
    private float slopeAngle;
    private Vector3 forwardVec;
    
    private ElectricBallMove move_script;
    private SLOPE_STATE current_slope_state;
    
    public bool IsOnIncline()
    {
        return current_slope_state == SLOPE_STATE.UP ? true : false;
    }
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        move_script = GetComponent<ElectricBallMove>();
        
        current_slope_state = SLOPE_STATE.NONE;
    }

    void FixedUpdate()
    {
        forwardVec = transform.right;

        RaycastHit hit;
        if (Physics.Raycast(transform.position,transform.up * -1, out hit, slopeRayLength, LayerMask.GetMask("Ground")))
        {
            Vector3 center_vec = hit.point - hit.transform.root.gameObject.transform.position;
            float dot_to_center = Vector3.Dot(center_vec.normalized, transform.up);
            
            slopeAngle = Vector3.SignedAngle(center_vec.normalized, transform.up, new Vector3(0, 0, 1));
            
            Vector3 displacement = transform.position + transform.right * 5.0f;
            Vector3 vecToFront = displacement - hit.point;
            float moveDir = Vector3.SignedAngle(transform.up, vecToFront.normalized, new Vector3(0, 0, 1));
            
            if (dot_to_center > 0)
            {
                moveDir *= -1.0f;
            }
            
            if (moveDir < 0.0f)
            {
                slopeAngle *= -1.0f;
            }
            
            bool is_on_slope = false;
            float decelerate_real_time = deceleration_time;
            SLOPE_STATE previous_slope_state = current_slope_state;
            
            // 外側にいる
            if (dot_to_center > 0)
            {
                if (slopeAngle > 10.0f || slopeAngle < -10.0f)
                {
                    // 上り
                    if (Mathf.Sign(slopeAngle) > 0.0f)
                    {
                        current_slope_state = SLOPE_STATE.UP;
                    }
                    else
                    {
                        current_slope_state = SLOPE_STATE.DOWN;
                    }
                    
                    decelerate_real_time /= (50.0f / Mathf.Abs(slopeAngle));
                    is_on_slope = true;
                }
            }
            else
            {
                if (slopeAngle > -170.0f || slopeAngle < -190.0f)
                {
                    // 上り
                    if (Mathf.Sign(slopeAngle) > 0.0f)
                    {
                        current_slope_state = SLOPE_STATE.UP;
                    }
                    else
                    {
                        current_slope_state = SLOPE_STATE.DOWN;
                    }
                    
                    decelerate_real_time /= (50.0f / (Mathf.Abs(slopeAngle + 180.0f)));
                    is_on_slope = true;
                }
            }
            
            if (is_on_slope)
            {
                if (current_slope_state != previous_slope_state)
                {
                    float delay_time = 0.2f;
                    float boost_speed = 0.0f;
                    
                    switch (current_slope_state)
                    {
                        case SLOPE_STATE.UP:
                            boost_speed = -deceleration_speed;
                            delay_time = decelerate_real_time;
                        break;
                        case SLOPE_STATE.DOWN:
                            boost_speed = acceleration_speed;
                            delay_time = acceleration_time;
                        break;
                    }
                    
                    move_script.BoostSpeed(delay_time, boost_speed, delay_time);
                }
            }
            else
            {
                current_slope_state = SLOPE_STATE.NONE;
            }
        }
        else
        {
            slopeAngle = 0f;
        }
    }
}