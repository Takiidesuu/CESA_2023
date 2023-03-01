using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    private int RemoveFrame = 5;

    public class hit_object
    {
        public GameObject obj;
        public int is_lastframe;
    }
    public List<hit_object> hit_ground;

    void Start()
    {
        hit_ground = new List<hit_object>();
    }

    private void Update()
    {
        foreach (hit_object hit in hit_ground)
        {
            hit.is_lastframe++;
            Debug.Log(hit.obj.name);
        }
        bool is_frame = true;
        while (is_frame)
        {
            for (int i = 0; i < hit_ground.Count; i++)
            {
                if (hit_ground[i].is_lastframe > RemoveFrame)
                {
                    hit_ground.RemoveAt(i);
                    break;
                }
            }
            is_frame = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        bool isobj = false;
        foreach(hit_object hit in hit_ground)
        {
            if (hit.obj == other.gameObject)
            {
                hit.is_lastframe = 0;
                isobj = true;
            }
        }
        if (!isobj)
        {
            if (other.gameObject.layer == 6)
            {
                hit_object temp = new hit_object();
                temp.obj = other.gameObject;
                temp.is_lastframe = 0;
                hit_ground.Add(temp);
            }
        }
    }

    public GameObject[] GetHitGround()
    {
        GameObject[] gameObjects = new GameObject[hit_ground.Count];

        for (int i = 0; i < hit_ground.Count; i++) 
        {
            gameObjects[i] = hit_ground[i].obj;
        }
        return gameObjects;
    }
}
