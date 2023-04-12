using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KarteRotation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.upArrowKey.isPressed)//上ボタンを押したら
        {
            transform.Rotate(new Vector3(0.2f, 0f, 0f));
        }
        else if (Keyboard.current.downArrowKey.isPressed)//下ボタンを押したら
        {
            transform.Rotate(new Vector3(-0.2f, 0f, 0f));
        }
    }
}
