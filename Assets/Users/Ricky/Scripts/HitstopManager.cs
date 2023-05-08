using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitstopManager : MonoBehaviour
{
    public static HitstopManager instance {get; private set;}
    
    ParticleSystem[] scene_particles;
    
    private float elapsed_time;
    private float stop_time;
    
    public bool is_stopped {get; private set;}
    
    public void StartHitStop(float duration)
    {
        stop_time = duration;
        elapsed_time = 0.0f;
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
        scene_particles = GameObject.FindObjectsOfType<ParticleSystem>();
        elapsed_time = 10.0f;
        stop_time = 0.0f;
        
        is_stopped = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (elapsed_time < stop_time)
        {
            Time.timeScale = 0.0f;
            elapsed_time += Time.unscaledDeltaTime;
            is_stopped = true;
        }
        else
        {
            is_stopped = false;
            
            if (!PauseManager.instance.pause_flg)
            {
                Time.timeScale = 1.0f;
            }
        }
    }
}