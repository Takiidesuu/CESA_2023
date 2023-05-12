using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulbStatusScript : MonoBehaviour
{
    [Tooltip("BulbLineプレハブ")]
    [SerializeField] private GameObject bulb_line_obj_template;
    
    private Vector2 background_size = new Vector2(500.0f, 250.0f);
    
    public List<BulbLineScript> progress_bar {get;set;}
    private LightBulbCollector collector;
    
    private int num_of_bulbs;
    
    public void AddStatus(LightBulb bulb)
    {
        GameObject new_line_obj = Instantiate(bulb_line_obj_template, Vector3.zero, Quaternion.identity);
        new_line_obj.transform.SetParent(this.transform);
        new_line_obj.GetComponent<BulbLineScript>().current_bulb = bulb;
        
        progress_bar.Add(new_line_obj.GetComponent<BulbLineScript>());
    }
    
    // Start is called before the first frame update
    void Start()
    {
        collector = GameObject.FindObjectOfType<LightBulbCollector>();
        
        num_of_bulbs = collector.LightBulb_num;
        
        progress_bar = new List<BulbLineScript>();
    }

    // Update is called once per frame
    void Update()
    {
        float space = background_size.y / (float)num_of_bulbs;
        float y_pos = background_size.y / 2.0f * -1.0f;
        if (num_of_bulbs % 2 == 0)
        {
            y_pos += space / 2.0f;
        }
        
        if (progress_bar.Count > 0)
        {
            for (int i = progress_bar.Count - 1; i >= 0; i--)
            {
                RectTransform rect_transform = progress_bar[i].gameObject.GetComponent<RectTransform>();
                rect_transform.localPosition = new Vector3(0.0f, y_pos, -0.1f);
                rect_transform.localScale = new Vector3(1, 1, 1);
                
                y_pos += space;
            }
        }
    }
}
