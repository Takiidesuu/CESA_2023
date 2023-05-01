using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KarteRotation : MonoBehaviour
{
    public enum STAGE_DIRECTION//現在カルテ(ステージ)が回転している方向
    {
        NEUTRAL,
        UP,
        DOWN
    }

    STAGE_DIRECTION g_direction = STAGE_DIRECTION.NEUTRAL;

    //現在回転した角度
    public float g_nowrotate = 0.0f;
    //回転する角度
    public float g_rotate_speed = 0.2f;
    //カメラがワールド選択の場所にあるか
    [System.NonSerialized]
    public bool g_world_cam = true;
    //現在選択してるカルテ(ワールド)
    [System.NonSerialized]
    public int g_now_world = 1;
    //現在選択している部位(ステージ)
    public int g_now_stage = 1;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (g_direction == STAGE_DIRECTION.NEUTRAL && g_world_cam)//ニュートラルの時だけ入力を受け付ける
        {
            if (Keyboard.current.upArrowKey.isPressed)//上ボタンを押したら
            {
                g_direction = STAGE_DIRECTION.UP;
            }
            else if (Keyboard.current.downArrowKey.isPressed)//下ボタンを押したら
            {
                g_direction = STAGE_DIRECTION.DOWN;
            }
        }

        if (g_direction == STAGE_DIRECTION.UP)//上ボタンをした後の処理
        {
            //回転が始まってからの角度を記録していく
            g_nowrotate += g_rotate_speed;

            transform.Rotate(new Vector3(g_rotate_speed, 0f, 0f));

            //90度回転し終わったら
            if (g_nowrotate >= 90.0f)
            {
                //現在のワールドの数を足す
                g_now_world++;
                //ワールド4以上になりそうだったら
                if (g_now_world > 4)
                {
                    g_now_world = 1;
                }

                g_nowrotate = 0.0f;
                g_direction = STAGE_DIRECTION.NEUTRAL;
                Debug.Log("Stage" + g_now_world.ToString() + "-" + g_now_stage.ToString());
            }
        }
        else if (g_direction == STAGE_DIRECTION.DOWN)//下ボタンをした後の処理
        {
            //回転が始まってからの角度を記録していく
            g_nowrotate -= g_rotate_speed;

            transform.Rotate(new Vector3(-g_rotate_speed, 0f, 0f));

            //90度回転し終わったら
            if (g_nowrotate <= -90.0f)
            {
                //現在のワールドの数を引く
                g_now_world--;
                //ワールド1以下になりそうになったら
                if (g_now_world < 1)
                {
                    g_now_world = 4;
                }

                g_nowrotate = 0.0f;
                g_direction = STAGE_DIRECTION.NEUTRAL;
                //Debug.Log("現在のワールド：" + g_now_world.ToString());
            }
        }
    }
}
