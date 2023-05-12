using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour 
{
    [Tooltip("プレイヤーからの距離")]
    [SerializeField] private float player_distance = 5.0f;
    
    [Tooltip("ステージからの距離")]
    [SerializeField] private float distance_from_stage = 70.0f;
    
    [Tooltip("プレイヤーに近づく速度")]
    [SerializeField] private float time_to_player = 10.0f;
    [Tooltip("元の位置に戻る速度")]
    [SerializeField] private float return_speed = 5.0f;
    [Tooltip("切り替わり待ち時間")]
    [SerializeField] private int time_to_return = 60;
    
    [Tooltip("FOV")]
    [SerializeField] private float camera_fov = 15.0f;
    
    [Tooltip("カメラ揺れ力")]
    [SerializeField] private float camera_shake_power = 5.0f;
    
    Camera cam;
    
    GameObject player_obj;
    GameObject lookat_pos;
    
    Vector3 origin_pos;
    
    private float distance_scalar;
    private int return_count;
    
    private bool shake_camera;
    private float shake_power;
    
    public bool is_zooming {get; private set;}
    
    public void ShakeCamera(float fpower, float fduration)
    {
        shake_power = fpower * camera_shake_power;
        shake_camera = true;
        
        StartCoroutine("StopShake", fduration);
    }
    
    private void Start() 
    {
        cam = GetComponent<Camera>();
        cam.fieldOfView = camera_fov;
        GameObject.Find("SubCamera").GetComponent<Camera>().fieldOfView = camera_fov;
        
        player_obj = GameObject.FindGameObjectWithTag("Player");
        lookat_pos = new GameObject("CameraLookAtObj");
        
        distance_scalar = 1.0f;
        return_count = 0;
        
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
        
        if (target_obj)
        {
            distance_from_obj = distance_from_stage * 10.0f * distance_scalar;
        }
        else
        {
            target_obj = player_obj;
            distance_from_obj = player_distance * 10.0f * distance_scalar;
        }
        
        Vector3 targetLookAt;
        
        targetLookAt = target_obj.transform.position;
        
        if (distance_scalar < 1.0f)
        {
            distance_scalar += Time.deltaTime * 8.0f;
        }
        else
        {
            distance_scalar = 1.0f;
        }
        
        Vector3 target_pos = new Vector3(target_obj.transform.position.x, target_obj.transform.position.y, target_obj.transform.position.z - Mathf.Abs(distance_from_obj));
        
        lookat_pos.transform.position = Vector3.MoveTowards(lookat_pos.transform.position, targetLookAt, Time.deltaTime * return_speed * 30.0f);
        transform.position = Vector3.MoveTowards(transform.position, target_pos, 300.0f * return_speed * Time.deltaTime);
        
        if (transform.position == target_pos)
        {
            is_zooming = false;
        }
        
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