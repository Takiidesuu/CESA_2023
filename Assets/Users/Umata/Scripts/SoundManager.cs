using UnityEngine;

[System.Serializable]
public struct SoundEffect
{
    public string name;
    public AudioClip[] clip;
    [Range(0.0f, 1.0f)] public float volume;
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
        audioSource = GetComponent<AudioSource>();
        SoundEffect effect = GetSoundEffectByName(name);
        if (effect.clip != null)
        {
            int randomIndex = Random.Range(0, effect.clip.Length);
            audioSource.clip = effect.clip[randomIndex];
            if (effect.volume > 0)
            {
                audioSource.volume = effect.volume;
            }
            else
            {
                audioSource.volume = 1;
            }
            audioSource.PlayOneShot(audioSource.clip);
        }
        else
        {
            Debug.LogError("Sound effect not found. Name: " + name);
        }
    }
    
    public void StopSoundEffect(string name)
    {
        SoundEffect effect = GetSoundEffectByName(name);
        if (effect.clip != null)
        {
            int randomIndex = Random.Range(0, effect.clip.Length);
            if (audioSource.clip == effect.clip[randomIndex])
            {
                audioSource.Stop();
            }
        }
        else
        {
            Debug.LogError("Sound effect not found. Name: " + name);
        }
    }
    
    public bool CheckIsPlaying(string name)
    {
        SoundEffect effect = GetSoundEffectByName(name);
        if (effect.clip != null)
        {
            int randomIndex = Random.Range(0, effect.clip.Length);
            if (audioSource.clip == effect.clip[randomIndex])
            {
                return audioSource.isPlaying;
            }
        }
        else
        {
            Debug.LogError("Sound effect not found. Name: " + name);
        }
        
        return false;
    }

    public void PlayHitStopSound(string name,float stoptime)
    {
        SoundEffect effect = GetSoundEffectByName(name);
        if (effect.clip != null)
        {
            int randomIndex = Random.Range(0, effect.clip.Length);
            audioSource.clip = effect.clip[randomIndex];
            if (effect.volume > 0)
            {
                audioSource.volume = effect.volume;
            }
            else
            {
                audioSource.volume = 1;
            }
            audioSource.Play();
        }
        else
        {
            Debug.LogError("Sound effect not found. Name: " + name);
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
