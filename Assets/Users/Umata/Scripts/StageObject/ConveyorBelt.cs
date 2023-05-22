using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    public float speed = 1.0f; // ベルトコンベアの速度
    public Transform MoveDir;  //ベルトコンベアの向き
    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag("BackBuilding"))
        {
            // オブジェクトを輸送する
            MoveObject(other.gameObject);
        }
    }

    private void MoveObject(GameObject obj)
    {
        // オブジェクトを指定した速度と方向で輸送する
        obj.transform.Translate(MoveDir.forward * speed * Time.deltaTime);
    }
}
