using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float rotate_speed = 10f;
    public GameObject[] rotate_objects;  //回転対象オブジェクト

    private GameObject modeldata;    //モデルデータ
    public bool is_rotate = true;   //1 = 回転
    public Material active_material; //起動時マテリアル
    public Material inactive_material;  //非稼働時マテリアル
    private Renderer object_renderer;
    Rotator_switch trigger_switch;    //スイッチオブジェクト

    void Start()
    {
        trigger_switch = transform.Find("Switch").gameObject.GetComponent<Rotator_switch>();
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
        is_rotate = trigger_switch.is_stage_hit;
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
