using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmashCallScript : MonoBehaviour
{
    private PlayerMove player_script;
    // Start is called before the first frame update
    void Start()
    {
        player_script = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMove>();
    }

    public void CallBeforeSmash()
    {
        player_script.BeforeSmashFunc();
    }

    public void CallSmash()
    {
        player_script.SmashFunc();
    }
    
    public void SpawnSparkEffect()
    {
        player_script.SpawnSparks();
    }

    public void ResetAnim()
    {
        player_script.ResetAnim();
    }
}
