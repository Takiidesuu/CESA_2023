using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamSound : MonoBehaviour
{
    // Start is called before the first frame update
    ParticleSystem ps;
    SoundManager se;
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        se = GetComponent<SoundManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnParticleSystemStopped()
    {
        se.PlaySoundEffect("Steam");
        ps.Play();
    }
}
