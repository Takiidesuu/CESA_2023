using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageBackgroundManager : MonoBehaviour
{
    public RotateGear[] GearObjects; //生成した電球配列
    public GameObject[] SmokeObjects; //生成した電球配列
    public ElectricRoads electricRoads;

    public float AddTime;
    public float startDensity = 0.001f;
    public float endDensity = 0.0001f;
    public float duration = 5.0f;

    private float fogtimer = 0.0f;
    private GameObject Canvas;

    private float ComplateRate = 0;
    private SoundManager SoundManager;

    //FOGカラー
    public Color StartFogColor;
    public Color ClearFogColor;

    // Start is called before the first frame update
    void Start()
    {
        //FogColorを変更
        RenderSettings.fogColor = StartFogColor;

        GearObjects = GetComponentsInChildren<RotateGear>(true);
        SmokeObjects = GameObject.FindGameObjectsWithTag("SmokeEffect");

        Canvas = GameObject.Find("Canvas");
        SoundManager = gameObject.GetComponent<SoundManager>();
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
            if (electricRoads != null)
            {
                if(ComplateRate < SecondComplateRate)
                    electricRoads.SpeedUp();
            }
            ChangeGearsMove();
            ChangeSmokeEffect();
        }
        //クリア時に霧を晴らす
        if (ComplateRate == 1)
        {
            if (electricRoads != null)
            {
                electricRoads.SpeedUp();
            }

            for (int i = 0; i < GearObjects.Length; i++)
            {
                GearObjects[i].ChangeGearMaterial();
            }
            fogtimer += Time.deltaTime;
            float lerp = Mathf.Clamp01(fogtimer / duration);
            float currentDensity = Mathf.Lerp(startDensity, endDensity, lerp);
            Color currentFogColor;
            currentFogColor.r = Mathf.Lerp(StartFogColor.r, ClearFogColor.r, lerp);
            currentFogColor.g = Mathf.Lerp(StartFogColor.g, ClearFogColor.g, lerp);
            currentFogColor.b = Mathf.Lerp(StartFogColor.b, ClearFogColor.b, lerp);
            currentFogColor.a = 1.0f;

            RenderSettings.fogColor = currentFogColor;
            RenderSettings.fogDensity = currentDensity;
        }

    }

    //歯車の移動量を変更
    public void ChangeGearsMove()
    {
        float SecondComplateRate = (float)Canvas.GetComponent<LightBulbCollector>().LightBulb_active / (float)Canvas.GetComponent<LightBulbCollector>().LightBulb_num;
        //SE再生
        
        if (StageDataManager.instance.now_world + 1 != 3)
        {
            if(SecondComplateRate - ComplateRate > 0)
            {
                SoundManager.PlaySoundEffect("GearMove");
            }
            else
            {
                SoundManager.PlaySoundEffect("ReverseGear");
            }
        }
        
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
