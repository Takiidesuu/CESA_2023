using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistortionShock : MonoBehaviour
{
    public float time = 2f; // 消滅までの時間
    public Vector3 size = new Vector3(0.1f, 0.1f, 0.1f); // 加算するスケールの値

    private float elapsedTime = 0f; // 経過時

    private void Update()
    {
        // 経過時間を更新
        elapsedTime += Time.deltaTime;

        // 時間が経過したらオブジェクトを破棄する
        if (elapsedTime >= time)
        {
            Destroy(gameObject);
        }

        // スケールを加算する
        transform.localScale += size * Time.deltaTime;
    }
}
