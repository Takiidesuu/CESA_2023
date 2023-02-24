using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Deform;

//[ExecuteInEditMode]

public class DefotmCenter : MonoBehaviour
{
    public GameObject[] ChildMeshObject;   //メッシュがあるオブジェクトを格納
    public Deformable[] ChildDefotmbles;
    public SAMeshCollider[] ChildMeshCollider;

    void Start()
    {
        //開始時にメッシュがあるオブジェクトを検索格納
        int meshcount = 0;
        for (int i = 0; i < transform.childCount; i++)
            if (transform.GetChild(i).GetComponent<MeshRenderer>())
                meshcount++;
        ChildMeshObject = new GameObject[meshcount];
        ChildDefotmbles = new Deformable[meshcount];
        ChildMeshCollider = new SAMeshCollider[meshcount];
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
        for (int i = 0; i < ChildMeshObject.Length; i++)
        {
            if (!ChildMeshObject[i].GetComponent<Deformable>())
                ChildDefotmbles[i] = ChildMeshObject[i].AddComponent<Deformable>();

            if (!ChildMeshObject[i].GetComponent<SAMeshCollider>())
                ChildMeshCollider[i] = ChildMeshObject[i].AddComponent<SAMeshCollider>();
        }
    }

    void Update()
    {
        
    }
}
