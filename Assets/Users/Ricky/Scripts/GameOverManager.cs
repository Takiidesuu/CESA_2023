using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{   
    public static GameOverManager instance {get; private set;}
    
    enum MENU
    {
        RETRY,
        SELECT
    }
    
    [Header("画像オブジェクト")]
    [Tooltip("黒いパネル")]
    [SerializeField] private GameObject black_panel;
    [Tooltip("治療失敗")]
    [SerializeField] private GameObject failed_pic;
    [Tooltip("リトライ")]
    [SerializeField] private GameObject retry_button;
    [Tooltip("セレクトに戻る")]
    [SerializeField] private GameObject select_button;
    [Tooltip("日差し？")]
    [SerializeField] private GameObject light_obj;
    
    private RectTransform black_panel_rect;
    private float elapsed_time;
    
    public bool game_over_state {get; private set;}
    
    public bool test_state;
    
    private float button_x_target;
    
    private bool move_button;
    private bool accept_button_input;
    
    private MENU current_menu = MENU.RETRY;
    
    public void SwitchToGameOver()
    {
        game_over_state = true;
        black_panel.SetActive(true);
    }
    
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
        black_panel_rect = black_panel.GetComponent<RectTransform>();
        game_over_state = false;
        
        elapsed_time = -0.5f;
        
        button_x_target = -430.0f;
        
        move_button = true;
        accept_button_input = false;
        
        light_obj.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (test_state)
        {
            SwitchToGameOver();
        }
        
        if (game_over_state)
        {
            if (accept_button_input)
            {                
                if (MoveBlackPanel())
                {
                    if (elapsed_time < 4.0f)
                    {
                        elapsed_time += Time.unscaledDeltaTime;
                    }
                    
                    RectTransform failed_rect = failed_pic.GetComponent<RectTransform>();
                    
                    if (failed_rect.localScale != Vector3.one)
                    {
                        failed_rect.localScale = Vector3.MoveTowards(failed_rect.localScale, new Vector3(1.5f, 1.3f, 1), Time.unscaledDeltaTime);
                        
                        ChangeAlphaState(failed_pic);
                    }
                    else
                    {
                        //failed_rect.pivot = new Vector2(0.4f, 1.0f);
                        //failed_rect.localEulerAngles = Vector3.MoveTowards(failed_rect.localEulerAngles, new Vector3(failed_rect.localEulerAngles.x, failed_rect.localEulerAngles.y, -5.0f), Time.unscaledDeltaTime * 3.0f);
                    }
                    
                    if (elapsed_time > 1.0f)
                    {
                        if(move_button)
                        {
                            if (select_button.GetComponent<RectTransform>().localPosition != new Vector3(button_x_target, select_button.GetComponent<RectTransform>().localPosition.y, 0.0f))
                            {
                                if (retry_button.GetComponent<RectTransform>().localPosition != new Vector3(button_x_target, retry_button.GetComponent<RectTransform>().localPosition.y, 0.0f))
                                {
                                    StartCoroutine(ChangeButtonAlphaState(retry_button));
                                }
                                else
                                {
                                    StartCoroutine(ChangeButtonAlphaState(select_button));
                                }
                            }
                            else
                            {
                                accept_button_input = true;
                            }
                            
                            move_button = false;
                        }
                    }
                }
            }
            else
            {
                if (InputManager.instance.GetMenuMoveFloat() < 0)
                {
                    current_menu = MENU.RETRY;
                }
                else if (InputManager.instance.GetMenuMoveFloat() > 0)
                {
                    current_menu = MENU.SELECT;
                }
            }
            
            if (InputManager.instance.press_select)
            {
                if (!accept_button_input)
                {
                    if (elapsed_time < 0.0f)
                    {
                        elapsed_time = 0.0f;
                    }
                    else if (elapsed_time >= 0.0f && elapsed_time < 3.0f)
                    {
                        elapsed_time = 1.0f;
                    }
                }
                else
                {
                    switch (current_menu)
                    {
                        case MENU.RETRY:
                        
                        break;
                        case MENU.SELECT:
                        
                        break;
                    }
                }
            }
        }
    }
    
    private bool MoveBlackPanel()
    {
        bool result = false;
        
        if (black_panel_rect.localPosition != Vector3.zero)
        {
            black_panel_rect.localPosition = Vector3.MoveTowards(black_panel_rect.localPosition, Vector3.zero, 20.0f);
        }
        else
        {
            result = true;
            failed_pic.SetActive(true);
        }
        
        return result;
    }
    
    private void ChangeAlphaState(GameObject obj_to_change)
    {
        var img = obj_to_change.GetComponent<Image>();
        var img_color = img.color;
        img_color.a = Mathf.MoveTowards(img_color.a, 255.0f, Time.unscaledDeltaTime * 10.0f);
        img.color = img_color;
    }
    
    IEnumerator ChangeButtonAlphaState(GameObject button_to_change)
    {
        float button_t = 0.0f;
        
        while (button_t <= 1.0f)
        {
            if (InputManager.instance.press_select)
            {
                button_t = 1.0f;
            }
            
            button_t += Time.unscaledDeltaTime * 2.0f;
            
            var img = button_to_change.GetComponent<Image>();
            var img_color = img.color;
            img_color.a = Mathf.Lerp(0.0f, 255.0f, button_t);
            img.color = img_color;
            
            var text_img = button_to_change.transform.GetChild(0).GetComponent<Image>();
            var text_img_color = text_img.color;
            text_img_color.a = Mathf.Lerp(0.0f, 255.0f, button_t);
            text_img.color = text_img_color;
            
            RectTransform button_rect = button_to_change.GetComponent<RectTransform>();
            button_rect.localPosition = Vector3.Lerp(new Vector3(-750.0f, button_rect.localPosition.y, 0.0f), new Vector3(button_x_target, button_rect.localPosition.y, 0.0f), button_t);
            yield return null;
        }
        
        move_button = true;
    }
}
