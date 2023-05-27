using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlurScreenScript : MonoBehaviour
{
    private Material blur_mat;
    private float origin_divide_num;
    
    private float divide_num;
    private float real_divide_num;
    
    private float elapsed_time;
    
    private float target_power;
    private float real_power;
    
    private bool cleanup;
    
    // Start is called before the first frame update
    void Start()
    {
        blur_mat = GetComponent<Renderer>().materials[0];
        
        origin_divide_num = 2.26f;
        divide_num = origin_divide_num;
        real_divide_num = divide_num;
        
        blur_mat.SetFloat("_Power", 0.0015f);
        
        target_power = 0.0015f;
        real_power = target_power;
        
        cleanup = false;
        
        elapsed_time = 0;
    }

    // Update is called once per frame
    void Update()
    {
        real_divide_num = Mathf.MoveTowards(real_divide_num, divide_num, Time.deltaTime);
        real_power = Mathf.MoveTowards(real_power, target_power, Time.deltaTime / 100.0f);
        
        if (cleanup)
        {
            if (real_power == target_power && real_divide_num == divide_num)
            {
                Destroy(this.gameObject);
            }
        }
        
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
    
    public void StartClean()
    {
        cleanup = true;
        
        target_power = 0.0f;
        divide_num = 2.54f;
    }
}
