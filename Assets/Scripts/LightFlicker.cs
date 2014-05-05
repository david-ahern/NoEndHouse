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

    public float AverageRefreshRate = 1.0f;
    public float RefreshRateRange = 1.0f;
    private float NextRefreshTime = 0.0f;

    public float AverageFadeSpeed = 0.5f;
    public float FadeSpeedRange = 1.0f;

    public float FadedLightPercentage = 0.5f;

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
                StartCoroutine(FadeLight(LightComponent.intensity, DefaultIntensity * FadedLightPercentage, false));
                LightComponent.flare = DefaultFlare;
                CurrentState = LightState.LOWERED;
            }
            else if (CurrentState == LightState.LOWERED)
            {
                int newState = Random.Range(0, 2);

                if (newState == 0)
                {
                    StartCoroutine(FadeLight(LightComponent.intensity, DefaultIntensity, false));
                    LightComponent.flare = DefaultFlare;
                    CurrentState = LightState.FULL;
                }
                else
                {
                    StartCoroutine(FadeLight(LightComponent.intensity, 0, true));
                    CurrentState = LightState.OFF;
                }
            }
            else
            {
                StartCoroutine(FadeLight(LightComponent.intensity, DefaultIntensity * FadedLightPercentage, false));
                LightComponent.flare = DefaultFlare;
                CurrentState = LightState.LOWERED;
            }
            NextRefreshTime = Time.time + Random.Range(AverageRefreshRate * (1 - RefreshRateRange), AverageRefreshRate * (1 + RefreshRateRange));
        }        
	}

    private IEnumerator FadeLight(float startIntensity, float endIntensity, bool turnOffFlare)
    {
        LightComponent.flare = DefaultFlare;

        float fadeEndTime = Time.time + Random.Range(AverageFadeSpeed * (1 - FadeSpeedRange), AverageFadeSpeed * (1 + FadeSpeedRange));

        while (Time.time < fadeEndTime)
        {
            // end time is the lower value in this because the time calculation goes from 1 to 0
            LightComponent.intensity = Mathf.Lerp(endIntensity, startIntensity, fadeEndTime - Time.time);
            LightSound.pitch = Mathf.Lerp(Mathf.Clamp(endIntensity / DefaultIntensity, 0.75f, 1.0f), Mathf.Clamp(startIntensity / DefaultIntensity, 0.75f, 1.0f), fadeEndTime - Time.time);
            LightSound.volume = Mathf.Lerp(endIntensity / DefaultIntensity, startIntensity / DefaultIntensity, fadeEndTime - Time.time);
            yield return new WaitForEndOfFrame();
        }

        if (turnOffFlare)
            LightComponent.flare = null;
    }
}
