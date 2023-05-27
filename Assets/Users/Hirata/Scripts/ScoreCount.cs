using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCount : MonoBehaviour
{
    private ScoreManager scoreManager;
    private DamageScript damageScript;

    private int BaseTiem = 300;
    private float CurrentTime;
    private int Coefficient = 10;
    private int HPCoefficient = 500;

    // Start is called before the first frame update
    void Start()
    {
        scoreManager = transform.GetChild(0).GetChild(0).GetComponent<ScoreManager>();
        damageScript = GameObject.Find("Player").GetComponent<DamageScript>();
    }

    // Update is called once per frame
    void Update()
    {
        CurrentTime += Time.deltaTime;
    }

    public void SetScore()
    {
        int bonus = damageScript.GetHitPoint() * HPCoefficient;
        scoreManager.Score = (BaseTiem - (int)CurrentTime) * Coefficient + bonus;
        scoreManager.ClearTime = (int)CurrentTime;

        int nowstage = StageDataManager.instance.now_stage;
        int nowworld = StageDataManager.instance.now_world;
        if (StageDataManager.instance.worlds[nowworld].stages[nowstage].Score < scoreManager.Score)
            StageDataManager.instance.worlds[nowworld].stages[nowstage].Score = scoreManager.Score;
    }
}
