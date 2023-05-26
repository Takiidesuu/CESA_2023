using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBulbClearTrigger : MonoBehaviour
{
    public bool slow_motion;
    
    private float return_to_normal_duration = 3.0f;
    private float elapsed_time;
    
    // Start is called before the first frame update
    void Start()
    {
        slow_motion = true;
        
        elapsed_time = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (slow_motion)
        {
            if (elapsed_time > return_to_normal_duration / 2.0f)
            {
                Time.timeScale = 0.5f;
            }
            else
            {
                Time.timeScale = 0.1f;
            }
        }
        else
        {
            Time.timeScale = 1.0f;
            Destroy(this);
        }
    }
    
    private void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.tag == "ElectricalBall")
        {
            transform.parent.GetComponent<LightBulb>().LightUpBulb();
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