using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBulb : MonoBehaviour
{
    public bool is_stage_hit = false;   //ステージに当たっているか
    private int nothit_count;           //当たってらずに何フレーム立ったか (Exitが呼ばれないため)

    private LightBulbChangeMaterial changeMaterial;    //マテリアルを変更
    private SoundManager soundManager;
    public GameObject Laser;
    public GameObject HitEffect;    //接触時に発光する
    [Tooltip("消滅時間")]
    [SerializeField] private float m_destroy_time = 5.0f;
    private float m_destroy_timer;
    
    [Tooltip("クリア判定の子オブジェクト")]
    [SerializeField] private GameObject clear_trigger_obj;
    
    private GameObject blur_object;
    
    private GameObject touching_electric;
    private SphereCollider col;
    
    private Vector3 default_scale = new Vector3(3, 3, 3);
    private GameObject electric_effect;
    
    public GameObject line_status_obj {get;set;}

    public float GetDestroyTime()
    {
        return m_destroy_time;
    }
    
    public float GetCurrentTimer()
    {
        return m_destroy_timer;
    }

    private void Start()
    {   
        changeMaterial = GetComponent<LightBulbChangeMaterial>();
        soundManager = GetComponent<SoundManager>();    
        electric_effect = transform.GetChild(0).gameObject;
        electric_effect.transform.localScale = default_scale;
        
        col = GetComponent<SphereCollider>();
        
        blur_object = Camera.main.transform.GetChild(2).gameObject;
        
        line_status_obj = null;
        clear_trigger_obj.SetActive(false);
    }

    private void Update()
    {
        if (!GameObject.FindObjectOfType<LightBulbCollector>().IsCleared())
        {
            if(is_stage_hit)
            {
                changeMaterial.OnPower = true;
                m_destroy_timer += Time.deltaTime;
            }
            else
            {
                changeMaterial.OnPower = false;
            }
            
            float scale_to_minus = default_scale.x * (m_destroy_timer / m_destroy_time);
            electric_effect.transform.localScale = new Vector3(default_scale.x - scale_to_minus, default_scale.y - scale_to_minus, default_scale.z - scale_to_minus);

            //時間経過後削除
            if (m_destroy_timer > m_destroy_time)
            {
                is_stage_hit = false;
                m_destroy_timer = 0;
            }
        }
        else
        {
            changeMaterial.OnPower = true;
            m_destroy_timer = 0.0f;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("ElectricalBall"))
        {
            if (!clear_trigger_obj.activeSelf)
            {
                is_stage_hit = true;
                nothit_count = 0;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("ElectricalBall"))
        {
            touching_electric = other.gameObject;
            
            if (GameObject.FindObjectOfType<LightBulbCollector>().lightbulb_left == 1 && changeMaterial.OnPower == false)
            {
                //blur_object.GetComponent<Renderer>().enabled = true;
                clear_trigger_obj.SetActive(true);
            }
            else
            {
                LightUpBulb();
            }
        }
    }
    
    private void OnTriggerExit(Collider other) 
    {
        if (other.gameObject.CompareTag("ElectricalBall"))
        {
            if (GameObject.FindObjectOfType<LightBulbCollector>().lightbulb_left == 1 && changeMaterial.OnPower == false)
            {
                clear_trigger_obj.SetActive(false);
                //blur_object.GetComponent<Renderer>().enabled = false;
                Time.timeScale = 1;
            }
        }
    }
    
    public void LightUpBulb()
    {
        soundManager.PlaySoundEffect("Hit");
        m_destroy_timer = 0;
        is_stage_hit = true;
        Quaternion LaserRotation = CalculateRotation(gameObject.transform.position, touching_electric.gameObject.GetComponent<ElectricBallMove>().ParentGenerator.gameObject.transform.position);
        Instantiate(Laser, gameObject.transform.position,LaserRotation);
        Instantiate(HitEffect, gameObject.transform.position,transform.rotation);

        if (line_status_obj == null)
        {
            line_status_obj = GameObject.FindObjectOfType<BulbStatusScript>().AddStatus(this);
        }
        else
        {
            GameObject.FindObjectOfType<BulbStatusScript>().ResetStatus(this);
        }
    }
    
    public Quaternion CalculateRotation(Vector3 startPos, Vector3 endPos)
    {
        Vector3 direction = endPos - startPos;
        Quaternion rotation = Quaternion.LookRotation(direction);

        return rotation;
    }
}
