using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageBackgroundManager : MonoBehaviour
{
    public RotateGear[] GearObjects; //生成した電球配列
    public GameObject[] SmokeObjects; //生成した電球配列

    public float AddTime;
    public float startDensity = 0.001f;
    public float endDensity = 0.0001f;
    public float duration = 5.0f;

    private float fogtimer = 0.0f;
    private GameObject Canvas;

    private float ComplateRate = 0;

    // Start is called before the first frame update
    void Start()
    {
        GearObjects = GetComponentsInChildren<RotateGear>(true);
        SmokeObjects = GameObject.FindGameObjectsWithTag("SmokeEffect");

        Canvas = GameObject.Find("Canvas");

        //煙エフェクト調整
        for (int i = 0; i < SmokeObjects.Length; i++)
        {
            //初期値設定
            //SmokeObjects[i].GetComponent<ParticleSystem>().maxParticles = 10;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //背景ギアの操作
        float SecondComplateRate = (float)Canvas.GetComponent<LightBulbCollector>().LightBulb_active / (float)Canvas.GetComponent<LightBulbCollector>().LightBulb_num;

        if (ComplateRate != SecondComplateRate)
        {
            ChangeGearsMove();
            ChangeSmokeEffect();
        }
        //クリア時に霧を晴らす
        if (ComplateRate == 1)
        {
            fogtimer += Time.deltaTime;
            float lerp = Mathf.Clamp01(fogtimer / duration);
            float currentDensity = Mathf.Lerp(startDensity, endDensity, lerp);

            RenderSettings.fogDensity = currentDensity;
        }

    }

    //歯車の移動量を変更
    public void ChangeGearsMove()
    {
        float SecondComplateRate = (float)Canvas.GetComponent<LightBulbCollector>().LightBulb_active / (float)Canvas.GetComponent<LightBulbCollector>().LightBulb_num;
        for (int i = 0; i < GearObjects.Length; i++)
        {
            GearObjects[i].ChangeGearMode(SecondComplateRate, SecondComplateRate - ComplateRate, AddTime);
        }
        ComplateRate = (float)Canvas.GetComponent<LightBulbCollector>().LightBulb_active / (float)Canvas.GetComponent<LightBulbCollector>().LightBulb_num;
    }
    public void ChangeSmokeEffect()
    {
        float ParticleMagnification = (float)Canvas.GetComponent<LightBulbCollector>().LightBulb_active / (float)Canvas.GetComponent<LightBulbCollector>().LightBulb_num;
        for (int i = 0; i < SmokeObjects.Length; i++)
        {
            SmokeObjects[i].GetComponent<ParticleSystem>().maxParticles = 10 + (int)(100 * ParticleMagnification);
        }
    }

}
