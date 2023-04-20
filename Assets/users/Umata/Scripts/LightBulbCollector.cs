using UnityEngine;

public class LightBulbCollector : MonoBehaviour
{
    public LightBulb[] lightBulb;

    public int LightBulb_num;
    public int LightBulb_active;
    public GameObject[] lightBulbs;

    void Start()
    {
        // タグが "LightBulb" のオブジェクトを検索し、配列に格納する
        lightBulbs = GameObject.FindGameObjectsWithTag("LightBulb");
        LightBulb_num = lightBulbs.Length;
        lightBulb = new LightBulb[LightBulb_num];

        for(int i = 0;i < LightBulb_num;i++)
        {
            lightBulb[i] = lightBulbs[i].GetComponent<LightBulb>();
        }
    }
    private void Update()
    {
        LightBulb_active = 0;
     for(int i = 0; i< LightBulb_num;i++)
        {
            if(lightBulb[i].is_stage_hit)
            {
                LightBulb_active++;
            }
        }
    }
}
