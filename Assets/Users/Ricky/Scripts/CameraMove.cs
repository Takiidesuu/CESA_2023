using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour 
{
    [SerializeField] private float distance = 40.0f;
    
    GameObject player_obj;
    
    private void Start() 
    {
        player_obj = GameObject.FindGameObjectWithTag("Player");
    }
    
    private void LateUpdate() 
    {
        transform.position = new Vector3(player_obj.transform.position.x, player_obj.transform.position.y, player_obj.transform.position.z - distance);
        transform.LookAt(player_obj.transform, player_obj.transform.up);
    }
}