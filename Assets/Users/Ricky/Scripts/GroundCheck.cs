using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    GameObject playerObj;
    PlayerMove playerMove;      //どのステージに乗っているか取得するため
    private int RemoveFrame = 5;

    public class hit_object
    {
        public GameObject obj;
        public int is_lastframe;
    }
    public List<hit_object> hit_ground;

    void Start()
    {
        playerObj = GameObject.FindGameObjectWithTag("Player");
        playerMove = playerObj.GetComponent<PlayerMove>();
        hit_ground = new List<hit_object>();
    }

    private void Update()
    {
        this.transform.position = playerObj.transform.position;
        
        foreach (hit_object hit in hit_ground)
        {
            hit.is_lastframe++;
        }
        bool is_frame = true;
        while (is_frame)
        {
            for (int i = 0; i < hit_ground.Count; i++)
            {
                if (playerMove.GetGroundObj() == null)
                {
                    break;
                }
                
                if (hit_ground[i].obj.transform.root != playerMove.GetGroundObj().transform.root)
                {
                    hit_ground.RemoveAt(i);
                    break;
                }
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
                //自分が乗っているステージのみへこませる
                if (playerMove.GetGroundObj() != null)
                {
                    if (other.transform.root == playerMove.GetGroundObj().transform.root)
                    {
                        hit_object temp = new hit_object();
                        temp.obj = other.gameObject;
                        temp.is_lastframe = 0;
                        hit_ground.Add(temp);
                    }
                }
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
