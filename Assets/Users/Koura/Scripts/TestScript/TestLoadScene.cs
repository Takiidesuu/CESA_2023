using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestLoadScene : MonoBehaviour
{
    private bool test_change = false;

    public float move_speed = 1.0f;
    public float g_camrot = 0.0f;

    //カルテのポジションを決める
    [SerializeField] Transform target;

    enum CAMERA_DIRECTION//現在カメラが回転している方向
    {
        NEUTRAL,
        FOWARD,
        BACK
    }

    CAMERA_DIRECTION cam_direction = CAMERA_DIRECTION.NEUTRAL;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(cam_direction == CAMERA_DIRECTION.NEUTRAL)
        {
            if (Keyboard.current.spaceKey.isPressed)
            {
                cam_direction = CAMERA_DIRECTION.FOWARD;
            }
        }

        if(cam_direction == CAMERA_DIRECTION.FOWARD)
        {
            g_camrot += move_speed;

            //決められたカメラのポジションへ移動を行う
            transform.Rotate(0.0f, move_speed, 0.0f);
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, 12.0f * Time.deltaTime);

            if (g_camrot >= 90.0f)
            {
                g_camrot = 0.0f;
                cam_direction = CAMERA_DIRECTION.NEUTRAL;
            }
        }
        //if (!test_change)
        //{
        //    if (Keyboard.current.spaceKey.isPressed)
        //    {
        //        //決められたカメラのポジションへ移動を行う
        //        transform.Rotate(0.0f, move_speed, 0.0f);
        //        //camera_move
        //        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, 5.0f * Time.deltaTime);
        //        //FindObjectOfType<SceneController>().SceneChange("Select");
        //        //test_change = true;
        //    }
        //}
    }
}
