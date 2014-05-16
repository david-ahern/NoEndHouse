using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Light))]

public class Lightning : MonoBehaviour 
{
    private Light LightComponent;

    [Range(0,2)]
    public float MaxFlashIntensity = 1.0f;

    public float AverageLightningFrequency = 10.0f;
    [Range(0,1)]
    public float LightningFrequencyRange = 1.0f;

    public float AverageFlashSeperation = 1.0f;
    [Range(0,1)]
    public float FlashSeperationRange = 1.0f;

    public float AverageFlashTime = 0.1f;
    [Range(0,1)]
    public float FlashTimeRange = 1.0f;

    public int MaxFlashes = 3;
    public int MinFlashes = 1;

    public ParticleSystem LightningParticles;

    public List<MiscAudioClip> SoundClips;

    private float NextFlashTime = 0.0f;

    public Vector2 Velocity = new Vector2(0.1f, 0.1f);
    public Vector2 MaxDistances = new Vector2(100.0f, 100.0f);

	void Start () 
    {
        NextFlashTime = Time.time + Random.Range(AverageLightningFrequency * (1 - LightningFrequencyRange), AverageLightningFrequency * (1 + LightningFrequencyRange));
        gameObject.light.intensity = 0;
	}
	
	// Update is called once per frame
	void Update () 
    {
        HandleMovement();
	    if (Time.time > NextFlashTime)
        {
            NextFlashTime = Time.time + Random.Range(AverageLightningFrequency * (1 - LightningFrequencyRange), AverageLightningFrequency * (1 + LightningFrequencyRange));
            int FlashCount = Random.Range(MinFlashes, MaxFlashes);
            StartCoroutine(Flash(FlashCount));
        }
	}

    public IEnumerator Flash(int FlashCount)
    {
        int NumFlashes = 0;

        float Intensity = CalculateIntensity();

        while (NumFlashes < FlashCount)
        {
            float flashTime = Random.Range(AverageFlashTime * (1 - FlashTimeRange), AverageFlashTime * (1 + FlashTimeRange));

            if (SoundClips.Count > 0)
                StartCoroutine(HandleSound((SoundClips[Random.Range(0, SoundClips.Count)])));

            gameObject.light.intensity = Intensity;
            if (LightningParticles != null)
            {
                LightningParticles.startLifetime = flashTime;
                LightningParticles.Play();
            }
            yield return new WaitForSeconds(flashTime);

            if (LightningParticles != null)
                LightningParticles.Stop();
            gameObject.light.intensity = 0;
            
            float nextTime = Random.Range(AverageFlashSeperation * (1 - FlashSeperationRange), AverageFlashSeperation * (1 + FlashSeperationRange));
            yield return new WaitForSeconds(nextTime);
            NumFlashes++;
        }
    }

    private float CalculateIntensity()
    {
        return Mathf.Clamp(MaxFlashIntensity * (1 - ((new Vector2(gameObject.transform.position.x, gameObject.transform.position.z) - new Vector2(Globals.Player.gameObject.transform.position.x, Globals.Player.gameObject.transform.position.z)).magnitude / MaxDistances.magnitude)), 0, MaxFlashIntensity);
    }

    private IEnumerator HandleSound(MiscAudioClip clip)
    {
        clip.Volume = Mathf.Clamp(1 - ((new Vector2(gameObject.transform.position.x, gameObject.transform.position.z) - new Vector2(Globals.Player.gameObject.transform.position.x, Globals.Player.gameObject.transform.position.z)).magnitude / MaxDistances.magnitude), 0, 1);
        clip.Pitch = Mathf.Clamp(1 - ((new Vector2(gameObject.transform.position.x, gameObject.transform.position.z) - new Vector2(Globals.Player.gameObject.transform.position.x, Globals.Player.gameObject.transform.position.z)).magnitude / MaxDistances.magnitude), 0, 1);
        float delay = (new Vector2(gameObject.transform.position.x, gameObject.transform.position.z) - new Vector2(Globals.Player.gameObject.transform.position.x, Globals.Player.gameObject.transform.position.z)).magnitude / Globals.SoundSpeed;

        yield return new WaitForSeconds(delay);
        SoundController.PlayClip(clip);
        yield break;
    }

    private void HandleMovement()
    {
        gameObject.transform.position += new Vector3(Velocity.x, 0, Velocity.y);

        if (gameObject.transform.position.x > Globals.Player.gameObject.transform.position.x + MaxDistances.x)
            gameObject.transform.position = new Vector3(Globals.Player.gameObject.transform.position.x - MaxDistances.x + 5.0f, gameObject.transform.position.y, gameObject.transform.position.z);

        if (gameObject.transform.position.x < Globals.Player.gameObject.transform.position.x - MaxDistances.x)
            gameObject.transform.position = new Vector3(Globals.Player.gameObject.transform.position.x + MaxDistances.x - 5.0f, gameObject.transform.position.y, gameObject.transform.position.z);

        if (gameObject.transform.position.z > Globals.Player.gameObject.transform.position.y + MaxDistances.y)
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, Globals.Player.gameObject.transform.position.y - MaxDistances.y + 5.0f);

        if (gameObject.transform.position.z < Globals.Player.gameObject.transform.position.y - MaxDistances.y)
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, Globals.Player.gameObject.transform.position.y + MaxDistances.y - 5.0f);

        gameObject.transform.forward = Globals.Player.transform.position - gameObject.transform.position;
    }
}