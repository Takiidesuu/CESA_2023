using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Deform;

[ExecuteInEditMode]

public class DeformStage : MonoBehaviour
{
    private GameObject[] ChildMeshObject;   //メッシュがあるオブジェクトを格納
    private Deformable[] ChildDefotmbles;
    private SAMeshColliderBuilder SAMeshColliderBuilder;

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

        for (int i = 0; i < ChildMeshObject.Length; i++)
        {
            ChildMeshObject[i].transform.GetChild(0).GetChild(0).gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            ChildDefotmbles[i].ColliderRecalculation = ColliderRecalculation.Auto;
            ChildDefotmbles[i].MeshCollider = ChildMeshObject[i].transform.GetChild(0).GetChild(0).gameObject.GetComponent<MeshCollider>();
        }
    }
}