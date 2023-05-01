using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageBackgroundManager : MonoBehaviour
{
    public RotateGear[] GearObjects; //ê∂ê¨ÇµÇΩìdãÖîzóÒ

    private GameObject Canvas;

    private float ComplateRate = 0;


    // Start is called before the first frame update
    void Start()
    {
        GearObjects = GetComponentsInChildren<RotateGear>(true);
        Canvas = GameObject.Find("Canvas");
        ChangeGearsMove();
    }

    // Update is called once per frame
    void Update()
    {
        float SecondComplateRate = (float)Canvas.GetComponent<LightBulbCollector>().LightBulb_active / (float)Canvas.GetComponent<LightBulbCollector>().LightBulb_num;

        if(ComplateRate != SecondComplateRate)
        {
            ChangeGearsMove();
        }
    }

    public void ChangeGearsMove()
    {
        ComplateRate = (float)Canvas.GetComponent<LightBulbCollector>().LightBulb_active / (float)Canvas.GetComponent<LightBulbCollector>().LightBulb_num;
        for (int i = 0;i < GearObjects.Length;i++)
        {
            
            GearObjects[i].ChangeGearMode(ComplateRate);
        }
    }

}
