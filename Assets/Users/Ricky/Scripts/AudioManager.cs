using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    enum VOLUMESTATE
    {
        KEEP,
        FADEOUT,
        FADEIN
    }
    
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
    
    [SerializeField] private AudioClip clear_bgm;
    [SerializeField] [Range(0.0f, 1.0f)] private float clear_bgm_volume = 1.0f;
    
    [Tooltip("フェードイン速度")]
    [SerializeField] private float fadein_speed = 2.0f;
    [Tooltip("フェードアウト速度")]
    [SerializeField] private float fadeout_speed = 2.0f;
    [Tooltip("次のBGMまでの待ち時間")]
    [SerializeField] private float bgm_wait_time = 1.0f;
    
    private AudioSource audio_source;
    
    public float volume_to_use {get; set;}
    
    private int world_record;
    private string previous_scene_name;
    
    private string current_scene;
    
    private bool scene_has_changed;
    private bool is_game;
    
    private float volume_t;
    private VOLUMESTATE volume_state = VOLUMESTATE.FADEIN;
    private float switch_bgm_delay;
    
    AudioListener listener;
    
    private void Awake() 
    {
        if (instance != null && instance != this) 
        { 
            Destroy(this.gameObject); 
        } 
        else 
        { 
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        audio_source = GetComponent<AudioSource>();
        audio_source.loop = true;
        audio_source.ignoreListenerPause = true;
        
        previous_scene_name = "FirstScene";
        
        listener = GetComponent<AudioListener>();
        
        scene_has_changed = true;
        is_game = false;
        
        world_record = -1;
        
        volume_t = 0;
        switch_bgm_delay = 1.5f;
    }
    
    private void Update() 
    {
        current_scene = SceneManager.GetActiveScene().name;
        
        if (current_scene.Contains("Stage") && !current_scene.Contains("Select"))
        {   
            if (world_record != StageDataManager.instance.now_world + 1)
            {   
                scene_has_changed = true;
                is_game = true;
            }
        }
        else 
        {
            if (previous_scene_name != current_scene)
            {
                scene_has_changed = true;
                is_game = false;
            }
        }
        
        if (scene_has_changed)
        {
            AudioListener[] list_of_listeners = GameObject.FindObjectsOfType<AudioListener>();
            foreach (var lis in list_of_listeners)
            {
                if (is_game)
                {
                    if (lis.gameObject.tag == "Player" || lis == listener)
                    {
                        continue;
                    }
                }
                else
                {
                    if (lis == listener)
                    {
                        continue;
                    }
                }
                Destroy(lis);
            }
                
            if (is_game)
            {
                listener.enabled = false;
                if (GameObject.FindObjectOfType<PlayerMove>().gameObject.GetComponent<AudioListener>() == null)
                {
                    GameObject.FindObjectOfType<PlayerMove>().gameObject.AddComponent<AudioListener>();
                }
            }
            else
            {
                listener.enabled = true;
            }
            
            volume_state = VOLUMESTATE.FADEOUT;
            scene_has_changed = false;
            previous_scene_name = current_scene;
        }
        
        if (is_game)
        {
            if (!GameObject.FindObjectOfType<PauseManager>().pause_flg)
            {
                if (GameObject.FindObjectOfType<LightBulbCollector>().IsCleared() && audio_source.clip != clear_bgm)
                {
                    volume_to_use = 0;
                }
                
                if (audio_source.volume <= 0)
                {
                    audio_source.clip = clear_bgm;
                    volume_to_use = clear_bgm_volume;
                }
                
                EaseVolume();
                AudioListener.pause = false;
            }
            else
            {
                AudioListener.pause = true;
                audio_source.volume = volume_to_use / 2;
            }
        }
        else
        {
            if (current_scene == "StageSelect")
            {
                if (GameObject.FindObjectOfType<WorldSelect>().isTransitioning)
                {
                    volume_t -= Time.deltaTime;
                }
            }
            
            EaseVolume();
        }
        
        if (!audio_source.isPlaying)
        {
            audio_source.Play();
        }
    }
    
    void SwitchBGM(bool in_game)
    {
        if (in_game)
        {
            world_record = StageDataManager.instance.now_world + 1;
        
            switch (world_record)
            {
                case 1:
                audio_source.clip = world1_bgm;
                volume_to_use = world1_bgm_volume;
                break;
                case 2:
                audio_source.clip = world2_bgm;
                volume_to_use = world2_bgm_volume;
                break;
                case 3:
                audio_source.clip = world3_bgm;
                volume_to_use = world3_bgm_volume;
                break;
                case 4:
                audio_source.clip = world4_bgm;
                volume_to_use = world4_bgm_volume;
                break;
                default:
                audio_source.clip = world1_bgm;
                volume_to_use = world1_bgm_volume;
                break;
            }
        }
        else
        {
            switch (current_scene)
            {
                case "Title":
                audio_source.clip = title_bgm;
                volume_to_use = title_bgm_volume;
                break;
                case "StageSelect":
                audio_source.clip = stage_select_bgm;
                volume_to_use = stage_select_bgm_volume;
                break;
            }
        }
        
        if (audio_source.clip == null)
        {
            Debug.LogError("BGM is not set");
        }
        
        audio_source.Stop();
    }
    
    void SwitchBGM(string type)
    {
        switch (type)
        {
            case "Clear":
            audio_source.clip = clear_bgm;
            volume_to_use = clear_bgm_volume;
            break;
            case "GameOver":
            break;
            default:
            break;
        }
    }
    
    void EaseVolume()
    {
        audio_source.volume = Mathf.Lerp(0, volume_to_use, volume_t);
        
        if (volume_t <= 0 && volume_state == VOLUMESTATE.FADEOUT)
        {
            if (switch_bgm_delay < bgm_wait_time)
            {
                switch_bgm_delay += Time.deltaTime;
            }
            else
            {
                volume_state = VOLUMESTATE.FADEIN;
                SwitchBGM(is_game);
            }
        }
        
        if (volume_t >= 1 && volume_state == VOLUMESTATE.FADEIN)
        {
            switch_bgm_delay = 0;
            volume_state = VOLUMESTATE.KEEP;
        }
        
        volume_t = Mathf.Clamp(volume_t, 0, 1);
        
        switch (volume_state)
        {
            case VOLUMESTATE.KEEP:
            break;
            case VOLUMESTATE.FADEOUT:
            volume_t -= Time.deltaTime / 2.0f * fadeout_speed;
            break;
            case VOLUMESTATE.FADEIN:
            volume_t += Time.deltaTime / 2.0f * fadein_speed;
            break;
        }
    }
}
