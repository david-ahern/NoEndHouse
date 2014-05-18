using UnityEngine;
using System.Collections;

public class PlayerEffectController : MonoBehaviour 
{
    public Heartbeat heartbeat;
    public Breathing breathing;

    private float HeartRateMultiplyer;
    private float BreatheRateMultiplyer;

	void Start () 
    {
        SoundController.PlayClip(heartbeat.HeartbeatAudio);
        SoundController.PlayClip(breathing.BreathingAudio);
	}
	
	// Update is called once per frame
	void Update () 
    {
	    if (Input.GetKey(KeyCode.LeftShift))
        {
            HeartRateMultiplyer = Mathf.Clamp(HeartRateMultiplyer + heartbeat.RateChangeIncrease, heartbeat.DefaultHeartRate, heartbeat.MaxHeartRate);
            BreatheRateMultiplyer = Mathf.Clamp(BreatheRateMultiplyer + breathing.RateChangeIncrease, breathing.DefaultBreathingRate, breathing.MaxBreathingRate);
        }
        else
        {
            HeartRateMultiplyer = Mathf.Clamp(HeartRateMultiplyer - heartbeat.RateChangeDecrease, heartbeat.DefaultHeartRate, heartbeat.MaxHeartRate);
            BreatheRateMultiplyer = Mathf.Clamp(BreatheRateMultiplyer - breathing.RateChangeDecrease, breathing.DefaultBreathingRate, breathing.MaxBreathingRate);
        }

        heartbeat.HeartbeatAudio.Volume = Mathf.Clamp(heartbeat.DefaultVolume + ((1 - heartbeat.DefaultVolume) * ((HeartRateMultiplyer - heartbeat.DefaultHeartRate) / (heartbeat.MaxHeartRate - heartbeat.DefaultHeartRate))), heartbeat.DefaultVolume, 1);
        heartbeat.HeartbeatAudio.Pitch = HeartRateMultiplyer;
        if (heartbeat.HeartbeatAudio.Clip != null)
            SoundController.PlayClip(heartbeat.HeartbeatAudio);

        breathing.BreathingAudio.Volume = Mathf.Clamp(breathing.DefaultVolume + ((1 - breathing.DefaultVolume) * ((BreatheRateMultiplyer - breathing.DefaultBreathingRate) / (breathing.MaxBreathingRate - breathing.DefaultBreathingRate))), heartbeat.DefaultVolume, 1);
        breathing.BreathingAudio.Pitch = Mathf.Clamp(breathing.DefaultPitch + ((1 - breathing.DefaultPitch) * ((BreatheRateMultiplyer - breathing.DefaultBreathingRate) / (breathing.MaxBreathingRate - breathing.DefaultBreathingRate))), breathing.DefaultPitch, breathing.MaxPitch);
        if (breathing.BreathingAudio.Clip != null)
            SoundController.PlayClip(breathing.BreathingAudio);
	}

    [System.Serializable]
    public class Heartbeat
    {
        [SerializeField]
        public MiscAudioClip HeartbeatAudio;

        [SerializeField]
        public float RateChangeIncrease = 0.001f;
        [SerializeField]
        public float RateChangeDecrease = 0.0005f;

        [SerializeField]
        [Range(0,2)]
        public float DefaultHeartRate = 1.0f;
        [SerializeField]
        [Range(0,2)]
        public float MaxHeartRate = 2.0f;
        [SerializeField]
        [Range(0,1)]
        public float DefaultVolume = 1.0f;
    }

    [System.Serializable]
    public class Breathing
    {
        [SerializeField]
        public MiscAudioClip BreathingAudio;

        [SerializeField]
        public float RateChangeIncrease = 0.001f;
        [SerializeField]
        public float RateChangeDecrease = 0.0005f;

        [SerializeField]
        [Range(0, 2)]
        public float DefaultBreathingRate = 0.3f;
        [SerializeField]
        [Range(0, 2)]
        public float MaxBreathingRate = 1.0f;
        [SerializeField]
        [Range(0, 1)]
        public float DefaultVolume = 1.0f;
        [SerializeField]
        [Range(0, 2)]
        public float DefaultPitch = 1.0f;
        [SerializeField]
        [Range(0, 2)]
        public float MaxPitch = 1.0f;
    }
}
