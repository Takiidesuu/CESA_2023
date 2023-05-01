using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageColor : MonoBehaviour
{
    //ステージとなるオブジェクト
    GameObject[] stages;
    //選択されているステージの色
    public Material Select;
    //選択されていないステージの色
    public Material noSelect;

    //現在選択されているステージを参照する
    private string now_select_stage;

    //ワールド選択の場所以外で回転させないよう管理するためのもの
    public KarteRotation karteRotation;

    // Start is called before the first frame update
    void Start()
    {
        //Stageのタグの付いたオブジェクトをすべて格納する
        stages = GameObject.FindGameObjectsWithTag("Stage");
    }

    // Update is called once per frame
    void Update()
    {
        //現在選択されているステージは何か
        now_select_stage = "Stage" + karteRotation.g_now_stage.ToString();

        //ステージの数だけ繰り返す
        foreach (GameObject stage in stages)
        {
            //現在選択されているステージと同じ名前なら
            if (stage.name == now_select_stage)
            {
                //選択されているとき用のマテリアルを反映する
                stage.GetComponent<Renderer>().material.color = Select.color;
            }
            else
            {
                //選択されていない用のマテリアルを反映する
                stage.GetComponent<Renderer>().material.color = noSelect.color;
            }
        }
    }
}
