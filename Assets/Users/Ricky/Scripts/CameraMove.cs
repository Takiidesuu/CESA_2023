using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour 
{
    [Tooltip("プレイヤーからの距離")]
    [SerializeField] private float distance = 40.0f;
    [Tooltip("プレイヤーの回転についていくか")]
    [SerializeField] private bool follow_player_rot = false;
    
    GameObject player_obj;
    
    private void Start() 
    {
        player_obj = GameObject.FindGameObjectWithTag("Player");
    }
    
    private void LateUpdate() 
    {
        transform.position = new Vector3(player_obj.transform.position.x, player_obj.transform.position.y, player_obj.transform.position.z - distance);
        
        if (follow_player_rot)
        {
            transform.LookAt(player_obj.transform, player_obj.transform.up);
        }
        else
        {
            transform.LookAt(player_obj.transform, Vector3.up);
        }
    }
}