using UnityEngine;

//ステージデータ
[System.Serializable]
public struct StageData
{
    public int rank_s_border;
    public int rank_a_border;
    public int rank_b_border;
    public int rank_c_border;
    public int Score;
}

//ワールドデータ
[System.Serializable]
public struct WorldData
{
    public StageData[] stages;
}

[System.Serializable]
public class StageDataManager : MonoBehaviour
{
    public WorldData[] worlds;
    public int now_world;
    public int now_stage;

    public static StageDataManager instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        now_stage = -1;
        now_world = -1;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        SetCurrentWorldStage();
    }

    public StageData GetCurrentStageData()
    {
        StageData currentStageData = worlds[now_world].stages[now_stage];
        return currentStageData;
    }
    public StageData GetStageData(int world,int stage)
    {
        StageData currentStageData = worlds[world].stages[stage];
        return currentStageData;
    }

    public void SetStageScore(int score)
    {
        worlds[now_world].stages[now_stage].Score = score;
    }
    public void SetCurrentWorldStage()
    {
        int world, stage;
        StageUtilitys.GetCurrentWorldAndStage(out world,out stage);
        if (world != 9999 && stage != 9999)
        {
            now_world = world;
            now_stage = stage;
        }
    }
    
    public void ResetData()
    {
        for (int i = 0; i < worlds.Length; i++)
        {
            for (int a = 0; a < worlds[i].stages.Length; a++)
            {
                worlds[i].stages[a].rank_a_border = 0;
                worlds[i].stages[a].rank_b_border = 0;
                worlds[i].stages[a].rank_c_border = 0;
                worlds[i].stages[a].rank_s_border = 0;
                worlds[i].stages[a].Score = 0;
            }
        }
    }
}