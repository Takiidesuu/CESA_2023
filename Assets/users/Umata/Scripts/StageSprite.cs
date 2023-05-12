using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class StageSprite : MonoBehaviour
{
    public float rank_S, rank_A, rank_B, rank_C, CurrentScore;
    public GameObject rank_S_image, rank_A_image, rank_B_image, rank_C_image, Stage_Image;
    public GameObject text_S, text_A, text_B, text_C;
    public GameObject StageName;
    public GameObject StageSelectManager;
    public GameObject StageData;
    private WorldSelect ssmanager;
    private TextMeshPro text_S_component, text_A_component, text_B_component, text_C_component,stage_name;

    private void Start()
    {
        text_S_component = text_S.GetComponent<TextMeshPro>();
        text_A_component = text_A.GetComponent<TextMeshPro>();
        text_B_component = text_B.GetComponent<TextMeshPro>();
        text_C_component = text_C.GetComponent<TextMeshPro>();

        //ステージデータの取得
        StageData = GameObject.Find("StageData").gameObject;

        ssmanager = StageSelectManager.GetComponent<WorldSelect>();

        stage_name = StageName.GetComponent<TextMeshPro>();

    }

    private void Update()
    {
        UpdateRankImage();
    }

    private void UpdateRankImage()
    {
        rank_S = StageData.GetComponent<StageDataManager>().GetStageData(ssmanager.currentWorld, ssmanager.currentStage).rank_s_border;
        rank_A = StageData.GetComponent<StageDataManager>().GetStageData(ssmanager.currentWorld, ssmanager.currentStage).rank_a_border;
        rank_B = StageData.GetComponent<StageDataManager>().GetStageData(ssmanager.currentWorld, ssmanager.currentStage).rank_b_border;
        rank_C = StageData.GetComponent<StageDataManager>().GetStageData(ssmanager.currentWorld, ssmanager.currentStage).rank_c_border;
        CurrentScore = StageData.GetComponent<StageDataManager>().GetStageData(ssmanager.currentWorld, ssmanager.currentStage).Score;

        SetRankText(text_S_component,rank_S);
        SetRankText(text_A_component,rank_A);
        SetRankText(text_B_component,rank_B);
        SetRankText(text_C_component,rank_C);
        stage_name.text = "STAGE" + (ssmanager.currentWorld+1).ToString() + "-" + (ssmanager.currentStage + 1).ToString();
        if(CurrentScore == 0)
        {
            rank_S_image.gameObject.SetActive(false);
            rank_A_image.gameObject.SetActive(false);
            rank_B_image.gameObject.SetActive(false);
            rank_C_image.gameObject.SetActive(false);
        }
        else if (CurrentScore >= rank_S)
        {
            rank_S_image.gameObject.SetActive(true);
            rank_A_image.gameObject.SetActive(false);
            rank_B_image.gameObject.SetActive(false);
            rank_C_image.gameObject.SetActive(false);
        }
        else if (CurrentScore >= rank_A)
        {
            rank_S_image.gameObject.SetActive(false);
            rank_A_image.gameObject.SetActive(true);
            rank_B_image.gameObject.SetActive(false);
            rank_C_image.gameObject.SetActive(false);
        }
        else if (CurrentScore >= rank_B)
        {
            rank_S_image.gameObject.SetActive(false);
            rank_A_image.gameObject.SetActive(false);
            rank_B_image.gameObject.SetActive(true);
            rank_C_image.gameObject.SetActive(false);
        }
        else if (CurrentScore >= rank_C)
        {
            rank_S_image.gameObject.SetActive(false);
            rank_A_image.gameObject.SetActive(false);
            rank_B_image.gameObject.SetActive(false);
            rank_C_image.gameObject.SetActive(true);
        }
    }

    private void SetRankText(TextMeshPro textComponent,float BorderScore)
    {
        textComponent.text = BorderScore.ToString();
    }

    // CurrentScoreの変更時に呼び出されるメソッド
    public void OnCurrentScoreChanged()
    {
        UpdateRankImage();
    }
}
