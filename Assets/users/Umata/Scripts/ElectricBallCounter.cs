using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ElectricBallCounter : MonoBehaviour
{
    public int MaxElectricBall;
    public GameObject Text_Active_obj;
    public GameObject Text_Num_obj;

    private int ElectricBallbnum;

    private TextMeshProUGUI Text_Active;
    private TextMeshProUGUI Text_Num;



    PlayerMove player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerMove>();

        Text_Active = Text_Active_obj.GetComponent<TextMeshProUGUI>();
        Text_Num = Text_Num_obj.GetComponent<TextMeshProUGUI>();

    }

// Update is called once per frame
void Update()
    {
        if(ElectricBallbnum >= MaxElectricBall)
        {
            player.GameOver();
        }
        //テキストの更新
        Text_Active.text = "" +ElectricBallbnum.ToString();
        Text_Num.text = "" + MaxElectricBall.ToString();
    }

    public void CountUpBulb()
    {
        ElectricBallbnum++;
    }
}
