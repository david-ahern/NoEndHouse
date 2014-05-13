using UnityEngine;
using System.Collections;

public class Heartbeat : MonoBehaviour 
{
    public MiscAudioClip Clip;

    private float HeartRateMultiplyer;

    public float RateChangeIncrease = 0.01f;
    public float RateChangeDecrease = 0.001f;

    public float DefaultRate = 1.0f;
    public float MaxRate = 2.0f;
    public float DefaultVolume = 1.0f;

	void Start () 
    {
        SoundController.PlayClip(Clip);
	}
	
	// Update is called once per frame
	void Update () 
    {
	    if (Input.GetKey(KeyCode.LeftShift))
        {
            HeartRateMultiplyer = Mathf.Clamp(HeartRateMultiplyer + RateChangeIncrease, DefaultRate, MaxRate);
        }
        else
        {
            HeartRateMultiplyer = Mathf.Clamp(HeartRateMultiplyer - RateChangeDecrease, DefaultRate, MaxRate);
        }

        Clip.Volume = Mathf.Clamp(DefaultVolume + ((1 - DefaultVolume) * ((HeartRateMultiplyer - DefaultRate) / (MaxRate - DefaultRate))), DefaultVolume, 1);
        Clip.Pitch = HeartRateMultiplyer;
        SoundController.PlayClip(Clip);
	}
}
