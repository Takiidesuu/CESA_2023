using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestJump : MonoBehaviour
{
    public float jump_power = 5.0f;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            rb.velocity = Vector3.up * jump_power;
        }

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    rb.velocity = Vector3.up * jump_power;
        //}
    }
}
