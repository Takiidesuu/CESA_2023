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
    //StageData loadStageData;

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
        //load_world = worldSelectScript.currentWorld + 1;
        //load_stage = worldSelectScript.currentStage + 1;
        //Debug.Log(load_world);
        //Debug.Log(load_stage);
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
        if(isRetry)
        {
            // world_numとstage_numからStageをLoad
            Debug.Log("Loading");
        }
        else
        {
            SceneManager.LoadScene("StageSelect");
        }

        now_state = Status.Loading;
    }

    private void StatusSelect()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            retry.rectTransform.localPosition = retry_defaultPos + selectPosOffset;
            stage_select.rectTransform.localPosition = stageselect_defaultPos;
            is_retry = true;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            retry.rectTransform.localPosition = retry_defaultPos;
            stage_select.rectTransform.localPosition = stageselect_defaultPos + selectPosOffset;
            is_retry = false;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            now_state = Status.Load;
        }
    }
}