using UnityEngine;

public class EmissionMapEasing : MonoBehaviour
{
    public float intensityRange = 3f; // インテンシティの範囲
    public float easingSpeed = 1f; // イージングの速度

    private Material material;
    private float targetIntensity; // イージングの終点
    private float currentVelocity; // スムーズダンプ用の現在の速度
    private bool increasing = true; // インテンシティを増やすかどうかのフラグ

    void Start()
    {
        material = GetComponent<Renderer>().material;
        targetIntensity = intensityRange; // 最初は最大値
    }

    void Update()
    {
        float currentIntensity = material.GetColor("_EmissionColor").r; // 現在のインテンシティを取得
        float newIntensity = Mathf.SmoothDamp(currentIntensity, targetIntensity, ref currentVelocity, easingSpeed); // スムーズダンプを使って新しいインテンシティを計算
        Color newEmissionColor = new Color(newIntensity, newIntensity, newIntensity, 1f); // 新しいエミッションカラーを作成
        material.SetColor("_EmissionColor", newEmissionColor); // マテリアルにエミッションカラーを設定

        if (Mathf.Abs(currentIntensity - targetIntensity) <= 0.01f) // 現在のインテンシティが目標値に近づいたら、目標値を反転させる
        {
            increasing = !increasing;
            targetIntensity = increasing ? intensityRange : 0f;
        }
    }
}
