using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour 
{
    [Tooltip("プレイヤーからの距離")]
    [SerializeField] private float distance = 5.0f;
    
    [Tooltip("元の位置に戻る速度")]
    [SerializeField] private float return_speed = 5.0f;
    [Tooltip("切り替わり待ち時間")]
    [SerializeField] private int time_to_return = 60;
    
    [Tooltip("FOV")]
    [SerializeField] private float camera_fov = 15.0f;
    
    Camera cam;
    
    GameObject player_obj;
    GameObject lookat_pos;
    
    private float distance_scalar;
    private int return_count;
    
    private void Start() 
    {
        cam = GetComponent<Camera>();
        cam.fieldOfView = camera_fov;
        
        player_obj = GameObject.FindGameObjectWithTag("Player");
        lookat_pos = new GameObject("CameraLookAtObj");
        
        distance_scalar = 1.0f;
        return_count = 0;
    }
    
    private void LateUpdate() 
    {
        float distance_from_obj;
        GameObject target_obj = player_obj.GetComponent<PlayerMove>().GetGroundObj();
        
        bool is_player_smashing = player_obj.GetComponent<PlayerMove>().GetSmashingState();
        
        if (target_obj && !is_player_smashing)
        {
            Vector3 target_obj_size = target_obj.GetComponent<Collider>().bounds.size;
            distance_from_obj = target_obj_size.x;
            if (distance_from_obj < target_obj_size.y)
            {
                distance_from_obj = target_obj_size.y;
            }
            
            distance_from_obj *= distance * distance_scalar;
        }
        else
        {
            target_obj = player_obj;
            distance_from_obj = distance * 10.0f * distance_scalar;
        }
        
        Vector3 targetLookAt;
        
        if (is_player_smashing)
        {
            distance_scalar = 0.5f;
            targetLookAt = player_obj.transform.position;
        }
        else
        {
            if (return_count <= 0)
            {
                targetLookAt = target_obj.transform.position;
            }
            else
            {
                targetLookAt = player_obj.transform.position;
            }
            
            if (distance_scalar < 1.0f)
            {
                distance_scalar += Time.deltaTime * 8.0f;
            }
            else
            {
                distance_scalar = 1.0f;
            }
        }
        
        Vector3 target_pos = new Vector3(target_obj.transform.position.x, target_obj.transform.position.y, target_obj.transform.position.z - distance_from_obj);
        
        if (is_player_smashing)
        {
            lookat_pos.transform.position = targetLookAt;
            transform.position = target_pos + player_obj.transform.up * 5.0f;
            return_count = time_to_return;
        }
        else
        {
            if (return_count <= 0)
            {
                lookat_pos.transform.position = Vector3.MoveTowards(lookat_pos.transform.position, targetLookAt, Time.deltaTime * return_speed * 30.0f);
                transform.position = Vector3.MoveTowards(transform.position, target_pos, 300.0f * Time.deltaTime);
            }
            else
            {
                return_count--;
            }
        }
        
        transform.LookAt(lookat_pos.transform, Vector3.up);
    }
}