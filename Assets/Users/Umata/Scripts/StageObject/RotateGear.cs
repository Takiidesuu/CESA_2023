using UnityEngine;

public class RotateGear : MonoBehaviour
{
    public float RotateSpeed = 10.0f; // 回転スピード
    public bool IsToggle = false; // トグルフラグ
    public float RotateAngle = 90.0f; // 回転角度
    public float ToggleInterval = 1.0f; // トグルの間隔
    public float ToggleDuration = 1.0f; // トグルの回転時間

    //初期パラメータを参照した実際の変数

    private float m_RotateSpeed = 10.0f; // 回転スピード
    private float m_RotateAngle = 90.0f; // 回転角度
    private float m_ToggleInterval = 1.0f; // トグルの間隔
    private float m_ToggleDuration = 1.0f; // トグルの回転時間
                  
    private float toggleTimer = 0.0f; // トグル用タイマー
    private float toggleDurationTimer = 0.0f; // トグル用回転時間タイマー
    private bool isToggling = false; // トグル中フラグ
    private Quaternion toggleRotation; // トグル用回転クオータニオン

    void Update()
    {
        // Y軸に回転
        transform.Rotate(Vector3.up * Time.deltaTime * RotateSpeed);

        // トグルがONの場合
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

    // トグル回転を開始する
    void StartToggle()
    {
        toggleRotation = transform.rotation * Quaternion.Euler(Vector3.up * RotateAngle);
        isToggling = true;
        toggleDurationTimer = 0.0f;
    }
    public void ChangeGearMode(float ComplateRate)
    {
        if(ComplateRate > 0.8)
        {
            IsToggle = false;
            m_RotateSpeed = RotateSpeed;
        }
        else
        {
            IsToggle = true;
            m_RotateAngle = 10 + (RotateAngle * ComplateRate);
            m_ToggleDuration = 1 +(ToggleDuration * ComplateRate);
            m_ToggleInterval = 1 +(ToggleInterval * (1 -ComplateRate));
        }
    }
        
}
