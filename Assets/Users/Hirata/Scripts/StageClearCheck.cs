using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageClearCheck : MonoBehaviour
{
    GameObject ClearCanvas;

    List<LightBulb> lightBulbs = new List<LightBulb>();

    bool once = true;

    void Start()
    {
        ClearCanvas = (GameObject)Resources.Load("ClearCanvas");
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("LightBulb");

        foreach (GameObject gameObject in gameObjects)
        {
            lightBulbs.Add(gameObject.GetComponent<LightBulb>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (once)
        {
            bool Is_StageClear = true;
            foreach (LightBulb gameObject in lightBulbs)
            {
                if (Is_StageClear)
                {
                    if (!gameObject.is_stage_hit)
                    {
                        Is_StageClear = false;
                    }
                }
            }
            if (Is_StageClear)
            {
                Instantiate(ClearCanvas);
                once = false;
            }
        }
    }
}