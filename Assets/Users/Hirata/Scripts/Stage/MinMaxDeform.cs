using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinMaxDeform : MonoBehaviour
{
    private MinMaxHitPlayer MinCol;
    private MinMaxHitPlayer MaxCol;


    // Start is called before the first frame update
    void Start()
    {
        MinCol = transform.Find("MinCol").GetComponent<MinMaxHitPlayer>();
        MaxCol = transform.Find("MaxCol").GetComponent<MinMaxHitPlayer>();
    }

    public bool GetMinHit()
    {
        return MinCol.GetHit();
    }

    public bool GetMaxHit()
    {
        return MaxCol.GetHit();
    }
}
