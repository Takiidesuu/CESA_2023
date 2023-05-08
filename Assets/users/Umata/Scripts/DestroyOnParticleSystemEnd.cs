using UnityEngine;

public class DestroyOnParticleSystemEnd : MonoBehaviour
{
    [SerializeField] private float lifetime = 5f; // パーティクルシステムが削除されるまでの時間

    private ParticleSystem particleSystem;
    private float timer = 0;

    private void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }

    private void Update()
    {

        timer += Time.deltaTime; // タイマーを加算する

        // パーティクルシステムが再生されていない場合、指定時間が経過したらGameObjectを削除する
        if (timer >= lifetime)
            Destroy(gameObject);

        // ParticleSystemが再生中かどうかを確認する
        if (particleSystem.isPlaying)
        {
            return;
        }
        Destroy(gameObject);

    }
}
