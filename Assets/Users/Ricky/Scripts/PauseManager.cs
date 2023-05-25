using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    
    public bool pause_flg {get; private set;}
    
    private MENU_OPTION selected_option;
    
    private PauseButtonMove[] pause_menu;
    
    private RectTransform curtain_transform;
    
    private bool switch_scene = false;
    
    private SoundManager soundManager;
    
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
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameOverManager.instance.game_over_state)
        {
            if (pause_flg)
            {
                if (switch_scene)
                {
                    if (curtain_transform.localPosition != Vector3.zero)
                    {
                        curtain_transform.localPosition = Vector3.MoveTowards(curtain_transform.localPosition, Vector3.zero, Time.unscaledDeltaTime * 1000.0f);
                    }
                    else
                    {
                        // Switch scene
                        switch (selected_option)
                        {
                            case MENU_OPTION.STAGESELECT:
                            SceneManager.LoadScene("StageSelect");
                            switch_scene = false;
                            pause_flg = false;
                            Time.timeScale = 1.0f;
                            break;
                            case MENU_OPTION.TITLE:
                            SceneManager.LoadScene("Title");
                            switch_scene = false;
                            pause_flg = false;
                            Time.timeScale = 1.0f;
                            break;
                        }
                    }
                }
                else
                {
                    Time.timeScale = 0.0f;
                    
                    foreach (Transform child in transform)
                    {
                        child.gameObject.SetActive(true);
                    }
                    
                    if (InputManager.instance.press_start || InputManager.instance.press_cancel)
                    {
                        pause_flg = false;
                        soundManager.PlaySoundEffect("Cancel");
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
                            break;
                            case MENU_OPTION.RETRY:
                            Time.timeScale = 1.0f;
                            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                            pause_flg = false;
                            break;
                            case MENU_OPTION.OPTION:
                            
                            break;
                            case MENU_OPTION.STAGESELECT:
                            switch_scene = true;
                            break;
                            case MENU_OPTION.TITLE:
                            switch_scene = true;
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
                    pause_flg = true;
                }
                
                if (!HitstopManager.instance.is_stopped)
                {
                    Time.timeScale = 1.0f;
                }
            }
        }
    }
    
    MENU_OPTION GetNextMenu(int iinput)
    {
        MENU_OPTION new_menu = selected_option;
        
        if (iinput > 0)
        {
            
            
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
        
        return new_menu;
    }
}
