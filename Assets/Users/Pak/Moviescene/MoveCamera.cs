using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPos = transform.position;
        Vector3 targetPos = new Vector3(10, 10, 10);

        transform.position = Vector3.Lerp(playerPos, targetPos, 0.5f * Time.deltaTime);

    }
}
