using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour 
{
    [Header("距離")]
    [Tooltip("プレイヤーからの距離")]
    [SerializeField] private float player_distance = 5.0f;
    [Tooltip("ステージからの距離")]
    [SerializeField] private float distance_from_stage = 70.0f;
    
    [Header("設定")]
    [Tooltip("FOV")]
    [SerializeField] private float camera_fov = 15.0f;
    [Tooltip("カメラ揺れ力")]
    [SerializeField] private float camera_shake_power = 5.0f;
    
    Camera cam;
    
    GameObject player_obj;
    GameObject lookat_pos;
    
    Vector3 origin_pos;
    
    private float distance_scalar;
    
    private bool shake_camera;
    private float shake_power;
    
    public bool is_zooming {get; private set;}
    
    public void ShakeCamera(float fpower, float fduration, Vector3 player_up_vec)
    {
        shake_power = fpower * camera_shake_power;
        shake_camera = true;
        
        StartCoroutine("StopShake", fduration);
    }
    
    public void IsLastBulb(bool state, GameObject lookat)
    {
        is_zooming = state;
    }
    
    private void Start() 
    {
        cam = GetComponent<Camera>();
        cam.fieldOfView = camera_fov;
        GameObject.Find("SubCamera").GetComponent<Camera>().fieldOfView = camera_fov;
        
        player_obj = GameObject.FindGameObjectWithTag("Player");
        lookat_pos = new GameObject("CameraLookAtObj");
        
        distance_scalar = 1.0f;
        
        shake_camera = false;
        is_zooming = false;
    }
    
    private void Update() 
    {
        origin_pos = this.transform.position;
    }
    
    private void LateUpdate() 
    {
        float distance_from_obj;
        GameObject target_obj = player_obj.GetComponent<PlayerMove>().GetCurrentGravObj();
        
        bool is_player_smashing = player_obj.GetComponent<PlayerMove>().GetSmashingState();
        
        Vector3 targetLookAt;
        
        if (target_obj)
        {
            distance_from_obj = distance_from_stage * 10.0f * distance_scalar;
            targetLookAt = target_obj.transform.position;
        }
        else
        {
            target_obj = GameObject.FindGameObjectWithTag("Stage");
            targetLookAt = player_obj.transform.position;
            distance_from_obj = distance_from_stage * 10.0f * distance_scalar;
        }
        
        if (distance_scalar < 1.0f)
        {
            distance_scalar += Time.deltaTime * 8.0f;
        }
        else
        {
            distance_scalar = 1.0f;
        }
        
        if (is_zooming)
        {
            distance_from_obj = 10;
        }
        
        Vector3 target_pos = new Vector3(target_obj.transform.position.x, target_obj.transform.position.y, target_obj.transform.position.z - Mathf.Abs(distance_from_obj));
        
        lookat_pos.transform.position = targetLookAt;
        transform.position = Vector3.MoveTowards(transform.position, target_pos, 300.0f * Time.deltaTime);
        
        transform.LookAt(lookat_pos.transform, Vector3.up);
        
        if (shake_camera)
        {
            transform.position = origin_pos + Random.insideUnitSphere * shake_power * Time.timeScale;
        }
    }
    
    IEnumerator StopShake(float time_to_stop)
    {
        yield return new WaitForSeconds(time_to_stop);
        
        shake_camera = false;
    }
}