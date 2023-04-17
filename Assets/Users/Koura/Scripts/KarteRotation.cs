using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KarteRotation : MonoBehaviour
{
    enum STAGE_DIRECTION//現在カルテ(ステージ)が回転している方向
    {
        NEUTRAL,
        UP,
        DOWN
    }

    STAGE_DIRECTION g_direction = STAGE_DIRECTION.NEUTRAL;

    //現在回転した角度
    public float g_nowrotate = 0.0f;
    //回転する角度
    public float g_karte_x = 0.2f;
    //現在選択してるカルテ(最初はワールド1)
    //public 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(g_direction == STAGE_DIRECTION.NEUTRAL)//ニュートラルの時だけ入力を受け付ける
        {
            if (Keyboard.current.upArrowKey.isPressed)//上ボタンを押したら
            {
                g_direction = STAGE_DIRECTION.UP;
                //transform.Rotate(new Vector3(0.2f, 0f, 0f));
            }
            else if (Keyboard.current.downArrowKey.isPressed)//下ボタンを押したら
            {
                g_direction = STAGE_DIRECTION.DOWN;
                //transform.Rotate(new Vector3(-0.2f, 0f, 0f));
            }
        }


        if (g_direction == STAGE_DIRECTION.UP)//上ボタンをした後の処理
        {
            //回転が始まってからの角度を記録していく
            g_nowrotate += g_karte_x;

            transform.Rotate(new Vector3(g_karte_x, 0f, 0f));

            if (g_nowrotate >= 90.0f)
            {
                g_nowrotate = 0.0f;
                g_direction = STAGE_DIRECTION.NEUTRAL;
            }
        }
        else if (g_direction == STAGE_DIRECTION.DOWN)//下ボタンをした後の処理
        {
            //回転が始まってからの角度を記録していく
            g_nowrotate -= g_karte_x;

            transform.Rotate(new Vector3(-g_karte_x, 0f, 0f));

            if (g_nowrotate <= -90.0f)
            {
                g_nowrotate = 0.0f;
                g_direction = STAGE_DIRECTION.NEUTRAL;
            }
        }
    }
}
