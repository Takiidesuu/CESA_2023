using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulbLineScript : MonoBehaviour
{
    private Vector2 background_size = new Vector2(500.0f, 250.0f);
    
    public LightBulb current_bulb {private get; set;}
    
    private RawImage bar;
    private LightBulbCollector collector;
    
    private RectTransform rect_transform;
    
    [SerializeField] private Texture2D image;
    
    private float max_time;
    private float current_time;
    
    private float starting_size;
    
    // Start is called before the first frame update
    void Start()
    {
        transform.SetParent(GameObject.Find("BulbLineMask").transform);
        
        bar = GetComponent<RawImage>();
        collector = GameObject.FindObjectOfType<LightBulbCollector>();
        
        rect_transform = GetComponent<RectTransform>();
        
        rect_transform.pivot = new Vector2(1.0f, 0.5f);
        rect_transform.sizeDelta = new Vector2(500.0f, background_size.y / collector.LightBulb_num);
        
        max_time = current_bulb.GetDestroyTime();
    }

    // Update is called once per frame
    void Update()
    {
        current_time = current_bulb.GetCurrentTimer();
        starting_size = (rect_transform.anchoredPosition3D.x + background_size.x / 2.0f) / background_size.x;
        
        if (current_bulb.is_stage_hit)
        {
            float new_x_scale = starting_size - (starting_size * (current_time / max_time));
            rect_transform.localScale = new Vector3(new_x_scale, 1.0f, 1.0f);
        }
        else
        {
            GameObject.FindObjectOfType<BulbStatusScript>().progress_bar.Remove(this);
            current_bulb.line_status_obj = null;
            Destroy(this.gameObject);
        }
    }
}