using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Deform;

public class DefotmCenter : MonoBehaviour
{
    private GameObject[] ChildMeshObject;   //メッシュがあるオブジェクトを格納

    void Start()
    {
        //開始時にメッシュがあるオブジェクトを検索格納
        int meshcount = 0;
        for (int i = 0; i < transform.childCount; i++)
            if (transform.GetChild(i).GetComponent<MeshRenderer>())
                meshcount++;
        ChildMeshObject = new GameObject[meshcount];
        meshcount = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<MeshRenderer>())
            {
                ChildMeshObject[meshcount] = transform.GetChild(i).gameObject;
                meshcount++;
            }
        }

        //メッシュがあるものにDeformコンポーネントを追加
        foreach(GameObject gameObject in ChildMeshObject)
            gameObject.AddComponent<Deformable>();
        
    }

    void Update()
    {
        
    }
}
