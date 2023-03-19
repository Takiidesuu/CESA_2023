using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ParticleController : MonoBehaviour
{
    [SerializeField] private ParticleSystem particleSystem;
    [SerializeField] private bool looping = true;

    private void Start()
    {
        particleSystem.Play();
    }

    private void Update()
    {
        if (!particleSystem.IsAlive() && !looping)
        {
            Destroy(gameObject);
        }
    }
}
