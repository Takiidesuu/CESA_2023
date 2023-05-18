using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitstopManager : MonoBehaviour
{
    public static HitstopManager instance {get; private set;}
    
    private Animator player_anim;
    private float anim_speed_record;
    
    private float elapsed_time;
    private float stop_time;
    
    private bool stop_flg;
    
    public bool is_stopped {get; private set;}
    
    public void StartHitStop(float duration)
    {
        stop_time = duration;
        elapsed_time = 0.0f;
        anim_speed_record = player_anim.speed;
        
        stop_flg = true;
    }
    
    private void Awake() 
    {
        if (instance != null && instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            instance = this; 
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        player_anim = GameObject.FindGameObjectWithTag("Player").transform.GetChild(0).GetComponent<Animator>();
        elapsed_time = 10.0f;
        stop_time = 0.0f;
        
        is_stopped = false;
        stop_flg = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (stop_flg)
        {
            if (elapsed_time < stop_time)
            {
                player_anim.speed = anim_speed_record / 1000.0f;
                elapsed_time += Time.unscaledDeltaTime;
            }
            else
            {
                stop_flg = false;
                player_anim.speed = anim_speed_record;
            }
            
            is_stopped = true;
        }
        else
        {
            is_stopped = false;
        }
    }
}