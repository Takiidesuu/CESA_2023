using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckRadius : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        List<Mesh> mesh = new List<Mesh>();
        List<Vector3 []> vertices = new List<Vector3 []>();

        foreach(Transform child in transform)
        {
            if (child.GetComponent<MeshFilter>())
            {
                mesh.Add(child.GetComponent<MeshFilter>().mesh);
            }
        }
        foreach(Mesh mesh1 in mesh)
        {
            vertices.Add(mesh1.vertices);
        }

        // 最も遠い2点を見つける
        float maxDistance = 0f;
        foreach(Vector3[] vector3 in vertices)
        {
            for (int i = 0; i < vector3.Length; i++)
            {
                for (int j = i + 1; j < vector3.Length; j++)
                {
                    float distance = Vector3.Distance(vector3[i], vector3[j]);
                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                    }
                }
            }
        }


        // スケールを適用して直径を計算する
        float diameter = maxDistance * Mathf.Max(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        Debug.Log("直径：" + diameter);
    }
}
