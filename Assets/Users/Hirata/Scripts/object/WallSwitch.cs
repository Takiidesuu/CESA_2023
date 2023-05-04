using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSwitch : MonoBehaviour
{
    private bool player_hit;

    public bool GetIsHit()
    {
        return player_hit;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player_hit = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player_hit = false;
        }
    }

    public void WallMove()
    {
        transform.GetChild(0).GetComponent<WallMove>().OnceWallMove();
    }
}
