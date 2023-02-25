using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Deform;

[ExecuteInEditMode]

public class DefotmCenter : MonoBehaviour
{
    public GameObject[] ChildMeshObject;   //メッシュがあるオブジェクトを格納
    public Deformable[] ChildDefotmbles;
    public SAMeshColliderBuilder SAMeshColliderBuilder;

    void Start()
    {
        //開始時にメッシュがあるオブジェクトを検索格納
        int meshcount = 0;
        for (int i = 0; i < transform.childCount; i++)
            if (transform.GetChild(i).GetComponent<MeshRenderer>())
                meshcount++;
        ChildMeshObject = new GameObject[meshcount];
        ChildDefotmbles = new Deformable[meshcount];
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
            else
                ChildDefotmbles[i] = ChildMeshObject[i].GetComponent<Deformable>();
        }

        //メッシュコライダー適用
        if (!gameObject.GetComponent<SAMeshColliderBuilder>())
            SAMeshColliderBuilder = gameObject.AddComponent<SAMeshColliderBuilder>();
        else
            SAMeshColliderBuilder = gameObject.GetComponent<SAMeshColliderBuilder>();
        SAMeshColliderBuilder.reducerProperty.shapeType = SAColliderBuilderCommon.ShapeType.Mesh;
        SAMeshColliderBuilder.reducerProperty.meshType = SAColliderBuilderCommon.MeshType.Raw;
    }

    void Update()
    {

    }
}