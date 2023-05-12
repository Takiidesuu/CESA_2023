using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SelectStage : MonoBehaviour
{
    //カメラの移動が終わっているかのフラグ管理
    private bool cam_move_end = false;
    //現在カメラはステージの選択場所にいるか
    private bool now_cam_end = false;

    //カメラの移動速度
    public float move_speed = 1.0f;

    //開始するステージの名前
    public string start_stage;

    //シーンを移動するフラグ
    [System.NonSerialized]
    public bool change_flg = false;

    //現在画面遷移しているか
    [System.NonSerialized]
    public bool now_scene_change = false;

    //ワールド選択の場所以外で回転させないよう管理するためのもの
    public KarteRotation karteRotation;

    //カメラのポジションを決める
    [SerializeField] Transform start_target;
    [SerializeField] Transform end_target;

    enum CAMERA_DIRECTION//現在のカメラの動き
    {
        NEUTRAL,
        FORWARD,
        BACK
    }

    CAMERA_DIRECTION cam_direction = CAMERA_DIRECTION.NEUTRAL;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = start_target.position;
        KarteRotation karterotation = GetComponent<KarteRotation>();
        start_stage = "Stage" + 
                       karteRotation.g_now_world.ToString() + 
                       "-" + 
                       karteRotation.g_now_stage.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        WorldSelect();
        StageSelect();
        NextScene();
    }

    public void WorldSelect()//ワールド(カルテ)セレクトの仕様
    {
        //NEUTRAL時以外入力を受け付けない
        if (cam_direction == CAMERA_DIRECTION.NEUTRAL)
        {
            //spaceキーを押したら
            if (Keyboard.current.spaceKey.isPressed)
            {
                //カメラが初期位置なら
                if (!cam_move_end)
                {
                    cam_direction = CAMERA_DIRECTION.FORWARD;
                    karteRotation.g_world_cam = false;
                }

            }
            else if (Keyboard.current.escapeKey.isPressed)
            {
                if (cam_move_end)
                {
                    now_cam_end = false;
                    cam_direction = CAMERA_DIRECTION.BACK;
                }
            }
        }

        //ワールド選択からステージ選択へカメラを移動する処理
        if (cam_direction == CAMERA_DIRECTION.FORWARD)
        {
            //決められたカメラのポジションへ移動を行う
            transform.position = Vector3.MoveTowards(transform.position,
                                                     end_target.transform.position,
                                                     move_speed * Time.deltaTime);

            //移動し終わったら
            if (transform.position == end_target.transform.position)
            {
                cam_move_end = true;
                now_cam_end = true;
                cam_direction = CAMERA_DIRECTION.NEUTRAL;
            }
        }
        //ステージ選択からワールド選択へ戻る
        else if (cam_direction == CAMERA_DIRECTION.BACK)
        {
            //決められたカメラのポジションへ移動を行う
            transform.position = Vector3.MoveTowards(transform.position,
                                                     start_target.transform.position,
                                                     move_speed * Time.deltaTime);
            //移動し終わったら
            if (transform.position == start_target.transform.position)
            {
                cam_move_end = false;
                cam_direction = CAMERA_DIRECTION.NEUTRAL;
                karteRotation.g_world_cam = true;
                karteRotation.g_now_stage = 1;
            }
        }
    }

    public void StageSelect()//ステージセレクトの仕様
    {
        //カメラの位置が最終位置にいる
        if (now_cam_end)
        {
            if (Keyboard.current.rightArrowKey.wasPressedThisFrame)//右ボタンを押したら
            {
                karteRotation.g_now_stage++;
                //ワールド4以上になりそうだったら
                if (karteRotation.g_now_stage > 5)
                {
                    karteRotation.g_now_stage = 1;
                }
            }
            else if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
            {
                karteRotation.g_now_stage--;
                //ワールド1以下になりそうだったら
                if (karteRotation.g_now_stage < 1)
                {
                    karteRotation.g_now_stage = 5;
                }
            }

            //spaceキーを押したら
            if (Keyboard.current.spaceKey.wasPressedThisFrame && !now_scene_change)
            {
               start_stage = "Stage" +
                              karteRotation.g_now_world.ToString() +
                              "-" +
                              karteRotation.g_now_stage.ToString();

               change_flg = true;
               now_scene_change = true;
            }

        }
    }

    public void NextScene()
    {
        if (change_flg)
        {
            FindObjectOfType<SceneController>().SceneChange(start_stage);
            Debug.Log("通った");
            change_flg = false;
        }
    }
}
