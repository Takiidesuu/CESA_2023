using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Spine;

public class PauseManager : MonoBehaviour
{
    enum MENU_OPTION
    {
        RESUME,
        RETRY,
        OPTION,
        STAGESELECT,
        TITLE,
        
        MAX
    }
    
    public static PauseManager instance;
    
    public GameObject Optionwindow;
    public GameObject black_panel;
    public GameObject white_panel;
    public GameObject star_panel;

    public bool pause_flg {get; private set;}
    
    private MENU_OPTION selected_option;
    
    private PauseButtonMove[] pause_menu;
    
    private RectTransform curtain_transform;
    
    public bool switch_scene {get; private set;}
    
    private SoundManager soundManager;
    
    private float store_bgm_volume;
    
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
        pause_flg = false;
        
        selected_option = MENU_OPTION.RESUME;
        
        pause_menu = new PauseButtonMove[5];
        
        for (int i = 0; i < (int)MENU_OPTION.MAX; i++)
        {
            pause_menu[i] = transform.GetChild(0).GetChild(i + 1).gameObject.GetComponent<PauseButtonMove>();
        }
        
        curtain_transform = transform.GetChild(1).GetComponent<RectTransform>();
        
        soundManager = GetComponent<SoundManager>();
        GetComponent<AudioSource>().ignoreListenerPause = true;
        
        white_panel.SetActive(false);
        black_panel.SetActive(false);

        switch_scene = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameOverManager.instance.game_over_state)
        {
            if (pause_flg)
            {
                if (!switch_scene)
                {
                    white_panel.SetActive(false);
                    black_panel.SetActive(false);
                    
                    int j = 0;
                    foreach (Transform child in transform)
                    {
                        if (j > 0) break;
                        child.gameObject.SetActive(true);
                        j++;
                    }

                    if (InputManager.instance.press_start || InputManager.instance.press_cancel)
                    {
                        if (Optionwindow.activeSelf)
                        {
                            Optionwindow.SetActive(false);
                            soundManager.PlaySoundEffect("Cancel");
                        }
                        else {
                            pause_flg = false;
                            if (GameObject.FindObjectOfType<AudioManager>())
                            {
                                GameObject.FindObjectOfType<AudioManager>().volume_to_use = store_bgm_volume;
                            }
                            soundManager.PlaySoundEffect("Cancel");
                            Time.timeScale = 1.0f;
                        }
                    }
                    
                    selected_option = GetNextMenu(InputManager.instance.GetMenuMoveFloat());
                    
                    for (int i = 0; i < (int)MENU_OPTION.MAX; i++)
                    {
                        if (i == (int)selected_option)
                        {
                            pause_menu[(int)selected_option].SetSelectedState(true, (int)selected_option);
                        }
                        else
                        {
                            pause_menu[i].SetSelectedState(false, (int)selected_option);
                        }
                    }
                    
                    if (InputManager.instance.press_select)
                    {
                        soundManager.PlaySoundEffect("OK");
                        
                        switch (selected_option)
                        {
                            case MENU_OPTION.RESUME:
                            pause_flg = false;
                            Time.timeScale = 1.0f;
                            if (GameObject.FindObjectOfType<AudioManager>())
                            {
                                GameObject.FindObjectOfType<AudioManager>().volume_to_use = store_bgm_volume;
                            }
                            break;
                            case MENU_OPTION.RETRY:
                            Time.timeScale = 1.0f;
                            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                            pause_flg = false;
                            if (GameObject.FindObjectOfType<AudioManager>())
                            {
                                GameObject.FindObjectOfType<AudioManager>().volume_to_use = store_bgm_volume;
                            }
                            break;
                            case MENU_OPTION.OPTION:
                                Optionwindow.SetActive(true);
                            break;
                            case MENU_OPTION.STAGESELECT:
                            case MENU_OPTION.TITLE:
                            switch_scene = true;
                            StartCoroutine(SwitchScene());
                            break;
                        }
                    }
                }
            }
            else
            {   
                foreach (Transform child in transform)
                {
                    child.gameObject.SetActive(false);
                }
                
                if (InputManager.instance.press_pause)
                {
                    soundManager.PlaySoundEffect("Pause");
                    if (GameObject.FindObjectOfType<AudioManager>())
                    {
                        store_bgm_volume = GameObject.FindObjectOfType<AudioManager>().volume_to_use;
                        GameObject.FindObjectOfType<AudioManager>().volume_to_use = store_bgm_volume / 4;
                    }
                    
                    pause_flg = true;
                    Time.timeScale = 0.0f;
                }
            }
        }
    }
    
    MENU_OPTION GetNextMenu(int iinput)
    {
        MENU_OPTION new_menu = selected_option;

        if (!Optionwindow.activeSelf)
        {
            if (iinput > 0)
            {
                soundManager.PlaySoundEffect("Cursor");

                switch (selected_option)
                {
                    case MENU_OPTION.RESUME:
                        new_menu = MENU_OPTION.RETRY;
                        break;
                    case MENU_OPTION.RETRY:
                        new_menu = MENU_OPTION.OPTION;
                        break;
                    case MENU_OPTION.OPTION:
                        new_menu = MENU_OPTION.STAGESELECT;
                        break;
                    case MENU_OPTION.STAGESELECT:
                        new_menu = MENU_OPTION.TITLE;
                        break;
                    case MENU_OPTION.TITLE:
                        new_menu = MENU_OPTION.RESUME;
                        break;
                }
            }
            else if (iinput < 0)
            {
                soundManager.PlaySoundEffect("Cursor");

                switch (selected_option)
                {
                    case MENU_OPTION.RESUME:
                        new_menu = MENU_OPTION.TITLE;
                        break;
                    case MENU_OPTION.RETRY:
                        new_menu = MENU_OPTION.RESUME;
                        break;
                    case MENU_OPTION.OPTION:
                        new_menu = MENU_OPTION.RETRY;
                        break;
                    case MENU_OPTION.STAGESELECT:
                        new_menu = MENU_OPTION.OPTION;
                        break;
                    case MENU_OPTION.TITLE:
                        new_menu = MENU_OPTION.STAGESELECT;
                        break;
                }
            }
            else
            {
                new_menu = selected_option;
            }
        }
        
        return new_menu;
    }
    
    IEnumerator SwitchScene()
    {
        white_panel.SetActive(true);
        
        yield return new WaitForSecondsRealtime(0.1f);
        
        white_panel.SetActive(false);
        black_panel.SetActive(true);
        star_panel.SetActive(true);
        
        Spine.Unity.SkeletonGraphic skeleton = star_panel.GetComponent<Spine.Unity.SkeletonGraphic>();
        
        string startString = skeleton.startingAnimation;
        
        skeleton.startingAnimation = "animation";
        
        yield return new WaitForSecondsRealtime(2.0f);
        
        skeleton.startingAnimation = startString;
        
        // Switch scene
        switch (selected_option)
        {
            case MENU_OPTION.STAGESELECT:
            SceneManager.LoadScene("StageSelect");
            break;
            case MENU_OPTION.TITLE:
            StageDataManager.instance.now_world = -1;
            StageDataManager.instance.now_stage = -1;
            SceneManager.LoadScene("Title");
            break;
        }
        
        switch_scene = false;
        pause_flg = false;
        Time.timeScale = 1.0f;
        if (GameObject.FindObjectOfType<AudioManager>())
        {
            GameObject.FindObjectOfType<AudioManager>().volume_to_use = store_bgm_volume;
        }
        
        skeleton.startingAnimation = "animation";
        white_panel.SetActive(false);
        black_panel.SetActive(false);
        star_panel.SetActive(false);
    }
}
