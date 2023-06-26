using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleEventTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.tag == "PlayerNPC")
        {
            TitleLighting[] light_obj = GameObject.FindObjectsOfType<TitleLighting>();
            foreach (var obj in light_obj)
            {
                obj.isShining = true;
            }
        }
    }
}
