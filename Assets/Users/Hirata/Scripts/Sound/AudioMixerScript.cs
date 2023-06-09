using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioMixerScript : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider Master_Slider;
    public Slider BGM_Slider;
    public Slider SE_Slider;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float ConvertVolumeToDb(float volume)
    {
        return Mathf.Clamp(Mathf.Log10(Mathf.Clamp(volume, 0f, 1f)) * 20f, -80f, 0f);
    }

    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("Master", ConvertVolumeToDb(Master_Slider.value));
    }

    public void SetBgmVolume(float volume)
    {
        audioMixer.SetFloat("BGM", ConvertVolumeToDb(BGM_Slider.value));
    }

    public void SetSeVolume(float volume)
    {
        audioMixer.SetFloat("SE", ConvertVolumeToDb(SE_Slider.value));
    }
}
