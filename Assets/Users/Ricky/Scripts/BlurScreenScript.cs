using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlurScreenScript : MonoBehaviour
{
    private Material blur_mat;
    private float origin_divide_num;
    
    private float divide_num;
    private float real_divide_num;
    
    private float target_power;
    private float real_power;
    
    private float lerp_t;
    
    private bool increase;
    
    // Start is called before the first frame update
    void Start()
    {
        blur_mat = GetComponent<Renderer>().materials[0];
        
        origin_divide_num = 2.54f;
        divide_num = 1.5f;
        real_divide_num = divide_num;
        
        blur_mat.SetFloat("_Power", 0.0015f);
        
        target_power = 0.05f;
        real_power = target_power;
        
        increase = false;
        
        lerp_t = 0;
    }

    // Update is called once per frame
    void Update()
    {
        real_divide_num = Mathf.Lerp(origin_divide_num, divide_num, lerp_t);
        real_power = Mathf.Lerp(0, target_power, lerp_t);
        
        if (increase)
        {
            lerp_t += Time.unscaledDeltaTime / 4.0f;
        }
        else
        {
            lerp_t -= Time.unscaledDeltaTime / 2.0f;
        }
        
        lerp_t = Mathf.Clamp(lerp_t, 0, 1);
        
        blur_mat.SetFloat("_Power", real_power);
        blur_mat.SetFloat("_Divide", real_divide_num);
    }
    
    public void SetPower(float pow)
    {
        target_power = pow;
    }
    
    public void SetTargetDivide(float targetDiv)
    {
        divide_num = targetDiv;
    }
    
    public void SetState(bool state)
    {
        increase = state;
    }
    
    public void StartClean()
    {
        Destroy(this.gameObject);
    }
}
