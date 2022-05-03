using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class LightingManager : MonoBehaviour
{
    [SerializeField] private Light DirectionalLight;
    [SerializeField] private Light MoonLight;
    [SerializeField] private LightingPreset Preset;
    [SerializeField, Range(0, 150)] private float TimeOfDay;

    public bool isNight { get; private set; }


    private void Update()
    {
        if (Preset == null)
        {
            return;
        }

        if (Application.isPlaying)
        {
            TimeOfDay += Time.deltaTime;
            TimeOfDay %= 150f;
            UpdateLighting(TimeOfDay / 150f);
            //MoonLight.intensity = (TimeOfDay / 150f);
            if (TimeOfDay > 35f && TimeOfDay < 115f)
            {
                isNight = true;
                
                //MoonLight.intensity = 0.2f;
                //MoonLight.gameObject.SetActive(false);
            }
            else
            {
                isNight = false;
                //MoonLight.intensity = 3f;
                //MoonLight.gameObject.SetActive(true);
            }
        }
        else
        {
            UpdateLighting(TimeOfDay / 150f);
        }


    }

    private void UpdateLighting(float timePercent)
    {
        RenderSettings.ambientLight = Preset.AmbientColor.Evaluate(timePercent);
       //RenderSettings. = Preset.AmbientColor.Evaluate(timePercent);
        RenderSettings.fogColor = Preset.FogColor.Evaluate(timePercent);
        MoonLight.color = Preset.MoonLightColor.Evaluate(timePercent);

        if (DirectionalLight != null)
        {
            //DirectionalLight.color = Preset.DirectionalColor.Evaluate(timePercent);
            DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, -170, 0));
        }

    
     
       
    }


    private void OnValidate()
    {
        if (DirectionalLight != null)
        {
            return;
        }

        if (RenderSettings.sun != null)
        {
            DirectionalLight = RenderSettings.sun;
        }
        else
        {
            Light[] lights = GameObject.FindObjectsOfType<Light>();
            foreach(Light light in lights)
            {
                if (light.type == LightType.Directional)
                {
                    DirectionalLight = light;
                    return;
                }
            }
        }
    }

}
