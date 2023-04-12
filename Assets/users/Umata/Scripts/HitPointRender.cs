using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitPointRender : MonoBehaviour
{
    public int hitpoint = 10;  // 表示するhitpointの数
    public int width_num = 3;  // 横の表示数
    public int height_num = 4;  // 縦の表示数
    public float Image_width = 50f;  // RowImageの横幅
    public float Image_height = 50f;  // RowImageの縦幅
    public GameObject RowImagePrefab;  // RowImageのPrefab
    public Vector2 ParentPos = new Vector2(0, 0);  // RowImageParentの生成座標
    DamageScript damageScript;    //HP取得用
    private int prev_hitpoint = 0;

    void Start()
    {
        damageScript = GameObject.Find("Player").GetComponent<DamageScript>();
        CreateRowImage();
    }

    void Update()
    {
        hitpoint = damageScript.GetHitPoint();
        if (hitpoint != prev_hitpoint)
        {
            // 旧RowImageParentを破棄
            Destroy(transform.Find("RowImageParent").gameObject);
            CreateRowImage();
            prev_hitpoint = hitpoint;
        }
    }

    void CreateRowImage()
    {

        prev_hitpoint = hitpoint;
        // 1行に表示できる最大数
        int max_num_per_row = width_num;

        // 1行に表示できる最大数を超えた場合の改行
        int row_count = Mathf.CeilToInt((float)hitpoint / max_num_per_row);

        // RowImageParentオブジェクトの生成
        GameObject RowImageParent = new GameObject("RowImageParent");
        RowImageParent.transform.SetParent(transform);
        RowImageParent.transform.localPosition = ParentPos;

        // RowImageを生成
        for (int i = 0; i < hitpoint; i++)
        {
            // RowImageの座標を計算
            int x = i % max_num_per_row;
            int y = Mathf.FloorToInt((float)i / max_num_per_row);

            // RowImageを生成
            GameObject RowImageObj = Instantiate(RowImagePrefab);
            RowImageObj.transform.SetParent(RowImageParent.transform);
            RowImageObj.transform.localPosition = new Vector3(x * Image_width, -y * Image_height, 0f);
            RowImageObj.GetComponent<RectTransform>().sizeDelta = new Vector2(Image_width, Image_height);

        }

        // RowImageParentの位置を調整
        RowImageParent.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
        RowImageParent.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f);

    }
}
