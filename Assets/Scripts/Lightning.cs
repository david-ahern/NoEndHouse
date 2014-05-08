using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Light))]

public class Lightning : MonoBehaviour 
{
    private Light LightComponent;

    public float FlashIntensity = 1.0f;

    public float AverageLightningFrequency = 10.0f;
    public float LightningFrequencyRange = 1.0f;

    public int MaxFlashes = 3;
    public int MinFlashes = 1;

    public float AverageFlashSeperation = 1.0f;
    public float FlashSeperationgRange = 1.0f;

    public float AverageFlashTime = 0.1f;
    public float FlashTimeRange = 1.0f;

    private float NextFlashTime = 0.0f;

	void Start () 
    {
        NextFlashTime = Time.time + Random.Range(AverageLightningFrequency * (1 - LightningFrequencyRange), AverageLightningFrequency * (1 + LightningFrequencyRange));

        gameObject.light.intensity = 0;
	}
	
	// Update is called once per frame
	void Update () 
    {
	    if (Time.time > NextFlashTime)
        {
            int FlashCount = Random.Range(MinFlashes, MaxFlashes);
            StartCoroutine(Flash(FlashCount));
        }
	}

    public IEnumerator Flash(int FlashCount)
    {
        int NumFlashes = 0;
        while (NumFlashes < FlashCount)
        {
            float flashTime = Random.Range(AverageFlashTime * (1 - FlashTimeRange), AverageFlashTime * (1 + FlashTimeRange));

            if (gameObject.audio != null)
                gameObject.audio.Play();

            gameObject.light.intensity = FlashIntensity;
            yield return new WaitForSeconds(flashTime);
            gameObject.light.intensity = 0;
            
            float nextTime = Random.Range(AverageFlashSeperation * (1 - FlashSeperationgRange), AverageFlashSeperation * (1 + FlashSeperationgRange));
            yield return new WaitForSeconds(nextTime);
            NumFlashes++;
        }
        NextFlashTime = Time.time + Random.Range(AverageLightningFrequency * (1 - LightningFrequencyRange), AverageLightningFrequency * (1 + LightningFrequencyRange));
    }
}
