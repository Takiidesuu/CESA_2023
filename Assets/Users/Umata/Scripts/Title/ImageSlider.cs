using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ImageSlider : MonoBehaviour
{
    public Image[] images;  // ３つのImageを格納する配列
    public int select_button = 0;  // 選択中のボタンのインデックス
    public int select_distance = 50;  // ボタンをずらす距離
    public float select_delay = 0.3f; // ボタン選択の更新を制限する時間

    public bool is_firsttime = true; // 初回起動かどうかのフラグ

    public string scene_start_name; //最初のシーン名
    public string scene_continue_name; //続きのシーン名
    public string scene_option_name; //オプションのシーン名


    public Image image_hammer; // 追加のImage Hammer
    public Image image_banner; // 追加のImage Banner


    public float hammer_image_distance;  // 初期座標を保存する配列

    private float[] init_positions;  // 初期座標を保存する配列
    private float timeSinceSelect = 0f; // 前回のボタン選択からの時間
    private bool canSelect = true; // ボタン選択が可能かどうかのフラグ

    void Start()
    {
        // 初期座標を配列に保存
        init_positions = new float[images.Length];
        for (int i = 0; i < images.Length; i++)
        {
            init_positions[i] = images[i].rectTransform.anchoredPosition.x;
        }

        // 初期状態のボタンを選択
        SetSelectedButton(select_button);

        
    }

    void Update()
    {
        //シーン切替
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump"))
        {
            // スペースキーまたはジャンプボタンが押されたときの処理
            switch (select_button){
                case 0:
                    SceneManager.LoadScene(scene_start_name);
                    break;
                case 1:
                    SceneManager.LoadScene(scene_continue_name);
                    break;

                case 2:
                    SceneManager.LoadScene(scene_option_name);
                    break;
            }
        }
        // 初回起動の場合、additionalImageのアルファ値を255に設定
        if (!is_firsttime)
        {
            Color color = images[1].color;
            color.a = 1;
            images[1].color = color;
        }
        else
        {
            Color color = images[1].color;
            color.a = 0.5f;
            images[1].color = color;
        }

        timeSinceSelect += Time.deltaTime;

        // キーボード操作で選択中のボタンを変更
        if (canSelect && (Input.GetKeyDown(KeyCode.W) || Input.GetAxis("Vertical") > 0))
        {
            timeSinceSelect = 0f;
            canSelect = false;
            select_button--;
            if (select_button < 0)
            {
                select_button = images.Length - 1;
            }
        }
        else if (canSelect && (Input.GetKeyDown(KeyCode.S) || Input.GetAxis("Vertical") < 0))
        {
            timeSinceSelect = 0f;
            canSelect = false;
            select_button++;
            if (select_button >= images.Length)
            {
                select_button = 0;
            }
        }

        // ボタン選択の更新が制限時間を超えたらフラグを立てる
        if (timeSinceSelect >= select_delay)
        {
            canSelect = true;
        }

        // 選択中のボタンを移動
        SetSelectedButton(select_button);
    }

    // 選択中のボタンを移動する関数
    void SetSelectedButton(int index)
    {
        for (int i = 0; i < images.Length; i++)
        {
            // 初期座標から選択中のボタンの距離に応じて、X座標をずらす
            float x = init_positions[i];
            if (i == index)
            {
                x -= select_distance;
            }

            images[i].rectTransform.anchoredPosition = new Vector2(x, images[i].rectTransform.anchoredPosition.y);
            image_hammer.rectTransform.anchoredPosition = new Vector2(images[select_button].rectTransform.anchoredPosition.x - hammer_image_distance, images[select_button].rectTransform.anchoredPosition.y);
            image_banner.rectTransform.anchoredPosition = new Vector2(images[select_button].rectTransform.anchoredPosition.x, images[select_button].rectTransform.anchoredPosition.y);

        }
    }
}