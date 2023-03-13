using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopEnemy : MonoBehaviour
{
    Vector3 start_position;
    Rigidbody rb;
    bool is_stop = true;

    public int speed;

    private void Start()
    {
        start_position = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
         if (start_position != transform.position && is_stop)
         {
            rb.velocity = transform.position - start_position;
            rb.velocity *= speed;
            is_stop = false;
         }
    }
}
