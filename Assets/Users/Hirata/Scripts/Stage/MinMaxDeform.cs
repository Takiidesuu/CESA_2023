using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinMaxDeform : MonoBehaviour
{
    public MinMaxHitPlayer[] MinCol;
    private MinMaxHitPlayer MaxCol;

    // Start is called before the first frame update
    void Start()
    {
        if (MinCol.Length == 0)
        {
            MinCol = new MinMaxHitPlayer[1];
            MinCol[0] = transform.Find("MinCol").GetComponent<MinMaxHitPlayer>();
        }
        MaxCol = transform.Find("MaxCol").GetComponent<MinMaxHitPlayer>();
    }

    public bool GetMinHit()
    {
        bool Hit = false;
        foreach (MinMaxHitPlayer minMaxHitPlayer in MinCol)
        {
            if (minMaxHitPlayer.GetHit())
            {
                Hit = true;
                break;
            }
        }

        return Hit;
    }

    public bool GetMaxHit()
    {
        return MaxCol.GetHit();
    }
}
