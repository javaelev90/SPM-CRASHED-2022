using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

[System.Serializable]
public class Volume
{
    public string name;
    public float volume = 1f;
    public float tempVolume = 1f;
}
public class Settings
{
    public static SoundProfiles profile;
}

[CreateAssetMenu(menuName = "Create Profile")]
public class SoundProfiles : ScriptableObject
{
    public bool saveInPlayerPrefs = true;
    public string prePrefix = "Settings";

    public AudioMixer audioMixer;
    public Volume[] volumeControl;

    public void SetProfile(SoundProfiles profile)
    {
        Settings.profile = profile;
    }

    public float GetAudioLevels(string name)
    {
        float volume = 1f;

        if (!audioMixer)
        {
            Debug.LogWarning("There is no AudioMeixer defined in the profiles file");
            return volume;
        }

        for (int i = 0; i < volumeControl.Length; i++)
        {
            if (volumeControl[i].name != name)
            {
                continue;
            }
            else
            {
                if (saveInPlayerPrefs)
                {
                    if (PlayerPrefs.HasKey(prePrefix + volumeControl[i].name))
                    {
                        volumeControl[i].volume = PlayerPrefs.GetFloat(prePrefix + volumeControl[i].name);
                    }
                }

                volumeControl[i].tempVolume = volumeControl[i].volume;

                if (audioMixer)
                {
                    audioMixer.SetFloat(volumeControl[i].name, Mathf.Log(volumeControl[i].volume) * 20f);
                }
                volume = volumeControl[i].volume;

                break;
            }
        }
        return volume;
    }

    public void GetAudioLevels()
    {
        //float volume = 1f;

        if (!audioMixer)
        {
            Debug.LogWarning("There is no AudioMeixer defined in the profiles file");
        }

        for (int i = 0; i < volumeControl.Length; i++)
        {

            if (saveInPlayerPrefs)
            {
                if (PlayerPrefs.HasKey(prePrefix + volumeControl[i].name))
                {
                    volumeControl[i].volume = PlayerPrefs.GetFloat(prePrefix + volumeControl[i].name);
                }
            }
            // reset the audio volume
            volumeControl[i].tempVolume = volumeControl[i].volume;

            // set the mixer to match the volume
            audioMixer.SetFloat(volumeControl[i].name, Mathf.Log(volumeControl[i].volume) * 20f);
        }

    }

    public void SetAudioLevels(string name, float volume)
    {
        if (!audioMixer)
        {
            Debug.LogWarning("There is no AudioMeixer defined in the profiles file");
            return;
        }

        for(int i = 0; i < volumeControl.Length; i++)
        {
            if (volumeControl[i].name != name)
            {
                continue;
            }
            else
            {
                audioMixer.SetFloat(volumeControl[i].name, Mathf.Log(volume) * 20f);
                volumeControl[i].tempVolume = volume;
                break;
            }
        }
    }

    public void SaveAudioLevels()
    {
        if (!audioMixer)
        {
            Debug.LogWarning("There is no AudioMeixer defined in the profiles file");
            return;
        }

        float volume = 0f;
        for (int i = 0; i < volumeControl.Length; i++)
        {
            volume = volumeControl[i].tempVolume;
            if (saveInPlayerPrefs)
            {
                PlayerPrefs.GetFloat(prePrefix + volumeControl[i].name);

            }
            audioMixer.SetFloat(volumeControl[i].name, Mathf.Log(volume) * 20f);
            volumeControl[i].volume = volume;

        }
    }





}
