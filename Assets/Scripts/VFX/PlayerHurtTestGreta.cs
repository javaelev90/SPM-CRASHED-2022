using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class PlayerHurtTestGreta : MonoBehaviour
{
    /*
    private float vignetteIntensityNormal = 0.369f;
    private float vignetteSmoothnessNormal = 0.5f;
    [SerializeField] private Color vignetteColorNormal;

    private float vignetteIntensityHurt = 0.5f;
    private float vignetteSmoothnessHurt = 1f;
    [SerializeField] private Color vignetteColorHurt;

    public VolumeProfile volumeProfile;
    private Vignette vignette;
    ClampedFloatParameter intensity;
    ClampedFloatParameter smoothness;
    ColorParameter color;



    // Start is called before the first frame update
    private void Start()
    {
        for (int i = 0; i < volumeProfile.components.Count; i++)
        {
            if (volumeProfile.components[i].name == "Vignette")
            {
                vignette = (Vignette)volumeProfile.components[i];
            }
        }
        intensity = vignette.intensity;
        smoothness = vignette.smoothness;
        color = vignette.color;

        intensity.value = vignetteIntensityNormal;
        smoothness.value = vignetteSmoothnessNormal;
        color.value = vignetteColorNormal;

        StartCoroutine(LerpVignette(2));
    

    }
    

    // Update is called once per frame
    void Update()
    {
        
    }

    //Lerp between startValue and endValue over 'duration' seconds
    private IEnumerator LerpVignette(float duration)
    {
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float intensityValue = Mathf.Lerp(vignetteIntensityNormal, vignetteIntensityHurt, elapsed / duration);
            float smoothnessValue = Mathf.Lerp(vignetteSmoothnessNormal, vignetteSmoothnessHurt, elapsed / duration);
            Color vignetteColor = Color.Lerp(vignetteColorNormal, vignetteColorHurt, elapsed / duration);
            intensity.value = intensityValue;
            smoothness.value = smoothnessValue;
            color.value = vignetteColor;

            
            yield return null;
        }
        yield return new WaitForSeconds(3f);

        LerpVignetteBack(2);
    }

    private IEnumerator LerpVignetteBack(float duration)
    {
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float intensityValue = Mathf.Lerp(vignetteIntensityHurt, vignetteIntensityNormal, elapsed / duration);
            float smoothnessValue = Mathf.Lerp(vignetteSmoothnessHurt, vignetteSmoothnessNormal, elapsed / duration);
            Color vignetteColor = Color.Lerp(vignetteColorHurt, vignetteColorNormal, elapsed / duration);
            intensity.value = intensityValue;
            smoothness.value = smoothnessValue;
            color.value = vignetteColor;
            yield return null;
        }
        StartCoroutine(LerpVignetteBack(2));
    }

    */
}
