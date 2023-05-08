using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldSelect : MonoBehaviour
{
    // ワールドとステージの数
    public int numWorlds = 4;
    public int numStages = 6;

    // カメラの移動に使用する時間
    public float time = 1f;

    // 選択中のワールドとステージ
    private int currentWorld = 0;
    private int currentStage = 0;

    // カメラの移動先の位置と回転
    public Transform worldSelectPos;
    public Transform stageSelectPos;

    public GameObject CameraObj;

    private bool selectingWorld = true; // ワールドを選択中かどうか

    void Start()
    {
        // 初期位置にカメラを移動
        CameraObj.transform.position = worldSelectPos.position;
        CameraObj.transform.rotation = worldSelectPos.rotation;
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
                // ステージを選択中の場合、シーンをロード
                if (CameraObj.transform.position.x - stageSelectPos.position.x < 0.05)
                {
                    LoadScene();
                }
            }
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

        // カメラを移動する
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

    // カメラを移動する
    private void MoveCamera(Transform targetPos)
    {
        CameraObj.transform.position = Vector3.Lerp(CameraObj.transform.position, targetPos.position, time * Time.deltaTime);
        CameraObj.transform.rotation = Quaternion.Lerp(CameraObj.transform.rotation, targetPos.rotation, time * Time.deltaTime);
    }

    // シーンをロードする
    private void LoadScene()
    {
        // ワールドとステージに応じてシーンをロードする処理を書く
        string stage_name = "Stage" + (currentWorld+1) + "-" + (currentStage+1);
        SceneManager.LoadScene(stage_name);
    }
}
