using UnityEngine;

public class SpawnObjects : MonoBehaviour
{
    public GameObject prefab; // 生成するGameObjectのプレハブ
    public float spawnInterval = 2f; // 生成間隔（秒）

    private float timer = 0f; // タイマー

    private void Update()
    {
        // タイマーを更新
        timer += Time.deltaTime;

        // 一定間隔ごとにGameObjectを作成
        if (timer >= spawnInterval)
        {
            SpawnGameObject();
            timer = 0f; // タイマーをリセット
        }
    }

    private void SpawnGameObject()
    {
        // GameObjectを生成
        GameObject newObject = Instantiate(prefab, transform.position, Quaternion.identity);
    }
}
