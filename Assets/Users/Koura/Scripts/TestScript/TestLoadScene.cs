using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestLoadScene : MonoBehaviour
{
    private bool test_change = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!test_change)
        {
            if (Keyboard.current.spaceKey.isPressed)
            {
                FindObjectOfType<SceneController>().SceneChange("Select");
                test_change = true;
            }
        }
    }
}
