using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulbStatusScript : MonoBehaviour
{
    [Tooltip("BulbLineプレハブ")]
    [SerializeField] private GameObject bulb_line_obj_template;
    
    private Vector2 background_size = new Vector2(500.0f, 215.0f);
    
    public List<BulbLineScript> progress_bar {get;set;}
    private LightBulbCollector collector;
    
    private int num_of_bulbs;
    
    public GameObject AddStatus(LightBulb bulb)
    {
        GameObject new_line_obj = Instantiate(bulb_line_obj_template, Vector3.zero, Quaternion.identity);
        new_line_obj.transform.SetParent(this.transform);
        new_line_obj.GetComponent<BulbLineScript>().current_bulb = bulb;
        
        progress_bar.Add(new_line_obj.GetComponent<BulbLineScript>());
        
        return new_line_obj;
    }
    
    public void ResetStatus(LightBulb bulb)
    {
        progress_bar.Remove(bulb.line_status_obj.GetComponent<BulbLineScript>());
        progress_bar.Add(bulb.line_status_obj.GetComponent<BulbLineScript>());
    }
    
    // Start is called before the first frame update
    void Start()
    {
        collector = GameObject.FindObjectOfType<LightBulbCollector>();
        
        progress_bar = new List<BulbLineScript>();
    }

    // Update is called once per frame
    void Update()
    {
        num_of_bulbs = collector.LightBulb_num;

        float space = background_size.y / (float)num_of_bulbs;
        float y_pos = space * -1.0f;

        if (num_of_bulbs % 2 == 0)
        {
            y_pos = space / 2.0f + (space * (num_of_bulbs / 2 - 1));
        }
        else
        {
            y_pos = space * ((num_of_bulbs - 1) / 2);
        }

        y_pos *= -1.0f;
        
        if (progress_bar.Count > 0)
        {
            for (int i = progress_bar.Count - 1; i >= 0; i--)
            {
                RectTransform rect_transform = progress_bar[i].gameObject.GetComponent<RectTransform>();
                float new_x_pos = background_size.x / 2.0f - (y_pos / 2.5f);
                if (y_pos < 0.0f)
                {
                    new_x_pos -= Mathf.Abs(y_pos) / 2.0f;
                }

                rect_transform.anchoredPosition3D = new Vector3(new_x_pos, y_pos, -0.1f);
                
                y_pos += space;
            }
        }
    }
}
