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
        
        line_renderer.startWidth = background_size.y / 2.0f / collector.LightBulb_num;
        line_renderer.endWidth = background_size.y / 2.0f / collector.LightBulb_num;
        
        max_time = current_bulb.GetDestroyTime();
        current_time = current_bulb.GetCurrentTimer();
    }

    // Update is called once per frame
    void Update()
    {
        if (current_bulb.is_stage_hit)
        {
            Vector3[] points = new Vector3[2];
            line_renderer.GetPositions(points);
            
            points[points.Length - 1].x = (background_size.x / 2.0f * -1.0f) + (background_size.x / 2.0f * (current_time / max_time));
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}