using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public TextMeshPro ScoreText;
    public TextMeshPro TimeText;
    public GameObject rank_s;
    public GameObject rank_a;
    public GameObject rank_b;
    public GameObject rank_c;
    public int border_s;
    public int border_a;
    public int border_b;
    public int border_c;
    public int CountUpTime;
    public int ClearTime = 60;
    public int Score = 100;
    private bool counting;
    private float timeLeft;
    private int score;
    private float elapsedtime;
    public GameObject TimeTextObj;
    public GameObject ScoreTextObj;

    private void Start()
    {
        TimeText = TimeTextObj.transform.GetComponent<TextMeshPro>();
        ScoreText = ScoreTextObj.transform.GetComponent<TextMeshPro>();
        timeLeft = 0;
        ScoreText.text = "" + score.ToString();
        TimeText.text = "" + timeLeft.ToString();
        counting = true;
    }

    private void Update()
    {
        elapsedtime += Time.deltaTime;
        if (counting)
        {
            
            timeLeft = Mathf.RoundToInt(elapsedtime / CountUpTime * ClearTime);
            timeLeft = Mathf.Min(timeLeft, ClearTime);
            TimeText.text = "" + Mathf.RoundToInt(timeLeft).ToString();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                counting = false;
                TimeText.text = "" + Mathf.RoundToInt(timeLeft).ToString();
                score = 0;
                elapsedtime = 0;
            }
        }
        else
        {
            score = Mathf.RoundToInt(elapsedtime / CountUpTime * Score);
            score = Mathf.Min(score,Score);

            ScoreText.text = "" + score.ToString();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                counting = true;
                ScoreText.text = "" + Score.ToString();

                if (score >= border_s)
                {
                    rank_s.gameObject.SetActive(true);
                    rank_a.gameObject.SetActive(false);
                    rank_b.gameObject.SetActive(false);
                    rank_c.gameObject.SetActive(false);
                }
                else if (score >= border_a)
                {
                    rank_s.gameObject.SetActive(false);
                    rank_a.gameObject.SetActive(true);
                    rank_b.gameObject.SetActive(false);
                    rank_c.gameObject.SetActive(false);
                }
                else if (score >= border_b)
                {
                    rank_s.gameObject.SetActive(false);
                    rank_a.gameObject.SetActive(false);
                    rank_b.gameObject.SetActive(true);
                    rank_c.gameObject.SetActive(false);
                }
                else if (score >= border_c)
                {
                    rank_s.gameObject.SetActive(false);
                    rank_a.gameObject.SetActive(false);
                    rank_b.gameObject.SetActive(false);
                    rank_c.gameObject.SetActive(true);
                }
            }
        }
    }
}