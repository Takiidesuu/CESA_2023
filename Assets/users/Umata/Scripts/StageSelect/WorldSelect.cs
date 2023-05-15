using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class WorldSelect : MonoBehaviour
{
    // ワールドとステージの数
    public int numWorlds = 4;
    public int numStages = 6;

    //PostProcess
    public GameObject postProcessVolume;
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
    public Transform HammerPos;
    public Transform BackImagePos;
    public Transform StartImagePos;

    public GameObject CameraObj;
    public GameObject PlayerObj;
    public GameObject BlackPanelImage;//BlackPanel


    [SerializeField] private AnimationCurve curve = AnimationCurve.Linear(0f, 0f, 1f, 1f); // イージングカーブ

    private Vector3 StartHammerPos;
    private Vector3 StartBackImagePos;
    private Vector3 StartStartImagePos;

    private bool selectingWorld = true; // ワールドを選択中かどうか
    private bool selectingStage = true; // ステージを選択中かどうか
    private bool isStart = true;
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

    /// <summary>
    /// 平田
    /// </summary>
    LoadSceneFade LoadSceneFade;//スクリプト
    bool isSceneChange = false; //シーン移行のフラグ
    AsyncOperation asyncOperation;//進捗チェック

    void Start()
    {
        LoadSceneFade = GameObject.Find("LoadSceneFade").GetComponent<LoadSceneFade>();
        postProcessVolume.transform.GetComponent<Volume>().profile.TryGet(out vignette);

        StartHammerPos = HammerPos.position;
        StartBackImagePos = BackImagePos.position;
        StartStartImagePos = StartImagePos.position;
        //ポストプロセスを格納
        // 初期位置にカメラを移動
        CameraObj.transform.position = worldSelectPos.position;
        CameraObj.transform.rotation = worldSelectPos.rotation;

        // 初期値を設定
        targetIntensity = vignette.intensity.value;

        //BlackPanelを取得
        BlackPanel = BlackPanelImage.GetComponent<Image>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (selectingWorld)
            {
                // ワールドを選択中の場合、ステージ選択に移行
                selectingWorld = false;
            }
            else
            {
                if (isStart)
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
                    }
                }
                else
                {
                    selectingWorld = true;
                }
            }
        }
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
        }
        if (selectingWorld)
        {
            transform.GetComponent<StageSelectManager>().IsWorldSelect = false;
        }
        else
        {
            transform.GetComponent<StageSelectManager>().IsWorldSelect = true;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            // 右キーでワールドまたはステージを1つ進める
            if (selectingWorld)
            {
                currentWorld = (currentWorld + 1) % numWorlds;
            }
            else
            {
                currentStage = (currentStage + 1) % numStages;
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            // 左キーでワールドまたはステージを1つ戻す
            if (selectingWorld)
            {
                currentWorld = (currentWorld - 1 + numWorlds) % numWorlds;
            }
            else
            {
                currentStage = (currentStage - 1 + numStages) % numStages;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Escキーで選択中のステップを1つ戻す
            if (!selectingWorld)
            {
                selectingWorld = true;
            }
        }
        if (!selectingWorld)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Vector3 startpos;
                startpos.x = StartHammerPos.x;
                startpos.y = StartHammerPos.y;
                startpos.z = StartHammerPos.z;

                HammerPos.position = startpos;

                startpos.x = StartBackImagePos.x;
                startpos.y = StartBackImagePos.y;
                startpos.z = StartBackImagePos.z;

                BackImagePos.position = startpos;

                startpos.x = StartStartImagePos.x;
                startpos.y = StartStartImagePos.y;
                startpos.z = StartStartImagePos.z;
                StartImagePos.position = startpos;

                isStart = true;
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                Vector3 startpos;
                startpos.x = StartHammerPos.x;
                startpos.y = StartHammerPos.y - 0.165f;
                startpos.z = StartHammerPos.z;
                HammerPos.position = startpos;

                startpos.x = StartBackImagePos.x + 0.08f;
                startpos.y = StartBackImagePos.y;
                startpos.z = StartBackImagePos.z;
                BackImagePos.position = startpos;

                startpos.x = StartStartImagePos.x - 0.08f;
                startpos.y = StartStartImagePos.y;
                startpos.z = StartStartImagePos.z;
                StartImagePos.position = startpos;

                isStart = false;
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
        LoadSceneFade.SetTexture(currentWorld, currentStage);
        if (asyncOperation == null) 
            asyncOperation = SceneManager.LoadSceneAsync(stage_name);
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
}
