using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager instance {get; private set;}
    
    [Header("画像ファイル")]
    [Tooltip("黒いパネル")]
    [SerializeField] private GameObject black_panel;
    [Tooltip("治療失敗")]
    [SerializeField] private GameObject failed_pic;
    [Tooltip("戻る")]
    [SerializeField] private GameObject back_pic;
    
    private RectTransform black_panel_rect;
    private float elapsed_time;
    
    public bool game_over_state {get; private set;}
    
    public bool test_state;
    
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
            if (MoveBlackPanel())
            {
                if (elapsed_time < 4.0f)
                {
                    elapsed_time += Time.unscaledDeltaTime;
                }
                
                RectTransform failed_rect = failed_pic.GetComponent<RectTransform>();
                
                if (failed_rect.localScale != Vector3.one)
                {
                    failed_rect.localScale = Vector3.MoveTowards(failed_rect.localScale, Vector3.one, Time.unscaledDeltaTime);
                    
                    var failed_image = failed_pic.GetComponent<Image>();
                    var failed_img_color = failed_image.color;
                    failed_img_color.a = Mathf.MoveTowards(failed_img_color.a, 255.0f, Time.unscaledDeltaTime * 10.0f);
                    failed_image.color = failed_img_color;
                }
                else
                {
                    //failed_rect.pivot = new Vector2(0.4f, 1.0f);
                    //failed_rect.localEulerAngles = Vector3.MoveTowards(failed_rect.localEulerAngles, new Vector3(failed_rect.localEulerAngles.x, failed_rect.localEulerAngles.y, -5.0f), Time.unscaledDeltaTime * 3.0f);
                }
                
                if (elapsed_time > 3.0f)
                {
                    back_pic.SetActive(true);
                }
            }
            
            if (InputManager.instance.press_select)
            {
                if (elapsed_time < 0.0f)
                {
                    elapsed_time = 0.0f;
                }
                else if (elapsed_time >= 0.0f && elapsed_time < 3.0f)
                {
                    elapsed_time = 3.0f;
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
}
