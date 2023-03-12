using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour 
{
    [Tooltip("プレイヤーからの距離")]
    [SerializeField] private float distance = 5.0f;
    [Tooltip("プレイヤーの回転についていくか")]
    [SerializeField] private bool follow_player_rot = false;
    
    GameObject player_obj;
    
    private void Start() 
    {
        player_obj = GameObject.FindGameObjectWithTag("Player");
    }
    
    private void LateUpdate() 
    {
        float distance_from_obj;
        GameObject target_obj = player_obj.GetComponent<PlayerMove>().GetGroundObj();
        
        if (target_obj)
        {
            Vector3 target_obj_size = target_obj.GetComponent<Collider>().bounds.size;
            distance_from_obj = target_obj_size.x;
            if (distance_from_obj < target_obj_size.y)
            {
                distance_from_obj = target_obj_size.y;
            }
            
            distance_from_obj *= 2.0f;
        }
        else
        {
            target_obj = player_obj;
            distance_from_obj = distance * 2.0f;
        }
        
        transform.position = new Vector3(target_obj.transform.position.x, target_obj.transform.position.y, target_obj.transform.position.z - distance_from_obj);
        if (follow_player_rot)
        {
            transform.LookAt(target_obj.transform, target_obj.transform.up);
        }
        else
        {
            transform.LookAt(target_obj.transform, Vector3.up);
        }
    }
}