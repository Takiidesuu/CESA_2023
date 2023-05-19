using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    private RectTransform black_panel_rect;
    [Tooltip("治療失敗")]
    [SerializeField] private GameObject failed_pic;
    private RectTransform failed_pic_rect;
    [Tooltip("ハンマー")]
    [SerializeField] private GameObject hammer_pic;
    private RectTransform hammer_pic_rect;
    [Tooltip("リトライ")]
    [SerializeField] private GameObject retry_button;
    private RectTransform retry_button_rect;
    [Tooltip("セレクトに戻る")]
    [SerializeField] private GameObject select_button;
    private RectTransform select_button_rect;
    [Tooltip("日差し？")]
    [SerializeField] private GameObject light_obj;
    private RectTransform light_obj_rect;
    [Tooltip("プレイヤー")]
    [SerializeField] private GameObject player_obj;
    private Animator player_anim;
    
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
        failed_pic_rect = failed_pic.GetComponent<RectTransform>();
        hammer_pic_rect = hammer_pic.GetComponent<RectTransform>();
        retry_button_rect = retry_button.GetComponent<RectTransform>();
        select_button_rect = select_button.GetComponent<RectTransform>();
        light_obj_rect = light_obj.GetComponent<RectTransform>();
        
        hammer_pic.SetActive(false);
        
        player_anim = player_obj.transform.GetChild(0).GetComponent<Animator>();
        
        game_over_state = false;
        
        elapsed_time = -3.0f;
        
        button_x_target = -430.0f;
        
        move_button = true;
        accept_button_input = false;
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
            if (!accept_button_input)
            {                
                if (MoveBlackPanel())
                {
                    if (elapsed_time < 4.0f)
                    {
                        elapsed_time += Time.unscaledDeltaTime;
                    }
                    
                    if (elapsed_time < -1.0f)
                    {
                        player_obj.transform.localPosition = Vector3.MoveTowards(player_obj.transform.localPosition, new Vector3(475, player_obj.transform.localPosition.y, player_obj.transform.localPosition.z), Time.unscaledDeltaTime * 330.0f);
                    }
                    
                    if (elapsed_time >= -1.0f && elapsed_time < 0.0f)
                    {
                        player_anim.SetTrigger("failAnim");
                        player_obj.transform.localEulerAngles = new Vector3(0, 90, 0);
                        light_obj_rect.localScale = Vector3.MoveTowards(light_obj_rect.localScale, new Vector3(7, 16, 1), Time.unscaledDeltaTime * 30.0f);
                    }
                    
                    if (elapsed_time >= 0.0f)
                    {
                        if (failed_pic_rect.localScale != Vector3.one)
                        {
                            failed_pic_rect.localScale = Vector3.MoveTowards(failed_pic_rect.localScale, new Vector3(1.5f, 1.3f, 1), Time.unscaledDeltaTime);
                            
                            ChangeAlphaState(failed_pic);
                        }
                        else
                        {
                            //failed_rect.pivot = new Vector2(0.4f, 1.0f);
                            //failed_rect.localEulerAngles = Vector3.MoveTowards(failed_rect.localEulerAngles, new Vector3(failed_rect.localEulerAngles.x, failed_rect.localEulerAngles.y, -5.0f), Time.unscaledDeltaTime * 3.0f);
                        }
                    }
                    
                    if (elapsed_time > 1.0f)
                    {
                        if(move_button)
                        {
                            if (select_button_rect.localPosition != new Vector3(button_x_target, select_button_rect.localPosition.y, 0.0f))
                            {
                                if (retry_button_rect.localPosition != new Vector3(button_x_target, retry_button_rect.localPosition.y, 0.0f))
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
                                hammer_pic.SetActive(true);
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

                switch (current_menu)
                {
                    case MENU.RETRY:
                    hammer_pic_rect.localPosition = new Vector3(-700.0f, retry_button_rect.localPosition.y, 1);
                    retry_button_rect.localPosition = new Vector3(button_x_target + 150.0f, retry_button_rect.localPosition.y, 1);
                    select_button_rect.localPosition = new Vector3(button_x_target, select_button_rect.localPosition.y, 1);
                    break;
                    case MENU.SELECT:
                    hammer_pic_rect.localPosition = new Vector3(-700.0f, select_button_rect.localPosition.y, 1);
                    retry_button_rect.localPosition = new Vector3(button_x_target, retry_button_rect.localPosition.y, 1);
                    select_button_rect.localPosition = new Vector3(button_x_target + 150.0f, select_button_rect.localPosition.y, 1);
                    break;
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
                        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                        break;
                        case MENU.SELECT:
                        SceneManager.LoadScene("StageSelect");
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
            player_obj.SetActive(true);
            player_anim.SetBool("isWalking", true);
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
