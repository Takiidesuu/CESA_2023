using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SlopeController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float slopeForce = 100f;
    public float slopeRayLength = 10f;

    private Rigidbody rb;
    private float slopeAngle;
    private Vector3 fowordVec;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector3 rayOrigin =transform.position; // Rayの始点
        Vector3 rayDirection = transform.up * -1; // Rayの方向
        fowordVec = transform.right;

        Debug.Log(fowordVec);

        float rayDistance = 10f; // Rayの長さ

        // Rayを可視化する
        Debug.DrawRay(rayOrigin, rayDirection * slopeRayLength, Color.green);

        RaycastHit hit;
        if (Physics.Raycast(transform.position,transform.up * -1, out hit, slopeRayLength, LayerMask.GetMask("Ground")))
        {
            slopeAngle = 180 - Vector3.Angle(hit.normal,transform.up * - 1);
           

        }
        else
        {
            slopeAngle = 0f;
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(h, 0, v).normalized;
        rb.AddForce(moveDirection * moveSpeed * Time.fixedDeltaTime, ForceMode.VelocityChange);
    }
}