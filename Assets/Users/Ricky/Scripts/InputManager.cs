using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    public static InputManager instance {get; private set;}
    MainInputControls input_system;
    
    private string current_scene;
    
    public Vector2 player_move_float {get; private set;}
    public float menu_move_float {get; private set;}
    
    public bool press_smash {get; private set;}
    public bool press_flip {get; private set;}
    public bool press_rotate {get; private set;}
    public bool press_pause {get; private set;}
    public bool press_select {get; private set;}
    public bool press_cancel {get; private set;}
    public bool press_start {get; private set;}
    
    private void Awake() 
    {
        input_system = new MainInputControls();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        current_scene = SceneManager.GetActiveScene().name;
        
        if (current_scene.Contains("Stage"))
        {
            input_system.Player.Enable();
            input_system.Menu.Disable();
        }
        else
        {
            input_system.Player.Disable();
            input_system.Menu.Enable();
        }
        
        ResetAllParams();
    }

    // Update is called once per frame
    void Update()
    {
        if (current_scene.Contains("Stage"))
        {
            player_move_float = input_system.Player.WASD.ReadValue<Vector2>();
        }
        else
        {
            menu_move_float = input_system.Menu.VerticalMove.ReadValue<float>();
        }
    }
    
    private void LateUpdate() 
    {
        ResetAllParams();
    }
    
    private void ResetAllParams()
    {
        //変数を初期化する
        press_smash = false;
        press_flip = false;
        press_rotate = false;
        press_pause = false;
        press_select = false;
        press_cancel = false;
        press_start = false;
    }
    
    private void SmashInput(InputAction.CallbackContext obj)
    {
        press_smash = true;
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
    
    private void OnEnable() 
    {
        input_system.Player.Smash.performed += SmashInput;
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
        
        input_system.Enable();
    }
    
    private void OnDisable() 
    {
        input_system.Disable();
    }
}