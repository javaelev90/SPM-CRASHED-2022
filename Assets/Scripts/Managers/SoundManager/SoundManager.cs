using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

[RequireComponent(typeof(Slider))]
public class SoundManager : MonoBehaviour
{
    Slider slider { get { return GetComponent<Slider>(); } }
    
    [SerializeField] private string volumeName = "Enter Volume name here";
    [SerializeField] private Text volumeLabel;
    public void UpdateValueOnChange(float value)
    {
        
        if (volumeLabel != null)
        {
            volumeLabel.text = Mathf.Round(value * 100.0f).ToString() + "%";
        }
        if (Settings.profile)
        {
            Settings.profile.SetAudioLevels(volumeName, value);
        }

    }

    public void ResetSliderValue()
    {
        if (Settings.profile)
        {
            float volume = Settings.profile.GetAudioLevels(volumeName);
            UpdateValueOnChange(volume);
            slider.value = volume;
        }
    }

    /*
    public void SaveVolumes()
    {
        float volumeValue = slider.value;
        PlayerPrefs.SetFloat("VolumeValue", volumeValue);
        LoadValues();
    }

    void LoadValues()
    {
        float volumeValue = PlayerPrefs.GetFloat("VolumeValue");
        slider.value = volumeValue;
        AudioListener.volume = volumeValue;
    }
    */

    void Start()
    {
        //LoadValues();
        //slider.value = Settings.profile.GetAudioLevels(volumeName);
        ResetSliderValue();
        //UpdateValueOnChange(slider.value);

        slider.onValueChanged.AddListener(delegate {
            UpdateValueOnChange(slider.value);
        });

    }
}
