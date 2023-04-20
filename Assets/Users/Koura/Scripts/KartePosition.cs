using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartePosition : MonoBehaviour
{
    //カルテのポジションを決める
    [SerializeField] Transform target;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //決められたカルテのポジションへ移動を行う
        transform.position = Vector3.MoveTowards(target.position, transform.position, Time.deltaTime);
    }
}
