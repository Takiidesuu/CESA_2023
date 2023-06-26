using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WindowSize : MonoBehaviour
{
    public TMPro.TMP_Dropdown dropdown;

    //ドロップダウンの値が変更された時、選択肢の番号が引数に与えられ呼び出される
    public void OnValueChanged()
    {
        switch (dropdown.value)
        {
            case 0:
                Screen.SetResolution(Screen.width, Screen.height, true);
                break;

            case 1:
                Screen.SetResolution(1920, 1080, false);
                break;

            case 2:
                Screen.SetResolution(1600, 900, false);
                break;

            case 3:
                Screen.SetResolution(1366, 768, false);
                break;

            case 4:
                Screen.SetResolution(1280, 720, false);
                break;
        }
        Debug.Log(dropdown.value);
    }
}
