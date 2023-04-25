using UnityEngine;

public class DestroyOnParticleSystemEnd : MonoBehaviour
{
    private ParticleSystem particleSystem;

    private void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        // ParticleSystemが再生中かどうかを確認する
        if (particleSystem.isPlaying)
            return;

        // ParticleSystemが再生されていない場合、GameObjectを削除する
        Destroy(gameObject);
    }
}
