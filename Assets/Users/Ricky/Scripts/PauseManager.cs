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
            pause_menu[i] = transform.GetChild(1).GetChild(i + 1).gameObject.GetComponent<PauseButtonMove>();
        }
        
        curtain_transform = transform.GetChild(0).GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (pause_flg)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
            
            if (InputManager.instance.press_start || InputManager.instance.press_cancel)
            {
                pause_flg = false;
            }
            
            selected_option += InputManager.instance.menu_move_input;
            selected_option = (MENU_OPTION)Mathf.Clamp((int)selected_option, (int)MENU_OPTION.RESUME, (int)MENU_OPTION.TITLE);
            
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
                switch (selected_option)
                {
                    case MENU_OPTION.RESUME:
                    pause_flg = false;
                    break;
                    case MENU_OPTION.RETRY:
                    
                    break;
                    case MENU_OPTION.OPTION:
                    
                    break;
                    case MENU_OPTION.STAGESELECT:
                    break;
                    case MENU_OPTION.TITLE:
                    break;
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
        }
    }
}
