using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoosterScript : MonoBehaviour
{
    [Tooltip("加算される速度")]
    [SerializeField] private float speed_boost = 20.0f;

    SoundManager soundManager;

    // Start is called before the first frame update
    void Start()
    {
        soundManager = GetComponent<SoundManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.tag == "ElectricalBall")
        {
            soundManager.PlaySoundEffect("BoostSE");
            other.GetComponent<ElectricBallMove>().ChangeSpeed(speed_boost);
        }
    }
}
