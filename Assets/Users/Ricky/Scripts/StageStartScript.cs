using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageStartScript : MonoBehaviour
{
    [Tooltip("出てくるエフェクト")]
    [SerializeField] private GameObject start_effect;
    private GameObject player_obj;
    
    private float lerp_t;
    
    private float elapsed_time;
    
    private bool start;
    
    private Material mat;
    private Color defaultColor;
    
    // Start is called before the first frame update
    void Start()
    {
        player_obj = transform.root.gameObject;
        transform.SetParent(null);
        lerp_t = 0;
        start = true;
        
        mat = transform.GetChild(0).GetComponent<Renderer>().materials[0];
        
        StartCoroutine(EffectDelay(0.5f));
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
            transform.localScale = Vector3.Lerp(new Vector3(0, 1, 0.1f), new Vector3(1, 1, 0.1f), lerp_t);
            defaultColor.a = Mathf.Lerp(0, 1, lerp_t);
            mat.SetFloat("_Alpha", lerp_t);
        
            if (start)
            {
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
        
        Instantiate(start_effect, this.transform.position + new Vector3(0, 0, -4), Quaternion.identity);
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