using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadScene : MonoBehaviour
{
    //シーンチェンジのタイミングを管理するフラグ
    public static bool change_flg = true;

    enum SCENE_TYPE//シーンの種類
    {
        Title,
        Game,
        Result
    }

    //Enum内の文字を取得する為のもの
    SCENE_TYPE scene_type = SCENE_TYPE.Game;

    //シーンを遷移したいタイミングで下記の2行を書く
    //scene_type = SCENE_TYPE.;
    //GetComponent<SceneController>().SceneChange(scene_type.ToString());

    // Start is called before the first frame update
    void Start()
    {
        //GetComponent<SceneController>().SceneChange("Game");
    }

    // Update is called once per frame
    void Update()
    {
        //if (!change_flg)
        //{
        //    change_flg = true;
        //}

        if (change_flg)
        {
            //Enumの文字を取得し、取得した名前の画面へ遷移する
            GetComponent<SceneController>().SceneChange(scene_type.ToString());
            Debug.Log("通った");
            change_flg = false;
        }
    }
}
