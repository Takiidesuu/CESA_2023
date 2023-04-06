using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LoadScene : MonoBehaviour
{
    //シーンチェンジのタイミングを管理するフラグ
    public static bool change_flg = true;

    //シーンを遷移したいタイミングで下記の1行を書く
    //FindObjectOfType<SceneController>().SceneChange(next_scene);

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (change_flg)
        {
            TestLoad();
        }
    }

    public void TestLoad()//テストとして画面をロードする
    {
        //Enumの文字を取得し、取得した名前の画面へ遷移する
        FindObjectOfType<SceneController>().SceneChange("Select");
        Debug.Log("通った");
        change_flg = false;
    }
}
