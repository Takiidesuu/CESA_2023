using UnityEngine;

public class CircleObject : MonoBehaviour
{
    public float speed = 2f; // 周回するオブジェクトの速度
    private GameObject targetObject; // 接触したオブジェクトを参照する変数
    private MeshCollider meshCollider; // メッシュコライダーを参照する変数
    private Vector3[] vertices; // メッシュの頂点を格納する配列
    private int currentVertexIndex; // 現在の頂点のインデックス

    void Start()
    {
        meshCollider = GetComponent<MeshCollider>(); // MeshColliderを取得する
        vertices = meshCollider.sharedMesh.vertices; // メッシュの頂点を取得する
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Target")) // 接触したオブジェクトがTargetタグを持っている場合
        {
            targetObject = other.gameObject;
            transform.position = targetObject.transform.position; // 周回するオブジェクトを接触したオブジェクトの座標に配置する

            // メッシュの頂点から、周回するオブジェクトが最も近い頂点を探す
            float minDistance = float.MaxValue;
            for (int i = 0; i < vertices.Length; i++)
            {
                float distance = Vector3.Distance(transform.position, vertices[i]);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    currentVertexIndex = i;
                }
            }
        }
    }

    void Update()
    {
        if (targetObject != null) // 接触したオブジェクトがある場合
        {

            // 現在の頂点から、次の頂点の方向を決定する
            Vector3 direction = vertices[(currentVertexIndex + 1) % vertices.Length] - vertices[currentVertexIndex];
            if (Vector3.Distance(transform.position, vertices[(currentVertexIndex + 1) % vertices.Length]) < 0.1f)
            {
                // 次の頂点が現在の頂点と同じでない場合のみ、移動方向を計算する
                if (vertices[(currentVertexIndex + 1) % vertices.Length] != vertices[currentVertexIndex])
                {
                    direction = (vertices[(currentVertexIndex + 1) % vertices.Length] - vertices[currentVertexIndex]).normalized;
                }
                currentVertexIndex = (currentVertexIndex + 1) % vertices.Length;
            }
            direction.Normalize(); // 方向を正規化する

            // 周回するオブジェクトをメッシュの頂点に向かって移動させる
            transform.position += direction * speed * Time.deltaTime;  // 周回するオブジェクトが次の頂点に到達した場合、頂点のインデックスを更新する
            if (Vector3.Distance(transform.position, vertices[(currentVertexIndex + 1) % vertices.Length]) < 0.1f)
            {
                currentVertexIndex = (currentVertexIndex + 1) % vertices.Length;
            }
        }
    }
}