using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RotateGear : MonoBehaviour
{
    public float RotateSpeed = 10.0f; // 回転スピード
    public bool IsToggle = false; // トグルフラグ
    public float RotateAngle = 90.0f; // 回転角度
    public float ToggleInterval = 1.0f; // トグルの間隔
    public float ToggleDuration = 1.0f; // トグルの回転時間
    public GameObject SabiEffect;    //錆落下エフェクト
    public Material CleanMaterial;
    //初期パラメータを参照した実際の変数

    private float m_RotateSpeed = 10.0f; // 回転スピード
    private float m_RotateAngle = 90.0f; // 回転角度
    private float m_ToggleInterval = 1.0f; // トグルの間隔
    private float m_ToggleDuration = 1.0f; // トグルの回転時間

    private float toggleTimer = 0.0f; // トグル用タイマー
    private float toggleDurationTimer = 0.0f; // トグル用回転時間タイマー
    private bool isToggling = false; // トグル中フラグ
    private Quaternion toggleRotation; // トグル用回転クオータニオン
    private List<GearParam> GearRotateVector = new List<GearParam>();

    struct GearParam
    {
        public float Timer;
        public float Speed;
    }

    void Start()
    {
        m_RotateSpeed = RotateSpeed * 0.2f;
    }

    void Update()
    {
        if (GearRotateVector.Count > 0)
        {
            //時間切れの場合削除
            if (GearRotateVector[0].Timer <= 0)
            {
                GearRotateVector.RemoveAt(0);
                return;
            }
            //回転待機列がある場合優先実行
            GearParam gearParam = GearRotateVector[0];
            transform.Rotate(Vector3.up * Time.deltaTime * m_RotateSpeed * gearParam.Speed * 50);
            gearParam.Timer -= Time.deltaTime;
            GearRotateVector[0] = gearParam;
        }
        else
        {
            // Y軸に回転
            transform.Rotate(Vector3.up * Time.deltaTime * m_RotateSpeed);

            //トグルがONの場合
            if (IsToggle)
            {
                toggleTimer += Time.deltaTime;

                if (toggleTimer >= ToggleInterval)
                {
                    StartToggle();
                    toggleTimer = 0.0f;
                }
            }

            // トグル中の場合
            if (isToggling)
            {
                toggleDurationTimer += Time.deltaTime;
                if (toggleDurationTimer < ToggleDuration)
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, toggleRotation, toggleDurationTimer / ToggleDuration);
                }
                if (toggleDurationTimer >= ToggleDuration)
                {
                    isToggling = false;
                }
            }
        }
    }

    // トグル回転を開始する
    void StartToggle()
    {
        toggleRotation = transform.rotation * Quaternion.Euler(Vector3.up * RotateAngle);
        isToggling = true;
        toggleDurationTimer = 0.0f;
    }
    public void ChangeGearMode(float ComplateRate, float difference, float time)
    {

        GearParam gear_param;
        //スコアの増減に応じて変更
        gear_param.Timer = time;
        gear_param.Speed = difference;
        GearRotateVector.Add(gear_param);
        m_RotateSpeed = RotateSpeed * ComplateRate;
        //IsToggle = true;
        //m_RotateAngle = 10 + (RotateAngle * ComplateRate);
        //m_ToggleDuration = 1 +(ToggleDuration * ComplateRate);
        //m_ToggleInterval = 1 +(ToggleInterval * (1 -ComplateRate));
    }

    public void ChangeGearMaterial()
    {
        transform.GetComponent<MeshRenderer>().material = CleanMaterial;
        SabiEffect.active = true;
    }

}
