using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulbLineScript : MonoBehaviour
{
    private Vector2 background_size = new Vector2(500.0f, 250.0f);
    
    public LightBulb current_bulb {private get; set;}
    
    private LineRenderer line_renderer;
    private LightBulbCollector collector;
    
    private float max_time;
    private float current_time;
    
    // Start is called before the first frame update
    void Start()
    {
        line_renderer = GetComponent<LineRenderer>();
        collector = GameObject.FindObjectOfType<LightBulbCollector>();
        
        line_renderer.startWidth = background_size.y / collector.LightBulb_num;
        line_renderer.endWidth = background_size.y / collector.LightBulb_num;
        
        max_time = current_bulb.GetDestroyTime();
    }

    // Update is called once per frame
    void Update()
    {
        current_time = current_bulb.GetCurrentTimer();
        
        if (current_bulb.is_stage_hit)
        {
            Vector3 point_new_pos = new Vector3(0, 0, -0.1f);
            point_new_pos.x = ((background_size.x / 2.0f - 40.0f) * -1.0f) + ((background_size.x - 40.0f * 2.0f) * (current_time / max_time));
            line_renderer.SetPosition(1, point_new_pos);
        }
        else
        {
            GameObject.FindObjectOfType<BulbStatusScript>().progress_bar.Remove(this);
            Destroy(this.gameObject);
        }
    }
}