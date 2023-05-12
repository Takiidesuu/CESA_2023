using UnityEngine;

public class LightIntensityTransition : MonoBehaviour
{
    public float IntensityMin = 0f;     // 変化の最小値
    public float IntensityMax = 1f;     // 変化の最大値
    public float TransitionSpeed = 1f;  // 変化の速度

    private Light _light;              // アタッチされたLightコンポーネント
    private bool _isIncreasing = true; // 現在の変化の向き

    private void Start()
    {
        // アタッチされたLightコンポーネントを取得
        _light = GetComponent<Light>();
    }

    private void Update()
    {
        // 現在のIntensityを取得
        float currentIntensity = _light.intensity;

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
        _light.intensity = currentIntensity;
    }
}
