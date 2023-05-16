using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageScript : MonoBehaviour
{
    [Tooltip("無敵時間")]
    [SerializeField] private float invincible_duration = 2.0f;
    [Tooltip("ダメージ受けた時のヒットストップ時間")]
    [SerializeField] private float damage_stop_time = 0.1f;
    
    [Tooltip("体力")]
    [SerializeField] private int hp = 3;
    
    private float invincible_flicker_time = 0.1f;
    
    Renderer renderer_component;
    
    private bool is_invincible = false;
    
    private LightBulbCollector check_is_cleared;
    
    private ElectricShockDamage hiteffect;
    
    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<Renderer>() != null)
        {
            renderer_component = GetComponent<Renderer>();
        }
        
        check_is_cleared = GameObject.FindObjectOfType<LightBulbCollector>();
        
        hiteffect = GetComponent<ElectricShockDamage>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnTriggerEnter(Collider other) 
    {
        if (!check_is_cleared.IsCleared())
        {
            if (other.gameObject.tag == "ElectricalBall")
            {
                if (!is_invincible)
                {
                    StartCoroutine("InvincibleStatus");
                    hp--;
                    
                    if (hp <= 0)
                    {
                        if (this.gameObject.tag == "Player")
                        {
                            GameObject.FindObjectOfType<GameOverManager>().SwitchToGameOver();
                        }
                    }
                    else
                    {
                        if (this.gameObject.tag == "Player")
                        {
                            this.GetComponent<PlayerMove>().TookDamage();
                            HitstopManager.instance.StartHitStop(damage_stop_time);
                            
                            hiteffect.is_damage = true;
                            hiteffect.UpdateMaterial();
                        }
                    }
                    
                    InvokeRepeating("InvincibleFlicker", 0.0f, invincible_flicker_time);
                    is_invincible = true;
                }
            }
        }
    }
    
    public int GetHitPoint()
    {
        return hp;
    }
    
    IEnumerator InvincibleStatus()
    {
        yield return new WaitForSeconds(invincible_duration);
        
        CancelInvoke();
        
        is_invincible = false;
        
        if (renderer_component != null)
        {
            renderer_component.enabled = true;
        }
        
        hiteffect.is_damage = false;
        hiteffect.UpdateMaterial();
        
        if (this.gameObject.tag == "Player" || this.gameObject.tag == "PlayerNPC")
        {
            foreach (Transform child in this.transform.GetChild(0))
            {
                child.gameObject.SetActive(true);
            }
        }
    }
    
    private void InvincibleFlicker()
    {
        if (renderer_component != null)
        {
            renderer_component.enabled = !renderer_component.enabled;
        }
        
        if (this.gameObject.tag == "Player" || this.gameObject.tag == "PlayerNPC")
        {
            foreach (Transform child in this.transform.GetChild(0))
            {
                child.gameObject.SetActive(!child.gameObject.activeSelf);
            }
        }
    }
}