using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventCallbacksSystem;

[ExecuteAlways]
public class LightingManager : MonoBehaviour
{
    [Header("Lighting stuff")]
    [SerializeField] private Light DirectionalLight;
    [SerializeField] private Light MoonLight;
    [SerializeField] private LightingPreset Preset;

    [Header("Time stuff")]
    [SerializeField] private float dayLength;
    [SerializeField] private float nightLength;
    [SerializeField] private float timeOfDay = 0;

    [Header("Other stuff")]
    [SerializeField] private NightSpawnersHandler nightSpawnersHandler;

    private float timeOfSunrise;
    private float totalTimeWholeCycle;
    private readonly float MAGICAL_SUNRISE_STARTER_NUMBER = 10;
    private bool cycleOngoing = true;

    public bool IsNight { get; private set; }

    public float DayLength { get { return dayLength; } }
    public float NightLength { get { return nightLength; } }

    public float TimeUntilCycle { get { return timeOfDay < dayLength && timeOfDay > 0 ? dayLength - timeOfDay : timeOfDay < 0 ? -timeOfDay : nightLength - timeOfDay + dayLength; } } // Math magic to return the correct number for timer


    private void Start()
    {
        //EventSystem.Instance.RegisterListener<EventEvent>(SetCycleOngoing);
        //EventSystem.Instance.RegisterListener<ShipPartEvent>(SetMinTimeUntilDawn);
        timeOfSunrise = dayLength / 2;
        IsNight = timeOfDay > dayLength;
        totalTimeWholeCycle = dayLength + nightLength;
        nightSpawnersHandler.SetupSpawners(nightLength - (nightLength / MAGICAL_SUNRISE_STARTER_NUMBER));
        cycleOngoing = true;
    }

    private void Update()
    {
        if (!cycleOngoing)
        {
            return;
        }

        if (Preset == null)
        {
            return;
        }

        if (Application.isPlaying)
        {
            timeOfDay += Time.deltaTime;
            timeOfDay %= dayLength * 2;

            if (timeOfDay > dayLength && timeOfDay < totalTimeWholeCycle - (nightLength / MAGICAL_SUNRISE_STARTER_NUMBER) && !IsNight)
            {
                IsNight = true;
                nightSpawnersHandler.StartNightSpawning();
                //MoonLight.intensity = 0.2f;
                //MoonLight.gameObject.SetActive(false);
            }
            else if (timeOfDay >= totalTimeWholeCycle - (nightLength / MAGICAL_SUNRISE_STARTER_NUMBER) && IsNight)
            {
                IsNight = false;
                timeOfDay -= totalTimeWholeCycle;
                nightSpawnersHandler.StopNightSpawning();
                //MoonLight.intensity = 3f;
                //MoonLight.gameObject.SetActive(true);
            }

            UpdateLighting((timeOfDay + timeOfSunrise) / (dayLength * 2));
            //MoonLight.intensity = (TimeOfDay / 150f);
            
        }
        else
        {
            UpdateLighting((timeOfDay + timeOfSunrise) / (dayLength * 2));
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

    public void SetCycleOngoing(EventEvent eventEvent)
    {
        cycleOngoing = !eventEvent.Start;
    }

    public void SetMinTimeUntilDawn(ShipPartEvent shipPartEvent)
    {

        if (timeOfDay < dayLength - shipPartEvent.TimeUntilDawn)
        {
            timeOfDay = dayLength - shipPartEvent.TimeUntilDawn;
        }
    }
}
