using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageScript : MonoBehaviour
{
    [Tooltip("関電される時間")]
    [SerializeField] private float invincible_duration = 2.0f;
    [Tooltip("体力")]
    [SerializeField] private int hp = 3;
    
    private float invincible_flicker_time = 0.1f;
    
    Renderer renderer_component;
    
    private bool is_invincible = false;
    
    private LightBulbCollector check_is_cleared;
    
    private ElectricShockDamage hiteffect;
    
    private float prev_angle;
    
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
                            this.GetComponent<PlayerMove>().TookDamage(invincible_duration * 1.2f);
                            
                            prev_angle = this.transform.eulerAngles.y;
                            this.transform.eulerAngles = new Vector3(this.transform.eulerAngles.x, 90.0f, this.transform.eulerAngles.z);
                            
                            hiteffect.is_damage = true;
                            hiteffect.UpdateMaterial();
                        }
                    }
                    
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
        
        is_invincible = false;
        
        if (renderer_component != null)
        {
            renderer_component.enabled = true;
        }
        
        hiteffect.is_damage = false;
        hiteffect.UpdateMaterial();
        
        this.transform.eulerAngles = new Vector3(this.transform.eulerAngles.x, prev_angle, this.transform.eulerAngles.z);
    }
}