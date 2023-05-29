using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBulbClearTrigger : MonoBehaviour
{   
    public bool slow_motion {get; set;}
    
    public BlurScreenScript blur_sc {private get; set;}
    
    private float return_to_normal_duration = 5.0f;
    private float elapsed_time;
    
    private SoundManager soundManager;
    
    // Start is called before the first frame update
    void Start()
    {
        slow_motion = true;
        
        blur_sc = GameObject.FindObjectOfType<BlurScreenScript>();
        soundManager = GetComponent<SoundManager>();
        
        elapsed_time = 0;
    }

    // Update is called once per frame
    void Update()
    {   
        if (slow_motion)
        {
            if (elapsed_time > return_to_normal_duration / 2.0f)
            {
                blur_sc.SetState(false);
                Time.timeScale = 0.45f;
            }
            else
            {
                Time.timeScale = 0.1f;
            }
        }
        else
        {
            blur_sc.StartClean();
            Time.timeScale = 1.0f;
            Destroy(this);
        }
    }
    
    private void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.tag == "ElectricalBall")
        {
            transform.parent.GetComponent<LightBulb>().LightUpBulb();
            soundManager.PlaySoundEffect("ElectricOne");
            soundManager.PlaySoundEffect("ElectricTwo");
            blur_sc.SetState(true);
            StartCoroutine(CountTime());
        }
    }
    
    IEnumerator CountTime()
    {
        while (elapsed_time < return_to_normal_duration)
        {
            elapsed_time += Time.unscaledDeltaTime;
            yield return null;
        }
        
        elapsed_time = return_to_normal_duration;
        slow_motion = false;
    }
}