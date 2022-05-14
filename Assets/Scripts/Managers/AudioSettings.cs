using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    // Volume variables
    Slider effectsSlider;
    Slider musicSlider;
    [SerializeField] AudioMixer effectsMixer;
    [SerializeField] AudioMixer musicMixer;

    // Start is called before the first frame update
    void Start()
    {
        // Check if player prefs were set, if not, give default values
        if (!PlayerPrefs.HasKey("Music"))
        {
            PlayerPrefs.SetFloat("Music", 0.5f);
        }
        if (!PlayerPrefs.HasKey("FX"))
        {
            PlayerPrefs.SetFloat("FX", 0.5f);
        }

        // Update variables based on player prefs data
        musicMixer.SetFloat("masterVolume", PlayerPrefs.GetFloat("Music"));
        effectsMixer.SetFloat("masterVolume", PlayerPrefs.GetFloat("FX"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateMusicSlider()
    {
        PlayerPrefs.SetFloat("Music", musicSlider.value);
        musicMixer.SetFloat("masterVolume", PlayerPrefs.GetFloat("Music"));
    }

    public void UpdateFXSlider()
    {
        PlayerPrefs.SetFloat("FX", effectsSlider.value);
        effectsMixer.SetFloat("masterVolume", PlayerPrefs.GetFloat("FX"));
    }

    public void UpdateSettingsSliders()
    {
        if (!effectsSlider)
        {
            effectsSlider = GameObject.FindGameObjectWithTag("FXSlider").GetComponent<Slider>();
        }
        if (!musicSlider)
        {
            musicSlider = GameObject.FindGameObjectWithTag("MusicSlider").GetComponent<Slider>();
        }
        effectsSlider.value = PlayerPrefs.GetFloat("FX");
        musicSlider.value = PlayerPrefs.GetFloat("Music");
    }
}
