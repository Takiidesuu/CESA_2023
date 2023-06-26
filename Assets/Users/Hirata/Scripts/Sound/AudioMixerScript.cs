using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.EventSystems;

public class AudioMixerScript : MonoBehaviour
{
    public AudioMixer audioMixer;

    public Slider Master_Slider;
    public Slider BGM_Slider;
    public Slider SE_Slider;

    public TextMeshProUGUI Master_Volume;
    public TextMeshProUGUI BGM_Volume;
    public TextMeshProUGUI SE_Volume;

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(Master_Slider.gameObject);

        audioMixer.GetFloat("Master", out float master);
        audioMixer.GetFloat("BGM", out float bgm);
        audioMixer.GetFloat("SE", out float se);
        Master_Slider.value = 100 * ConvertDbtoVolume(master);
        BGM_Slider.value = 100 * ConvertDbtoVolume(bgm);
        SE_Slider.value = 100 * ConvertDbtoVolume(se);
    }

    void Update()
    {
        if (!gameObject.activeSelf)
        {
            audioMixer.GetFloat("Master", out float master);
            audioMixer.GetFloat("BGM", out float bgm);
            audioMixer.GetFloat("SE", out float se);
            Master_Slider.value = 100 * ConvertDbtoVolume(master);
            BGM_Slider.value = 100 * ConvertDbtoVolume(bgm);
            SE_Slider.value = 100 * ConvertDbtoVolume(se);
        }
    }

    public float ConvertVolumeToDb(float volume)
    {
        return Mathf.Clamp(Mathf.Log10(Mathf.Clamp(volume, 0f, 1f)) * 20f, -80f, 0f);
    }

    public float ConvertDbtoVolume(float Db)
    {
        return Mathf.Clamp(Mathf.Pow(10, Mathf.Clamp(Db, -80, 0) / 20f), 0, 1);
    }

    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("Master", ConvertVolumeToDb(Master_Slider.value));
        Master_Volume.text = (Mathf.Round(Master_Slider.value * 100)).ToString();
    }

    public void SetBgmVolume(float volume)
    {
        audioMixer.SetFloat("BGM", ConvertVolumeToDb(BGM_Slider.value));
        BGM_Volume.text = (Mathf.Round(BGM_Slider.value * 100)).ToString();
    }

    public void SetSeVolume(float volume)
    {
        audioMixer.SetFloat("SE", ConvertVolumeToDb(SE_Slider.value));
        SE_Volume.text = (Mathf.Round(SE_Slider.value * 100)).ToString();
    }
}
