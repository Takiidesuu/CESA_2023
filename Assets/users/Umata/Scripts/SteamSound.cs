using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamSound : MonoBehaviour
{
    // Start is called before the first frame update
    ParticleSystem ps;
    SoundManager se;
    public float repeatTime = 3.0f; // ŒJ‚è•Ô‚µÀs‚·‚éŠÔŠu

    private bool startSetup = false;

    private float timer = 0.0f;
    private IEnumerator coroutine;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        se = GetComponent<SoundManager>();

        Repeat(ps.startDelay);
    }

    // Update is called once per frame
    void Update()
    {

        if (startSetup)
        {
            timer += Time.deltaTime;

            if (timer >= repeatTime)
            {
                coroutine = Repeat(0); // ˆø”‚ğ“n‚·
                StartCoroutine(coroutine);
                timer = 0.0f;
            }
        }
        else
        {
            timer += Time.deltaTime;
        }
    }
    IEnumerator Repeat(float sec)
    {
        yield return new WaitForSeconds(sec);
        se.PlaySoundEffect("Steam");
    }
}
