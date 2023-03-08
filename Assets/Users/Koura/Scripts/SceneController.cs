using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    //インスペクタからPrefab化したCanvasを入れる
    public GameObject fade_canvas;

    // Start is called before the first frame update
    void Start()
    {
        //Fade用のCanvasが召喚されているか
        if (!FadeController.fade_instance)
        {
            //召喚されていなければ持ってくる
            Instantiate(fade_canvas);
        }

        //下記の関数を持ってくる
        FindFadeObject();
    }

    void FindFadeObject()//Fadeタグの付いたCanvasを見つけ、フェードインを実行する
    {
        fade_canvas = GameObject.FindGameObjectWithTag("Fade");
        fade_canvas.GetComponent<FadeController>().FadeIn();
    }

    public async void SceneChange(string str)//この処理を呼べば、画面遷移が出来る
    {
        fade_canvas.GetComponent<FadeController>().FadeOut();
        await Task.Delay(3000);//暗転するまで待つ
        SceneManager.LoadScene(str);//シーンチェンジ
    }

}
