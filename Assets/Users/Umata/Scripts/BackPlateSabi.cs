using UnityEngine;

public class BackPlateSabi : MonoBehaviour
{
    public int BulbNum;
    public int BulbActive;

    public GameObject Canvas;

    private Material material;

    private float targetSimulationFactor = 1;
    private float currentSimulationFactor = 1;
    private float changeSpeed = 2.5f; // イージングの速度を調整するパラメータ
    private MaterialPropertyBlock propertyBlock;

    private void Start()
    {
        Canvas = GameObject.Find("Canvas");
        material = transform.GetComponent<Renderer>().material;
        propertyBlock = new MaterialPropertyBlock();
    }

    private void Update()
    {
        //現在のBulbActiveを取得
        BulbActive = Canvas.GetComponent<LightBulbCollector>().LightBulb_active;
        BulbNum = Canvas.GetComponent<LightBulbCollector>().LightBulb_num;

        // SimulationFactorの計算
        float newSimulationFactor = Mathf.Clamp01(1 - ((float)BulbActive / BulbNum));

        // イージングによる変化
        if (newSimulationFactor != targetSimulationFactor)
        {
            targetSimulationFactor = newSimulationFactor;
        }

        currentSimulationFactor = Mathf.Lerp(currentSimulationFactor, targetSimulationFactor, changeSpeed * Time.deltaTime);

        // Materialのプロパティを設定
        material.SetFloat("_Progress", currentSimulationFactor);
        transform.GetComponent<Renderer>().SetPropertyBlock(propertyBlock);
    }
}
