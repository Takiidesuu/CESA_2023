using UnityEngine;

public class TitleLighting : MonoBehaviour
{
    public GameObject spotLightObject; // SpotLightをアタッチするオブジェクト
    public float targetIntensity = 5f; // イージング先のIntensity
    public float time = 2.0f; // イージングにかける時間

    public bool isShining = false;
    private Light spotLight;
    private float initialIntensity;
    private float currentIntensity;
    private float startTime;

    public float IntensityMin = 0f;     // 変化の最小値
    public float IntensityMax = 1f;     // 変化の最大値
    public float TransitionSpeed = 1f;  // 変化の速度
    private bool _isIncreasing = true; // 現在の変化


    private void Start()
    {
        spotLight = spotLightObject.GetComponent<Light>();
        initialIntensity = spotLight.intensity;
        currentIntensity = initialIntensity;
    }

    private void Update()
    {
        if (isShining)
        {
            if (currentIntensity < targetIntensity)
            {
                // イージング処理
                float elapsedTime = Time.time - startTime;
                float t = Mathf.Clamp01(elapsedTime / time);
                currentIntensity = Mathf.Lerp(initialIntensity, targetIntensity, t);
                spotLight.intensity = currentIntensity;
            }
            else
            {

                // 現在のIntensityを取得
                float currentIntensity = spotLight.intensity;

                // 変化の向きに応じてIntensityを増減させる
                if (_isIncreasing)
                {
                    currentIntensity += TransitionSpeed * Time.deltaTime;
                    if (currentIntensity >= IntensityMax)
                    {
                        currentIntensity = IntensityMax;
                        _isIncreasing = false;
                    }
                }
                else
                {
                    currentIntensity -= TransitionSpeed * Time.deltaTime;
                    if (currentIntensity <= IntensityMin)
                    {
                        currentIntensity = IntensityMin;
                        _isIncreasing = true;
                    }
                }

                // 変更したIntensityを反映
                spotLight.intensity = currentIntensity;
            }
        }
    }

    public void StartShining()
    {
        isShining = true;
        startTime = Time.time;
    }

    public void StopShining()
    {
        isShining = false;
        spotLight.intensity = initialIntensity;
        currentIntensity = initialIntensity;
    }
}
