using UnityEngine;

[System.Serializable]
public struct SoundEffect
{
    public string name;
    public AudioClip[] clip;
}

public class SoundManager : MonoBehaviour
{
    public SoundEffect[] soundEffects;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySoundEffect(string name)
    {
        SoundEffect effect = GetSoundEffectByName(name);
        if (effect.clip != null)
        {
            int randomIndex = Random.Range(0, effect.clip.Length);
            audioSource.clip = effect.clip[randomIndex];
            audioSource.Play();
        }
        else
        {
            Debug.LogError("Sound effect not found.");
        }
    }

    public void PlayHitStopSound(string name,float stoptime)
    {
        SoundEffect effect = GetSoundEffectByName(name);
        if (effect.clip != null)
        {
            int randomIndex = Random.Range(0, effect.clip.Length);
            audioSource.clip = effect.clip[randomIndex];
            audioSource.Play();
        }
        else
        {
            Debug.LogError("Sound effect not found.");
        }
    }

    private SoundEffect GetSoundEffectByName(string name)
    {
        for (int i = 0; i < soundEffects.Length; i++)
        {
            if (soundEffects[i].name == name)
            {
                return soundEffects[i];
            }
        }
        return new SoundEffect();
    }
}
