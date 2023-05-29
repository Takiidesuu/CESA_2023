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
    
    enum ANIMSTATE
    {
        PLAYERWALK,
        LIGHT,
        LOGO,
        RETRY,
        SELECT,
        
        MAX
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
    [Tooltip("プレイヤーテクスチャ")]
    [SerializeField] private GameObject player_obj_tex;
    private RectTransform player_obj_tex_rect;
    [Tooltip("プレイヤー影テクスチャ")]
    [SerializeField] private GameObject player_shadow_tex;
    private RectTransform player_shadow_tex_rect;
    
    private float player_move_t;
    private float light_ray_t;
    private float fail_pic_t;
    private float retry_t;
    private float select_t;
    
    private SoundManager soundManager;
    
    private ANIMSTATE anim_state = ANIMSTATE.PLAYERWALK;
    
    public bool game_over_state {get; private set;}
    
    public bool test_state;
    
    private float button_x_target;
    
    private bool move_button;
    private bool accept_button_input;
    
    private bool play_jingle;
    
    private MENU current_menu = MENU.RETRY;
    
    public void SwitchToGameOver()
    {
        game_over_state = true;
        black_panel.SetActive(true);
        
        AudioSource[] game_audios = GameObject.FindObjectsOfType<AudioSource>();
        foreach (var audio in game_audios)
        {
            if (audio != this.GetComponent<AudioSource>())
            {
                audio.Stop();
            }
        }
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
        player_obj_tex_rect = player_obj_tex.GetComponent<RectTransform>();
        player_shadow_tex_rect = player_shadow_tex.GetComponent<RectTransform>();
        
        soundManager = GetComponent<SoundManager>();
        
        hammer_pic.SetActive(false);
        
        player_anim = player_obj.transform.GetChild(0).GetComponent<Animator>();
        
        game_over_state = false;
        
        button_x_target = -430.0f;
        
        move_button = true;
        accept_button_input = false;
        
        play_jingle = true;
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
                    switch (anim_state)
                    {
                        case ANIMSTATE.PLAYERWALK:
                        player_move_t = CountT(player_move_t, 3, ANIMSTATE.LIGHT);
                        player_anim.SetBool("isWalking", true);
                        player_anim.speed = 0.7f;
                        break;
                        
                        case ANIMSTATE.LIGHT:
                        light_ray_t = CountT(light_ray_t, 0.5f, ANIMSTATE.LOGO);
                        player_anim.speed = 1;
                        player_anim.SetTrigger("failAnim");
                        player_obj.transform.localEulerAngles = new Vector3(0, 90, 0);
                        if (play_jingle)
                        {
                            soundManager.PlaySoundEffect("GameOverJingle");
                            play_jingle = false;
                        }
                        break;
                        
                        case ANIMSTATE.LOGO:
                        fail_pic_t = CountT(fail_pic_t, 0.5f, ANIMSTATE.RETRY);
                        ChangeAlphaState(failed_pic);
                        break;
                        case ANIMSTATE.RETRY:
                        retry_t = CountT(retry_t, 0.5f, ANIMSTATE.SELECT);
                        ChangeAlphaState(retry_button);
                        ChangeAlphaState(retry_button.transform.GetChild(0).gameObject);
                        break;
                        
                        case ANIMSTATE.SELECT:
                        select_t = CountT(select_t, 0.5f, ANIMSTATE.MAX);
                        ChangeAlphaState(select_button);
                        ChangeAlphaState(select_button.transform.GetChild(0).gameObject);
                        break;
                        
                        case ANIMSTATE.MAX:
                        accept_button_input = true;
                        hammer_pic.SetActive(true);
                        break;
                    }
    
                    player_obj_tex_rect.localPosition = Vector3.Lerp(new Vector3(1225, player_obj_tex_rect.localPosition.y, player_obj_tex_rect.localPosition.z), new Vector3(510, player_obj_tex_rect.localPosition.y, player_obj_tex_rect.localPosition.z), player_move_t);
                    player_shadow_tex_rect.localPosition = player_obj_tex_rect.localPosition + new Vector3(45, -109, 365);
                    
                    light_obj_rect.localScale = Vector3.Lerp(new Vector3(0, 16, 1), new Vector3(7, 16, 1), light_ray_t);
                    
                    failed_pic_rect.localScale = Vector3.Lerp(new Vector3(1.8f, 1.6f, 1.3f), new Vector3(1.5f, 1.3f, 1), fail_pic_t);
                    
                    retry_button_rect.localPosition = Vector3.Lerp(new Vector3(-750, retry_button_rect.localPosition.y, 0), new Vector3(button_x_target, retry_button_rect.localPosition.y, 0), retry_t);
                    select_button_rect.localPosition = Vector3.Lerp(new Vector3(-750, select_button_rect.localPosition.y, 0), new Vector3(button_x_target, select_button_rect.localPosition.y, 0), select_t);
                }
            }
            else
            {
                if (InputManager.instance.GetMenuMoveFloat() < 0)
                {
                    current_menu = MENU.RETRY;
                    soundManager.PlaySoundEffect("Cursor");
                }
                else if (InputManager.instance.GetMenuMoveFloat() > 0)
                {
                    current_menu = MENU.SELECT;
                    soundManager.PlaySoundEffect("Cursor");
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
            
            if (InputManager.instance.press_select && anim_state == ANIMSTATE.MAX)
            {
                soundManager.PlaySoundEffect("OK");
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
    
    private bool MoveBlackPanel()
    {
        bool result = false;
        
        if (black_panel_rect.localPosition != Vector3.zero)
        {
            black_panel_rect.localPosition = Vector3.MoveTowards(black_panel_rect.localPosition, Vector3.zero, 200.0f);
        }
        else
        {
            result = true;
            failed_pic.SetActive(true);
            player_obj.SetActive(true);
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
    
    private float CountT(float t, float slowSpeed, ANIMSTATE nextState)
    {
        if (InputManager.instance.press_select)
        {
            t = 1;
        }
        
        if (t < 1)
        {
            t += Time.unscaledDeltaTime / slowSpeed;
        }
        else
        {
            t = 1;
            anim_state = nextState;
        }
        
        return t;
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
