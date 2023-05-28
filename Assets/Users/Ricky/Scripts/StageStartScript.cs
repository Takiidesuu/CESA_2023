using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageStartScript : MonoBehaviour
{
    [Tooltip("出てくるエフェクト")]
    [SerializeField] private GameObject start_effect;
    private GameObject player_obj;
    
    private float lerp_t;
    
    private float horizontal_t;
    private float vertical_t;
    
    private float elapsed_time;
    
    private float scale_x;
    private float scale_y;
    
    private bool start;
    
    private Material mat;
    private Color defaultColor;
    
    // Start is called before the first frame update
    void Start()
    {
        player_obj = transform.root.gameObject;
        transform.SetParent(null);
        lerp_t = 0;
        horizontal_t = 0;
        vertical_t = 0;
        scale_x = 0;
        scale_y = 0;
        start = true;
        
        mat = transform.GetChild(0).GetComponent<Renderer>().materials[0];
        
        StartCoroutine(EffectDelay(0.8f));
    }

    // Update is called once per frame
    void Update()
    {
        if (elapsed_time < 0.2f)
        {
            elapsed_time += Time.deltaTime;
        }
        else
        {
            scale_x = Mathf.Lerp(0, 1, horizontal_t);
            scale_y = Mathf.Lerp(0, 1, vertical_t);
            transform.localScale = new Vector3(scale_x, scale_y, 0.1f);
            defaultColor.a = Mathf.Lerp(0, 1, lerp_t);
            mat.SetFloat("_Alpha", lerp_t);
        
            if (start)
            {
                if (horizontal_t < 1)
                {
                    horizontal_t += Time.deltaTime * 10;
                }
                else
                {
                    horizontal_t = 1;
                }
                
                if (horizontal_t > 0.5f)
                {
                    if (vertical_t < 1)
                    {
                        vertical_t += Time.deltaTime * 10;
                    }
                    else
                    {
                        vertical_t = 1;
                    }
                }
                
                if (lerp_t < 1)
                {
                    lerp_t += Time.deltaTime * 5;
                }
                else
                {
                    lerp_t = 1;
                }
            }
            else
            {
                if (vertical_t > 0)
                {
                    vertical_t -= Time.deltaTime * 10;
                }
                else
                {
                    vertical_t = 0;
                }
                
                if (vertical_t < 0.5f)
                {
                    if (horizontal_t > 0)
                    {
                        horizontal_t -= Time.deltaTime * 10;
                    }
                    else
                    {
                        horizontal_t = 0;
                    }
                }
                
                if (lerp_t > 0)
                {
                    lerp_t -= Time.deltaTime * 5;
                }
                else
                {
                    lerp_t = 0;
                }
            }
        }
    }
    
    IEnumerator EffectDelay(float delay)
    {
        yield return new WaitForSeconds(0.1f);
        
        player_obj.transform.GetChild(2).gameObject.SetActive(false);
        
        yield return new WaitForSeconds(delay);
        
        Instantiate(start_effect, this.transform.position + new Vector3(0, -4, -4), Quaternion.identity);
        player_obj.transform.GetChild(2).gameObject.SetActive(true);
        
        yield return new WaitForSeconds(delay * 2);
        
        start = false;
        
        StartCoroutine(DestroyThis());
    }
    
    IEnumerator DestroyThis()
    {
        yield return new WaitForSeconds(2);
        Destroy(this.gameObject);
    }
}