using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    public static InputManager instance {get; private set;}
    MainInputControls input_system;
    
    private Gamepad gamepad;
    
    private string current_scene;
    private bool is_game_scene;
    
    public Vector2 player_move_float {get; private set;}
    private int menu_move_input;
    
    public bool press_smash {get; private set;}
    public bool press_flip {get; private set;}
    public bool press_rotate {get; private set;}
    public bool press_pause {get; private set;}
    public bool press_select {get; private set;}
    public bool press_cancel {get; private set;}
    public bool press_start {get; private set;}
    
    public bool press_menu_left {get; private set;}
    public bool press_menu_right {get; private set;}
    
    private float input_delay;
    
    private float vibrate_duration;
    private float vibrate_strength;
    
    private LightBulbCollector check_is_cleared;
    
    public int GetMenuMoveFloat()
    {
        int return_num = menu_move_input;
        
        if (input_delay > 0)
        {
            return_num = 0;
        }
        
        return return_num;
    }
    
    public void VibrateController(float fvibrate_duration, float fstrength)
    {
        vibrate_duration = fvibrate_duration;
        vibrate_strength = fstrength;
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
    
        input_system = new MainInputControls();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        current_scene = SceneManager.GetActiveScene().name;
        
        gamepad = Gamepad.current;
        
        if (current_scene.Contains("Stage") && !current_scene.Contains("Select"))
        {
            input_system.Player.Enable();
            input_system.Menu.Disable();
            
            check_is_cleared = GameObject.FindObjectOfType<LightBulbCollector>();
            
            is_game_scene = true;
        }
        else
        {
            input_system.Player.Disable();
            input_system.Menu.Enable();
            
            is_game_scene = false;
        }
        
        ResetAllParams();
        
        input_delay = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (is_game_scene)
        {
            if (PauseManager.instance.pause_flg || check_is_cleared.IsCleared() || GameOverManager.instance.game_over_state)
            {
                if (!input_system.Menu.enabled)
                {
                    input_system.Player.Disable();
                    input_system.Menu.Enable();
                }
                
                if (menu_move_input != 0 || press_menu_left || press_menu_right)
                {
                    if (input_delay < 0.6f)
                    {
                        input_delay += Time.unscaledDeltaTime;
                    }
                    else
                    {
                        input_delay = 0.0f;
                    }
                }
                else
                {
                    input_delay = 0.0f;
                }
                
                menu_move_input = (int)input_system.Menu.VerticalMove.ReadValue<float>();
            }
            else
            {
                if (!input_system.Player.enabled)
                {
                    input_system.Player.Enable();
                    input_system.Menu.Disable();
                }
                
                player_move_float = input_system.Player.WASD.ReadValue<Vector2>();
            }
        }
        else
        {
            menu_move_input = (int)input_system.Menu.VerticalMove.ReadValue<float>();
        }
        
        if (vibrate_duration > 0.0f)
        {
            if (gamepad != null)
            {
                gamepad.SetMotorSpeeds(vibrate_strength, vibrate_strength);
            }
            
            vibrate_duration -= Time.unscaledDeltaTime;
        }
        else
        {
            if (gamepad != null)
            {
                gamepad.SetMotorSpeeds(0.0f, 0.0f);
            }
        }
    }
    
    private void LateUpdate() 
    {
        ResetAllParams();
    }
    
    private void ResetAllParams()
    {
        //変数を初期化する
        press_flip = false;
        press_rotate = false;
        press_pause = false;
        press_select = false;
        press_cancel = false;
        press_start = false;
        
        press_menu_right = false;
        press_menu_left = false;
    }
    
    private void SmashInput(InputAction.CallbackContext obj)
    {
        press_smash = true;
    }
    
    private void ReleaseSmashInput(InputAction.CallbackContext obj)
    {
        press_smash = false;
    }
    
    private void FlipInput(InputAction.CallbackContext obj)
    {
        press_flip = true;
    }
    
    private void RotateInput(InputAction.CallbackContext obj)
    {
        press_rotate = true;
    }
    
    private void PauseInput(InputAction.CallbackContext obj)
    {
        press_pause = true;
    }
    
    private void SelectInput(InputAction.CallbackContext obj)
    {
        press_select = true;
    }
    
    private void CancelInput(InputAction.CallbackContext obj)
    {
        press_cancel = true;
    }
    
    private void StartButtonInput(InputAction.CallbackContext obj)
    {
        press_start = true;
    }
    
    private void MenuRightInput(InputAction.CallbackContext obj)
    {
        press_menu_right = true;
    }
    
    private void MenuLeftInput(InputAction.CallbackContext obj)
    {
        press_menu_left = true;
    }
    
    private void ProtoReloadScene(InputAction.CallbackContext obj)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    private void ProtoEndScene(InputAction.CallbackContext obj)
    {
        Application.Quit();
    }
    
    private void ProtoNextScene(InputAction.CallbackContext obj)
    {
        int currentSceneName = SceneManager.GetActiveScene().buildIndex;
        
        if (currentSceneName == 6)
        {
            SceneManager.LoadScene("Select");
        }
        else
        {
            SceneManager.LoadScene(currentSceneName + 1);
        }
    }
    
    
    private void OnEnable() 
    {
        input_system.Player.Smash.performed += SmashInput;
        input_system.Player.Smash.canceled += ReleaseSmashInput;
        input_system.Player.Enable();
        
        input_system.Player.Flip.performed += FlipInput;
        input_system.Player.Flip.Enable();
        
        input_system.Player.Rotate.performed += RotateInput;
        input_system.Player.Rotate.Enable();
        
        input_system.Player.Pause.performed += PauseInput;
        input_system.Player.Pause.Enable();
        
        input_system.Menu.Select.performed += SelectInput;
        input_system.Menu.Select.Enable();
        
        input_system.Menu.Cancel.performed += CancelInput;
        input_system.Menu.Cancel.Enable();
        
        input_system.Menu.StartButton.performed += StartButtonInput;
        input_system.Menu.StartButton.Enable();
        
        input_system.Menu.Right.performed += MenuRightInput;
        input_system.Menu.Right.Enable();
        
        input_system.Menu.Left.performed += MenuLeftInput;
        input_system.Menu.Left.Enable();
        
        input_system.Prototype.ReloadScene.performed += ProtoReloadScene;
        input_system.Prototype.ReloadScene.Enable();
        
        input_system.Prototype.EndScene.performed += ProtoEndScene;
        input_system.Prototype.EndScene.Enable();
        
        input_system.Prototype.NextScene.performed += ProtoNextScene;
        input_system.Prototype.NextScene.Enable();
        
        input_system.Enable();
    }
    
    private void OnDisable() 
    {
        input_system.Disable();
    }
}