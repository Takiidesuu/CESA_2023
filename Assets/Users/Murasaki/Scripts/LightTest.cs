using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightTest : MonoBehaviour
{
    public bool is_Hit = false;
    private MeshRenderer material;

    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (is_Hit)
        {
            material.material.color = Color.red;
        }
        else
        {
            material.material.color = Color.grey;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == ("PlayerNPC"))
        {
            is_Hit = true;
        }
    }

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.gameObject.tag == ("PlayerNPC"))
    //    {
    //        is_Hit = false;
    //    }
    //}
}
