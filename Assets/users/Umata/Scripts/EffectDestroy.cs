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
        // ParticleSystem‚ªÄ¶’†‚©‚Ç‚¤‚©‚ğŠm”F‚·‚é
        if (particleSystem.isPlaying)
            return;

        // ParticleSystem‚ªÄ¶‚³‚ê‚Ä‚¢‚È‚¢ê‡AGameObject‚ğíœ‚·‚é
        Destroy(gameObject);
    }
}
