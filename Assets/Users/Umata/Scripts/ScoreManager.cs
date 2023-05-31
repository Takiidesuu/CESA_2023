using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ScoreManager : MonoBehaviour
{
    //�RD��ԃe�L�X�g
    public TextMeshPro ScoreText;
    public TextMeshPro TimeText;
    public TextMeshPro HiScoreText;


    public string NextSceneName;    //���V�[���̃e�L�X�g

    //�����N�摜
    public GameObject rank_s;
    public GameObject rank_a;
    public GameObject rank_b;
    public GameObject rank_c;
    public GameObject rank_pos;
    //�����N�{�[�_�[
    public int border_s;
    public int border_a;
    public int border_b;
    public int border_c;
    public float rank_push_time;
    //�J�E���g�A�b�v����
    public int CountUpTime;
    public int ClearTime = 60;
    public int Score = 100;
    private bool counting;
    private float timeLeft;
    private int score;
    private float elapsedtime;
    public GameObject TimeTextObj;
    public GameObject ScoreTextObj;
    public GameObject HiScoreTextObj;
    //��x�������s���Ȃ��悤�t���O�Ǘ�
    private bool time_countup_flg;
    private bool score_countup_flg;

    private SoundManager soundManager;

    /// <summary>
    /// ひらた
    /// </summary>
    Camera MainCamera;
    RawImage rawImage;
    RenderTexture renderTexture;
    AsyncOperation asyncOperation;//進捗チェック
    LoadSceneFade LoadSceneFade;
    public GameObject Fade;

    public GameObject postProcessVolume;
    private Vignette vignette;

    [SerializeField] private AnimationCurve curve = AnimationCurve.Linear(0f, 0f, 1f, 1f); // イージングカーブ
    public float TransitionStartDelay;
    private float startTime;
    private float duration = 15;
    private float BlackPanelDuration = 20;
    private float targetIntensity = 1;
    private RawImage BlackPanel;
    private bool IsBlackPanel;
    string world_to_load = "";

    int current_world = -1;
    int current_stage = -1;

    //�A�C�e���ړ��p�X�N���v�g
    ItemMover itemmover;
    
    public bool isTransition()
    {
        return IsBlackPanel;
    }

    private void Start()
    {
        if (StageDataManager.instance != null)
        {
            current_world = StageDataManager.instance.now_world;
            current_stage = StageDataManager.instance.now_stage;
        }
        rawImage = GameObject.Find("ResultWindow").GetComponent<RawImage>();
        renderTexture = (RenderTexture)Resources.Load("MainCamera");
        MainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        postProcessVolume.transform.GetComponent<Volume>().profile.TryGet(out vignette);
        LoadSceneFade = GameObject.Find("LoadSceneFade").GetComponent<LoadSceneFade>();
        BlackPanel = GameObject.Find("FadeBlackPanel").GetComponent<RawImage>();
        transform.root.GetComponent<ScoreCount>().SetScore();
        soundManager = GetComponent<SoundManager>();
        itemmover = gameObject.GetComponent<ItemMover>();
        TimeText = TimeTextObj.transform.GetComponent<TextMeshPro>();
        ScoreText = ScoreTextObj.transform.GetComponent<TextMeshPro>();
        HiScoreTextObj = GameObject.Find("HiScoreText");
        HiScoreText = HiScoreTextObj.transform.GetComponent<TextMeshPro>();
        timeLeft = 0;
        ScoreText.text = "" + score.ToString();
        TimeText.text = "" + timeLeft.ToString();
        counting = true;
        time_countup_flg = false;
        score_countup_flg = false;
        
        soundManager.PlaySoundEffect("ClearJingle");

        border_s = StageDataManager.instance.GetCurrentStageData().rank_s_border;
        border_a = StageDataManager.instance.GetCurrentStageData().rank_a_border;
        border_b = StageDataManager.instance.GetCurrentStageData().rank_b_border;
        border_c = StageDataManager.instance.GetCurrentStageData().rank_c_border;
        HiScoreText.text = StageDataManager.instance.GetCurrentStageData().Score.ToString();
        int nowstage = StageDataManager.instance.now_stage;
        int nowworld = StageDataManager.instance.now_world;
        if (StageDataManager.instance.GetCurrentStageData().Score < Score)
            StageDataManager.instance.worlds[nowworld].stages[nowstage].Score = Score;
    }

    private void Update()
    {
        elapsedtime += Time.deltaTime;
        if (counting)
        {
            if (!time_countup_flg)
            {
                timeLeft = Mathf.RoundToInt(elapsedtime / CountUpTime * ClearTime);
                timeLeft = Mathf.Min(timeLeft, ClearTime);
                TimeText.text = "" + Mathf.RoundToInt(timeLeft).ToString();

                if (InputManager.instance.press_select)
                {
                    counting = false;
                    timeLeft = ClearTime;
                    TimeText.text = "" + Mathf.RoundToInt(timeLeft).ToString();
                    score = 0;
                    elapsedtime = 0;
                    time_countup_flg = true;
                }
                //�J�E���g�A�b�v�I��
                if (timeLeft >= ClearTime)
                {
                    time_countup_flg = true;
                    counting = false;
                    score = 0;
                    elapsedtime = 0;
                    time_countup_flg = true;
                }
            }
        }
        else
        {
            if (time_countup_flg && !score_countup_flg)
            {
                score = Mathf.RoundToInt(elapsedtime / CountUpTime * Score);
                score = Mathf.Min(score, Score);

                ScoreText.text = "" + score.ToString();
                if (int.Parse(HiScoreText.text) < score)
                    HiScoreText.text = score.ToString();

                if (InputManager.instance.press_select)
                {
                    counting = true;
                    ScoreText.text = "" + Score.ToString();
                    if (int.Parse(HiScoreText.text) < score)
                        HiScoreText.text = score.ToString();
                    score_countup_flg = true;
                }
                if (score >= Score)
                {
                    score_countup_flg = true;
                }
            }
            if (score_countup_flg)
            {
                if (score >= border_s)
                {
                    rank_s.gameObject.SetActive(true);
                    rank_a.gameObject.SetActive(false);
                    rank_b.gameObject.SetActive(false);
                    rank_c.gameObject.SetActive(false);
                    itemmover.MoveItem(rank_s, rank_push_time, rank_pos.transform.position);
                }
                else if (score >= border_a)
                {
                    rank_s.gameObject.SetActive(false);
                    rank_a.gameObject.SetActive(true);
                    rank_b.gameObject.SetActive(false);
                    rank_c.gameObject.SetActive(false);
                    itemmover.MoveItem(rank_a, rank_push_time, rank_pos.transform.position);
                }
                else if (score >= border_b)
                {
                    rank_s.gameObject.SetActive(false);
                    rank_a.gameObject.SetActive(false);
                    rank_b.gameObject.SetActive(true);
                    rank_c.gameObject.SetActive(false);
                    itemmover.MoveItem(rank_b, rank_push_time, rank_pos.transform.position);

                }
                else if (score >= border_c)
                {
                    rank_s.gameObject.SetActive(false);
                    rank_a.gameObject.SetActive(false);
                    rank_b.gameObject.SetActive(false);
                    rank_c.gameObject.SetActive(true);
                    itemmover.MoveItem(rank_c, rank_push_time, rank_pos.transform.position);

                }
                if (!IsBlackPanel)
                {
                    if (InputManager.instance.press_select)
                    {

                        current_world = StageDataManager.instance.now_world;
                        current_stage = StageDataManager.instance.now_stage;
                        

                        bool change_to_select = false;

                        if (current_stage < 4)
                        {
                            current_stage++;
                        }
                        else
                        {
                            //if (current_world < 3)
                            //{
                                current_world++;
                                current_stage = 0;
                            //}
                            //else
                            //{
                            change_to_select = true;
                            //}
                        }

                        if (change_to_select)
                        {
                            world_to_load = "StageSelect";
                        }
                        else
                        {
                            world_to_load = "Stage" + (current_world + 1) + "-" + (current_stage + 1);
                        }

                        IsBlackPanel = true;
                        MainCamera.targetTexture = renderTexture;
                        rawImage.enabled = false;
                        startTime = Time.time;
                        postProcessVolume.SetActive(true);
                    }
                }
            }
        }
        if (IsBlackPanel)
        {
            TransitionStartDelay += Time.deltaTime;


                float t = (Time.time - startTime) / (duration / 12);
                float intensity = Mathf.Lerp(vignette.intensity.value, targetIntensity, t * t);

                // 値を設定
                vignette.intensity.value = intensity;

                float elapsedTime = Time.time - startTime;
                float progress = Mathf.Clamp01(elapsedTime / (BlackPanelDuration / 12));

                float alpha = curve.Evaluate(progress);
                // Imageのカラーを更新する
                Color color = BlackPanel.color;
                BlackPanel.color = new Color(color.r, color.g, color.b, alpha);
            

            if (BlackPanel.color.a >= 1)
            {
                IsBlackPanel = false;

                if (asyncOperation == null)
                {
                    postProcessVolume.SetActive(false);
                    Fade.SetActive(true);
                    asyncOperation = SceneManager.LoadSceneAsync(world_to_load);
                    LoadSceneFade.SetTexture(current_world, current_stage);
                }
                asyncOperation.allowSceneActivation = false;
            }
        }

        if (asyncOperation != null)
            if (LoadSceneFade.SpliteOnceMove(asyncOperation.progress) > 0.9f)
                asyncOperation.allowSceneActivation = true;
    }
}