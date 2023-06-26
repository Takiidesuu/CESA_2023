using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitPointRender : MonoBehaviour
{
    public int hitpoint = 10;  // �\������hitpoint�̐�
    public int width_num = 3;  // ���̕\����
    public int height_num = 4;  // �c�̕\����
    public float Image_width = 50f;  // RowImage�̉���
    public float Image_height = 50f;  // RowImage�̏c��
    public GameObject RowImagePrefab;  // RowImage��Prefab
    public Vector2 ParentPos = new Vector2(0, 0);  // RowImageParent�̐������W
    DamageScript damageScript;    //HP�擾�p
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
            // ��RowImageParent��j��
            Destroy(transform.Find("RowImageParent").gameObject);
            CreateRowImage();
            prev_hitpoint = hitpoint;
        }
    }

    void CreateRowImage()
    {

        prev_hitpoint = hitpoint;
        // 1�s�ɕ\���ł���ő吔
        int max_num_per_row = width_num;

        // 1�s�ɕ\���ł���ő吔�𒴂����ꍇ�̉��s
        int row_count = Mathf.CeilToInt((float)hitpoint / max_num_per_row);

        // RowImageParent�I�u�W�F�N�g�̐���
        GameObject RowImageParent = new GameObject("RowImageParent");
        RowImageParent.AddComponent<RectTransform>();
        RowImageParent.transform.SetParent(transform);
        RowImageParent.transform.localPosition = ParentPos;
        RowImageParent.transform.SetSiblingIndex(4);

        // RowImage�𐶐�
        for (int i = 0; i < hitpoint; i++)
        {
            // RowImage�̍��W���v�Z
            int x = i % max_num_per_row;
            int y = Mathf.FloorToInt((float)i / max_num_per_row);

            // RowImage�𐶐�
            GameObject RowImageObj = Instantiate(RowImagePrefab);
            RowImageObj.transform.SetParent(RowImageParent.transform);
            RowImageObj.transform.localPosition = new Vector3(x * Image_width, -y * Image_height, 0f);
            RowImageObj.GetComponent<RectTransform>().sizeDelta = new Vector2(Image_width, Image_height);

        }

        // RowImageParent�̈ʒu�𒲐�
        RowImageParent.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
        RowImageParent.GetComponent<RectTransform>().anchoredPosition = new Vector2(ParentPos.x,ParentPos.y);

    }
}
