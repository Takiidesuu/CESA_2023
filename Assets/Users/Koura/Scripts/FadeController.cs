using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeController : MonoBehaviour
{
    //Canvasが召喚されているか
    public static bool fade_instance = false;

    //フェードインを管理するフラグ
    public bool fade_in = false;
    //フェードアウトを管理するフラグ
    public bool fade_out = false;

    //透過率を変化させる
    public float alpha = 0.0f;
    //フェードに掛かる時間
    public float fade_speed = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        if (!fade_instance)//Canvasが既に、召喚されているなら
        {
            DontDestroyOnLoad(this);
            fade_instance = true;
        }
        else
        {
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (fade_in)//フェードイン処理
        {
            alpha -= Time.deltaTime / fade_speed;

            //フェードインが終了したら
            if (alpha <= 0.0f)
            {
                fade_in = false;
                alpha = 0.0f;
            }
            this.GetComponentInChildren<Image>().color = new Color(0.0f, 0.0f, 0.0f, alpha);
        }
        else if (fade_out)//フェードアウト処理
        {
            alpha += Time.deltaTime / fade_speed;

            //フェードアウトが終了したら
            if (alpha >= 1.0f)
            {
                fade_in = false;
                alpha = 1.0f;
            }
            this.GetComponentInChildren<Image>().color = new Color(0.0f, 0.0f, 0.0f, alpha);
        }
    }

    public void FadeIn()
    {
        fade_in = true;
        fade_out = false;
    }

    public void FadeOut()
    {
        fade_in = false;
        fade_out = true;
    }
}
