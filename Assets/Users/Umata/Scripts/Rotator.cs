using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float rotate_speed = 10f;
    public GameObject[] rotate_objects;  //回転対象オブジェクト
    public GameObject trigger_switch;    //スイッチオブジェクト

    private GameObject modeldata;    //モデルデータ
    public bool is_rotate = true;   //1 = 回転
    public Material active_material; //起動時マテリアル
    public Material inactive_material;  //非稼働時マテリアル
    private Renderer object_renderer;

    void Start()
    {
        modeldata = transform.Find("model").gameObject;
        object_renderer = modeldata.GetComponent<Renderer>();
        UpdateMaterial();
    }

    void Update()
    {
        if (is_rotate)
        {
            foreach (GameObject obj in rotate_objects)
            {
                obj.transform.RotateAround(transform.position, Vector3.forward, rotate_speed * Time.deltaTime);
            }
        }
    }

    void OnValidate()
    {
        UpdateMaterial();
    }

    private void UpdateMaterial()
    {
        if (object_renderer != null)
        {
            if (is_rotate)
            {
                object_renderer.material = active_material;
            }
            else
            {
                object_renderer.material = inactive_material;
            }
        }
    }
}
