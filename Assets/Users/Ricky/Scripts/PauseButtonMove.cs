using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseButtonMove : MonoBehaviour
{
    private bool is_selected;
    private float elapsed_time;
    
    private float default_x_pos;
    private float target_x_pos;
    private float default_y_pos;
    private float target_y_pos;
    
    private RectTransform rect_transform;
    
    private int current_menu_id;
    private int selected_menu_id;
    
    public void SetSelectedState(bool is_select, int selected_id)
    {
        is_selected = is_select;
        selected_menu_id = selected_id;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        is_selected = false;
        elapsed_time = 0.0f;
        
        default_x_pos = Screen.width / 2.0f;
        target_x_pos = default_x_pos - 460.0f;
        
        current_menu_id = transform.GetSiblingIndex() + 1;
        
        default_y_pos = Screen.height / 2.0f - ((Screen.height / 6.0f) * current_menu_id + 1);
        
        if (current_menu_id == 4)
        {
            default_y_pos = Screen.height / 2.0f * -1.0f + ((Screen.height / 6.0f) - 60.0f);
        }
        
        rect_transform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (is_selected)
        {
            elapsed_time = Mathf.MoveTowards(elapsed_time, 1.0f, Time.deltaTime);
            target_y_pos = default_y_pos;
        }
        else
        {
            elapsed_time = Mathf.MoveTowards(elapsed_time, 0.0f, Time.deltaTime);
            if (current_menu_id > selected_menu_id)
            {
                target_y_pos = default_y_pos - 10.0f;
            }
            else
            {
                target_y_pos = default_y_pos + 10.0f;
            }
        }
        
        rect_transform.localPosition = Vector3.Lerp(new Vector3(default_x_pos, default_y_pos, rect_transform.localPosition.z), new Vector3(target_x_pos, target_y_pos, rect_transform.localPosition.z), Mathf.Pow(elapsed_time / 1.0f, 2.0f));
    }
}