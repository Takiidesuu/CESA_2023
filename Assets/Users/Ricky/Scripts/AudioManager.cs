using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    static public AudioManager instance;
    
    [Header("BGM")]
    [SerializeField] private AudioClip title_bgm;
    [SerializeField] [Range(0.0f, 1.0f)] private float title_bgm_volume = 1.0f;
    [SerializeField] private AudioClip stage_select_bgm;
    [SerializeField] [Range(0.0f, 1.0f)] private float stage_select_bgm_volume = 1.0f;
    [SerializeField] private AudioClip world1_bgm;
    [SerializeField] [Range(0.0f, 1.0f)] private float world1_bgm_volume = 1.0f;
    [SerializeField] private AudioClip world2_bgm;
    [SerializeField] [Range(0.0f, 1.0f)] private float world2_bgm_volume = 1.0f;
    [SerializeField] private AudioClip world3_bgm;
    [SerializeField] [Range(0.0f, 1.0f)] private float world3_bgm_volume = 1.0f;
    [SerializeField] private AudioClip world4_bgm;
    [SerializeField] [Range(0.0f, 1.0f)] private float world4_bgm_volume = 1.0f;
    
    private AudioSource audio_source;
    
    private int world_record;
    
    private void Awake() 
    {
        if (instance != null && instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            instance = this; 
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        audio_source = GetComponent<AudioSource>();
        audio_source.loop = true;
    }
    
    private void Update() 
    {
        if (world_record != StageDataManager.instance.now_world + 1)
        {
            SwitchBGM();
        }
    }
    
    void SwitchBGM()
    {
        world_record = StageDataManager.instance.now_world + 1;
        
        switch (world_record)
        {
            case 1:
            audio_source.clip = world1_bgm;
            audio_source.volume = world1_bgm_volume;
            break;
            case 2:
            audio_source.clip = world2_bgm;
            audio_source.volume = world2_bgm_volume;
            break;
            case 3:
            audio_source.clip = world3_bgm;
            audio_source.volume = world3_bgm_volume;
            break;
            case 4:
            audio_source.clip = world4_bgm;
            audio_source.volume = world4_bgm_volume;
            break;
        }
        
        audio_source.Play();
    }
}
