using UnityEngine;
using System.Collections;

public class Heartbeat : MonoBehaviour 
{
    public MiscAudioClip Clip;

    private float HeartRateMultiplyer;

    public float RateChange = 0.01f;

    public float DefaultRate = 1.0f;
    public float MaxRate = 2.0f;
    public float MinRate = 0.5f;

	void Start () 
    {
        SoundController.PlayClip(Clip);
	}
	
	// Update is called once per frame
	void Update () 
    {
	    if (Input.GetKey(KeyCode.LeftShift))
        {
            HeartRateMultiplyer = Mathf.Clamp(HeartRateMultiplyer + RateChange, DefaultRate, MaxRate);
        }
        else
        {
            HeartRateMultiplyer = Mathf.Clamp(HeartRateMultiplyer - RateChange, DefaultRate, MaxRate);
        }
        Clip.Pitch = HeartRateMultiplyer;
        SoundController.PlayClip(Clip);
	}
}
