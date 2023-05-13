using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverScript : MonoBehaviour
{
    //WorldSelect worldSelectScript = gameObject.GetComponent<WorldSelect>();
    //StageDataManager stageDataManagerScript:
    int load_world;
    int load_stage;
    StageDataManager loadStageData;

    // State管理変数
    enum Status
    {
        Select,
        Load,
        Loading
    }
    Status now_state;

    // Retry 
    [SerializeField] Image retry;
    [SerializeField] Image stage_select;

    // 初期Pps
    Vector3 retry_defaultPos;
    Vector3 stageselect_defaultPos;

    // 移動量(調整可)
    public Vector3 selectPosOffset;

    // Retry選択中かどうか
    bool is_retry;


    private void Awake()
    {
        retry_defaultPos = retry.rectTransform.localPosition;
        stageselect_defaultPos = stage_select.rectTransform.localPosition;
        retry.rectTransform.localPosition += selectPosOffset;
        is_retry = true;
        now_state = Status.Select;
    }
    // Start is called before the first frame update
    void Start()
    {
        loadStageData = GameObject.Find("StageData").GetComponent<StageDataManager>();
        load_world = loadStageData.now_world +1;
        load_stage = loadStageData.now_stage + 1;
        Debug.Log(load_world);
        Debug.Log(load_stage);
    }

    // Update is called once per frame
    void Update()
    {
        switch(now_state)
        {
            case Status.Select:

                StatusSelect();
                break;

            case Status.Load:
                SceneChange(is_retry);
                break;

            case Status.Loading:
                break;  
        }

    }
    public void LoadRetryStage()
    {
        string load_stage_name = "Stage" + load_world + "-" + load_stage;
        SceneManager.LoadScene(load_stage_name);
    }


    private void SceneChange(bool isRetry)
    {
        string stagename = "StageSelect";
        
        if (isRetry)
        {// retryが選択されているなら
            //stagename = SceneManager.GetActiveScene().name;
            stagename = "Stage" + load_world + "-" + load_stage;
        }
        // SceneLoading
        FindObjectOfType<SceneController>().SceneChange(stagename);
        now_state = Status.Loading;
    }

    private void StatusSelect()
    {
        InputManager.instance.GetMenuMoveFloat();
        if (InputManager.instance.GetMenuMoveFloat() < 0)
        {
            retry.rectTransform.localPosition = retry_defaultPos + selectPosOffset;
            stage_select.rectTransform.localPosition = stageselect_defaultPos;
            is_retry = true;
        }
        if (InputManager.instance.GetMenuMoveFloat() > 0)
        {
            retry.rectTransform.localPosition = retry_defaultPos;
            stage_select.rectTransform.localPosition = stageselect_defaultPos + selectPosOffset;
            is_retry = false;
        }

        if (InputManager.instance.press_select)
        {
            now_state = Status.Load;
        }
    }
}