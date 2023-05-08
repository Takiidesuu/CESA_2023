using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinMaxHitPlayer : MonoBehaviour
{
    private bool hit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            hit = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            hit = false;
    }

    public bool GetHit()
    {
        return hit;
    }
}
