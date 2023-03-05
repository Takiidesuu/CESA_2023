using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [Header("Clipping Plane")]
    [SerializeField] private float near = 0.3f;
    [SerializeField] private float far = 1000.0f;
    
    [Header("Camera Params")]
    [Tooltip("カメラの距離")]
    [SerializeField] private Vector3 camera_offset;
    
    Camera camera_component;
    Transform player_pos;
    
    // Start is called before the first frame update
    void Start()
    {
        camera_component = GetComponent<Camera>();
        player_pos = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        camera_component.nearClipPlane = near;
        camera_component.farClipPlane = far;
        
        transform.position = player_pos.position + camera_offset;
        //transform.rotation = player_pos.rotation;
    }
    
    private void LateUpdate() 
    {
        //transform.eulerAngles = new Vector3(0.0f, 0.0f, transform.eulerAngles.z);
    }
}
