using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KanjaMove : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPos = transform.position;
        Vector3 targetPos = new Vector3(4.0f, 0.5f, -5.0f);

        transform.position = Vector3.MoveTowards(playerPos, targetPos, 0.5f * Time.deltaTime);
    }
}
