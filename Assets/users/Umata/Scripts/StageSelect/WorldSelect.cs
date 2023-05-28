using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class WorldSelect : MonoBehaviour
{
    [System.Serializable]
    public class Textures
    {
        public List<Texture2D> StageTextures = new List<Texture2D>();
    }
    [SerializeField] public List<Textures> WorldTextures = new List<Textures>();       //表示する画像

    // ワールドとステージの数
    public int numWorlds = 4;
    public int numStages = 6;

    //PostProcess
    public GameObject postProcessVolume;
    public GameObject NoVignetteVolume;

    private Vignette vignette;
    // カメラの移動に使用する時間
    public float time = 1f;

    // 選択中のワールドとステージ
    public int currentWorld = 0;
    public int currentStage = 0;

    // カメラの移動先の位置と回転
    public Transform worldSelectPos;
    public Transform stageSelectPos;
    public Transform LastCameraPos;

    public GameObject CameraObj;
    public GameObject PlayerObj;
    public GameObject BlackPanelImage;//BlackPanel
    public GameObject MainPreviewObj; // メインプレビューオブジェクト
    public GameObject[] SubPreviewObj;  // サブプレビューオブジェクト
    public GameObject[] SubPreviewHorogram;  // サブプレビューホロオブジェクト

    public GameObject[] WorldSelectModel;    //ワールドモデル
    public GameObject[] LockedWorldSelectModel;    //ワールドモデル
    public GameObject[] WorldSelectHorogram;    //ワールドモデル

    //ワールド選択時の矢印
    public GameObject LeftArrow;
    public GameObject RightArrow;

    [SerializeField] private AnimationCurve curve = AnimationCurve.Linear(0f, 0f, 1f, 1f); // イージングカーブ

    private bool selectingWorld = true; // ワールドを選択中かどうか
    private bool selectingStage = true; // ステージを選択中かどうか
    private bool isTransitioning = false;
    private Image BlackPanel;//BlackPanel

    // 変更後の強さ
    private float targetIntensity;

    // 変更にかかる時間
    public float duration;
    public float BlackPanelDuration;
    public float TransitionStartDelay;

    // 変更を開始した時刻
    private float startTime;
    private Material MainPreviewMaterial; // Unlitマテリアルへの参照
    public Material[] SubPreviewMaterial;  // サブプレビューオブジェクト
    public Material[] SubPreviewHorogramMaterial;  // サブプレビューホロオブジェクト
    public Material[] HorogramMaterial;

    //ステージデータ
    private GameObject SaveData;

    /// <summary>
    /// 平田
    /// </summary>
    LoadSceneFade LoadSceneFade;//スクリプト
    bool isSceneChange = false; //シーン移行のフラグ
    AsyncOperation asyncOperation;//進捗チェック
    private SoundManager soundManager;  //サウンドマネージャー

    void Start()
    {
        soundManager = GetComponent<SoundManager>();
        LoadSceneFade = GameObject.Find("LoadSceneFade").GetComponent<LoadSceneFade>();
        postProcessVolume.transform.GetComponent<Volume>().profile.TryGet(out vignette);

        //ポストプロセスを格納
        // 初期位置にカメラを移動
        CameraObj.transform.position = worldSelectPos.position;
        CameraObj.transform.rotation = worldSelectPos.rotation;

        // 初期値を設定
        targetIntensity = vignette.intensity.value;

        //セーブデータ格納
        SaveData = GameObject.Find("StageData");

        //BlackPanelを取得
        BlackPanel = BlackPanelImage.GetComponent<Image>();

        Renderer renderer = MainPreviewObj.GetComponent<Renderer>();
        MainPreviewMaterial = renderer.material;
        for (int i = 0; i < SubPreviewObj.Length; i++) 
        {
            SubPreviewMaterial[i] = SubPreviewObj[i].GetComponent<Renderer>().material;
        }
        for (int i = 0; i < SubPreviewHorogram.Length; i++)
        {
            SubPreviewHorogramMaterial[i] = SubPreviewHorogram[i].GetComponent<Renderer>().material;
        }
        if (StageDataManager.instance.now_stage != -1)
            currentStage = StageDataManager.instance.now_stage;
        if (StageDataManager.instance.now_world != -1)
            currentWorld = StageDataManager.instance.now_world;
        if (StageDataManager.instance.now_world != -1 && StageDataManager.instance.now_world != -1)
        {
            selectingStage = true;
            selectingWorld = false;
        }

    }
    void Update()
    {
        if (InputManager.instance.press_select)
        {
            if (selectingWorld)
            {
                // ワールドを選択中の場合、ステージ選択に移行
                selectingWorld = false;
                soundManager.PlaySoundEffect("OK");
            }
            else
            {

                // ステージを選択中の場合、シーンをロード
                if (CameraObj.transform.position.x - stageSelectPos.position.x < 0.05)
                {
                    isTransitioning = true;
                    startTime = Time.time;
                    //Vignetイージングを開始
                    PlayerObj.GetComponent<CellEffect>().ChangeMaterialsToInvisible();
                    Invoke("LoadScene", time * 3);
                    Invoke("TransitionInit", 1);
                    soundManager.PlaySoundEffect("GO");
                }


            }
        }
        //ワールド選択モデルの変更
        ChangeWorldModel(currentWorld);

        //シーン遷移エフェクト中
        if(isTransitioning)
        {
            TransitionStartDelay += Time.deltaTime;

            if (TransitionStartDelay > 1.0f)
            {
                // 変更する値をイージングで計算
                float t = (Time.time - startTime) / (duration / 12);
                float intensity = Mathf.Lerp(vignette.intensity.value, targetIntensity, t * t);

                // 値を設定
                vignette.intensity.value = intensity;

                // 現在の時間からフェードインの進行度合いを計算する
                float elapsedTime = Time.time - startTime;
                float progress = Mathf.Clamp01(elapsedTime / (BlackPanelDuration / 12));

                // イージングカーブを適用したアルファ値を計算する
                float alpha = curve.Evaluate(progress);

                // Imageのカラーを更新する
                Color color = BlackPanel.color;
                BlackPanel.color = new Color(color.r, color.g, color.b, alpha);

            }

            //背景が真っ暗になったらPostProcessを変更
            if (BlackPanel.color.a >= 1)
            {
                postProcessVolume.SetActive(false);
                NoVignetteVolume.SetActive(true);
            }
        }
        if (selectingWorld)
        {
            transform.GetComponent<StageSelectManager>().IsWorldSelect = false;
        }
        else
        {
            transform.GetComponent<StageSelectManager>().IsWorldSelect = true;
        }
        if (InputManager.instance.press_menu_right)
        {
            if (!isTransitioning)
            {
                // 右キーでワールドまたはステージを1つ進める
                if (selectingWorld && currentWorld != 3)
                {
                    currentWorld = (currentWorld + 1) % numWorlds;
                }
                else
                {
                    currentStage = (currentStage + 1) % numStages;
                }
                soundManager.PlaySoundEffect("Cursor");
            }
        }

        if (InputManager.instance.press_menu_left)
        {
            if (!isTransitioning)
            {
                // 左キーでワールドまたはステージを1つ戻す
                if (selectingWorld && currentWorld != 0)
                {
                    currentWorld = (currentWorld - 1 + numWorlds) % numWorlds;
                }
                else
                {
                    currentStage = (currentStage - 1 + numStages) % numStages;
                }
                soundManager.PlaySoundEffect("Cursor");
            }
        }

        if (InputManager.instance.press_cancel)
        {
            // Escキーで選択中のステップを1つ戻す
            if (!selectingWorld)
            {
                selectingWorld = true;
                soundManager.PlaySoundEffect("Cancel");
            }
        }

        // カメラを移動する
        if (!selectingStage)
        {
            MoveCamera(LastCameraPos);
        }
        else
        {
            if (selectingWorld)
            {
                // ワールド選択中の場合、WorldSelectPosに移動
                MoveCamera(worldSelectPos);
            }
            else
            {
                // ステージ選択中の場合、StageSelectPosに移動する
                MoveCamera(stageSelectPos);
            }
        }

        //ロード開始したら画面をフェードする
        if (asyncOperation != null)
        {
            //9割ロードし終われば移行
            if (LoadSceneFade.SpliteOnceMove(asyncOperation.progress) > 0.9f)
                asyncOperation.allowSceneActivation = true;
        }

        //メインプレビューの画像を設定
        MainPreviewMaterial.SetTexture("_BaseMap",WorldTextures[currentWorld].StageTextures[currentStage]);
        for (int i = 0; i < SubPreviewObj.Length; i++)
        {
            if (WorldTextures[currentWorld].StageTextures[i] != null)
            {
                SubPreviewMaterial[i].SetTexture("_BaseMap", WorldTextures[currentWorld].StageTextures[i]);
            }
        }
        for (int i = 0; i < SubPreviewHorogramMaterial.Length; i++)
        {
            //選択シーンが現在の画像である場合
            if (i == currentStage)
            {
                SubPreviewHorogram[i].GetComponent<Renderer>().material = HorogramMaterial[1];
            }
            else
            {
                SubPreviewHorogram[i].GetComponent<Renderer>().material = HorogramMaterial[0];
            }
        }
    }

    // カメラを移動する
    private void MoveCamera(Transform targetPos)
    {
        if (!selectingStage)
        {
            CameraObj.transform.position = Vector3.Lerp(CameraObj.transform.position, targetPos.position, (time/2) * Time.deltaTime);
            CameraObj.transform.rotation = Quaternion.Lerp(CameraObj.transform.rotation, targetPos.rotation, (time/2) * Time.deltaTime);
            CameraObj.transform.GetComponent<Camera>().fieldOfView += (float)(20 * Time.deltaTime);
        }
        else
        {
            CameraObj.transform.position = Vector3.Lerp(CameraObj.transform.position, targetPos.position, time * Time.deltaTime);
            CameraObj.transform.rotation = Quaternion.Lerp(CameraObj.transform.rotation, targetPos.rotation, time * Time.deltaTime);
        }
    }

    // シーンをロードする
    private void LoadScene()
    {
        // ワールドとステージに応じてシーンをロードする処理を書く
        string stage_name = "Stage" + (currentWorld+1) + "-" + (currentStage+1);
        //fadeのテクスチャセットロード開始
        if (asyncOperation == null)
        {
            asyncOperation = SceneManager.LoadSceneAsync(stage_name);
            LoadSceneFade.SetTexture(currentWorld, currentStage);
        }
        asyncOperation.allowSceneActivation = false;
    }

    private void TransitionInit()
    {
        SetVignetteIntensity(1, 100);
        startTime = Time.time;
        postProcessVolume.SetActive(true);
        selectingStage = false;

    }
    public void SetVignetteIntensity(float intensity, float time)
    {
        // 変更前の値を保存
        float currentIntensity = vignette.intensity.value;

        // 変更後の値と時間を設定
        targetIntensity = intensity;
        duration = time;
        startTime = Time.time;

        // 変更前と後の値を比較して、変更が必要な場合にのみUpdateを有効にする
        if (currentIntensity != targetIntensity)
        {
            enabled = true;
        }
    }

    private void ChangeWorldModel(int currentWorld)
    {
        for (int i = 0; i < WorldSelectModel.Length; i++)
        {
            WorldSelectModel[i].SetActive(false);
            WorldSelectHorogram[i].SetActive(false);
            LockedWorldSelectModel[i].SetActive(false);

        }
        //各ステージの5面がクリアされている場合
        if (currentWorld !=0 && SaveData.GetComponent<StageDataManager>().worlds[currentWorld - 1].stages[4].Score != 0)
        {
            WorldSelectModel[currentWorld].SetActive(true);
            WorldSelectHorogram[currentWorld].SetActive(true);
        }
        else
        {
            LockedWorldSelectModel[currentWorld].SetActive(true);
            WorldSelectHorogram[currentWorld].SetActive(true);
        }
        //矢印の表示切り替え
        LeftArrow.SetActive(false);
        RightArrow.SetActive(false);

        if (currentWorld != 3 && currentWorld >= 0)
        {
            RightArrow.SetActive(true);
        }
        if(currentWorld != 0 && currentWorld <= 3)
        {
            LeftArrow.SetActive(true);
        }
    }
}
