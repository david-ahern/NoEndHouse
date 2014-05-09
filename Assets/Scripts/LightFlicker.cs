using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Light))]

public class LightFlicker : MonoBehaviour 
{
    private Light LightComponent;
    private AudioSource LightSound;

    private float DefaultIntensity;
    private float DefaultRange;
    private Flare DefaultFlare;

    private enum LightState { FULL, LOWERED, OFF };
    private LightState CurrentState;

    public float AverageOnTime = 1.0f;
    [Range(0,1)]
    public float OnTimeRange = 1.0f;

    public float AverageOffTime = 1.0f;
    [Range(0,1)]
    public float OffTimeRange = 1.0f;


    private float NextRefreshTime = 0.0f;

    public float AverageFadeSpeed = 0.5f;
    [Range(0,1)]
    public float FadeSpeedRange = 1.0f;

    [Range(0,1)]
    public float FadedLightPercentage = 0.5f;
    [Range(0,1)]
    public float FullLightVolume = 0.2f;


	void Start () 
    {
        LightComponent = gameObject.GetComponent<Light>();
        LightSound = gameObject.GetComponent<AudioSource>();

        DefaultIntensity = LightComponent.intensity;
        DefaultRange = LightComponent.range;
        DefaultFlare = LightComponent.flare;

        CurrentState = LightState.FULL;
	}
	

	void Update () 
    {
	    if(Time.time > NextRefreshTime)
        {
            if (CurrentState == LightState.FULL)
            {
                StartCoroutine(FadeLight(LightComponent.intensity, 0));
                CurrentState = LightState.OFF;

                NextRefreshTime = Time.time + Random.Range(AverageOffTime * (1 - OffTimeRange), AverageOffTime * (1 + OffTimeRange));
            }
            else if (CurrentState == LightState.LOWERED)
            {
                int newState = Random.Range(0, 2);

                if (newState == 0)
                {
                    StartCoroutine(FadeLight(LightComponent.intensity, DefaultIntensity));
                    CurrentState = LightState.FULL;
                    NextRefreshTime = Time.time + Random.Range(AverageOnTime * (1 - OnTimeRange), AverageOnTime * (1 + OnTimeRange));
                }
                else
                {
                    StartCoroutine(FadeLight(LightComponent.intensity, 0));
                    CurrentState = LightState.OFF;
                    NextRefreshTime = Time.time + Random.Range(AverageOffTime * (1 - OffTimeRange), AverageOffTime * (1 + OffTimeRange));
                }
            }
            else
            {
                StartCoroutine(FadeLight(LightComponent.intensity, DefaultIntensity * FadedLightPercentage));
                CurrentState = LightState.LOWERED;
                NextRefreshTime = Time.time + Random.Range(AverageOnTime * (1 - OnTimeRange), AverageOnTime * (1 + OnTimeRange));
            }
        }        
	}

    private IEnumerator FadeLight(float startIntensity, float endIntensity)
    {
        LightComponent.flare = DefaultFlare;

        float fadeEndTime = Time.time + Random.Range(AverageFadeSpeed * (1 - FadeSpeedRange), AverageFadeSpeed * (1 + FadeSpeedRange));

        while (Time.time < fadeEndTime)
        {
            // end time is the lower value in this because the time calculation goes from 1 to 0
            LightComponent.intensity = Mathf.Lerp(endIntensity, startIntensity, fadeEndTime - Time.time);
            if (LightSound != null)
                LightSound.volume = Mathf.Lerp(1 - ((1 - FullLightVolume) * endIntensity/DefaultIntensity), 1 - ((1 - FullLightVolume) * startIntensity/DefaultIntensity), fadeEndTime - Time.time);
            yield return new WaitForEndOfFrame();
        }

        if (endIntensity < 0.05)
        {
            if (LightSound != null)
                LightSound.volume = 0;
            LightComponent.flare = null;
        }
    }
}
