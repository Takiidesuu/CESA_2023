using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    //�RD��ԃe�L�X�g
    public TextMeshPro ScoreText;
    public TextMeshPro TimeText;

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
    //��x�������s���Ȃ��悤�t���O�Ǘ�
    private bool time_countup_flg;
    private bool score_countup_flg;

    private SoundManager soundManager;

    //�A�C�e���ړ��p�X�N���v�g
    ItemMover itemmover;
    private void Start()
    {
        itemmover = gameObject.GetComponent<ItemMover>();
        TimeText = TimeTextObj.transform.GetComponent<TextMeshPro>();
        ScoreText = ScoreTextObj.transform.GetComponent<TextMeshPro>();
        timeLeft = 0;
        ScoreText.text = "" + score.ToString();
        TimeText.text = "" + timeLeft.ToString();
        counting = true;
        time_countup_flg = false;
        score_countup_flg = false;
        
        soundManager = GetComponent<SoundManager>();
        soundManager.PlaySoundEffect("ClearJingle");
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
                if(timeLeft >= ClearTime)
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

                if (InputManager.instance.press_select)
                {
                    counting = true;
                    ScoreText.text = "" + Score.ToString();
                    score_countup_flg = true;
                }
                if(score >= Score)
                {
                    score_countup_flg = true;
                }
            }
            if(score_countup_flg)
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
                //�ŏI����
                if (InputManager.instance.press_select)
                {
                    int current_world = StageDataManager.instance.now_world + 1;
                    int current_stage = StageDataManager.instance.now_stage + 1;

                    string world_to_load = "StageSelect";

                    bool change_to_select = false;

                    if (current_stage < 5)
                    {
                        current_stage++;
                    }
                    else
                    {
                        if (current_world < 4)
                        {
                            current_world++;
                            current_stage = 1;
                        }
                        else
                        {
                            change_to_select = true;
                        }
                    }

                    if (change_to_select)
                    {
                        world_to_load = "StageSelect";
                    }
                    else
                    {
                        world_to_load = "Stage" + current_world + "-" + current_stage;
                    }

                    SceneManager.LoadScene(world_to_load);
                }
            }
        }
    }
}