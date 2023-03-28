using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageHit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionStay(Collision collision)
    {
        Debug.Log("Collision hit" + collision.gameObject.name);
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("Trigger hit" + other.gameObject.name);
    }
}
