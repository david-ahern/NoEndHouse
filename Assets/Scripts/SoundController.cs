using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class SoundController : MonoBehaviour 
{
    static public SoundController instance;

    public List<Soundtrack> SoundTracks;

    private AudioSource SoundtrackSource;

    private List<MiscAudioSource> MiscSources;

    [Range(0,1)]
    public float _musicVolume = 1.0f;
    public float MusicVolume
    {
        get { return _musicVolume; }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;

            SoundtrackSource = gameObject.AddComponent<AudioSource>();

            SoundtrackSource.volume = _musicVolume;

            MiscSources = new List<MiscAudioSource>();
        }
        if (instance == this)
            DontDestroyOnLoad(this.gameObject);
        else
            Destroy(this.gameObject);
    }

	void Start () 
    {
        PlaySoundtrack(Random.Range(0, SoundTracks.Count));
	}
	
	void Update () 
    {
        if (!SoundtrackSource.isPlaying)
            PlaySoundtrack(Random.Range(0, SoundTracks.Count));

        for (int i = 0; i < instance.MiscSources.Count; ++i)
        {
            if (!instance.MiscSources[i].Source.isPlaying)
            {
                Destroy(instance.MiscSources[i].Source);
                instance.MiscSources.RemoveAt(i);
                --i;
            }
        }
	}

    static public void PlaySoundtrack(int track)
    {
        instance.SoundtrackSource.clip = instance.SoundTracks[track].Track;
        instance.SoundtrackSource.Play();
    }

    static public void PlayClip(MiscAudioClip clip)
    {
        for (int i = 0; i < instance.MiscSources.Count; ++i)
        {
            if (instance.MiscSources[i].Source.clip == clip.Clip)
            {
                if (instance.MiscSources[i].Override)
                {
                    instance.MiscSources[i].Source.Stop();
                    instance.MiscSources[i].Source.volume = clip.Volume;
                    instance.MiscSources[i].Source.pitch = clip.Pitch;
                    instance.MiscSources[i].Source.Play();
                }
                return;
            }
        }
        instance.MiscSources.Add(new MiscAudioSource(instance.gameObject.AddComponent<AudioSource>(), clip.Overridable));
        instance.MiscSources[instance.MiscSources.Count - 1].Source.clip = clip.Clip;
        instance.MiscSources[instance.MiscSources.Count - 1].Source.volume = clip.Volume;
        instance.MiscSources[instance.MiscSources.Count - 1].Source.pitch = clip.Pitch;
        instance.MiscSources[instance.MiscSources.Count - 1].Override = clip.Overridable;
        instance.MiscSources[instance.MiscSources.Count - 1].Source.Play();
    }

    static public void PlayClip(AudioClip clip, bool Overridable)
    {
        for (int i = 0; i < instance.MiscSources.Count; ++i)
        {
            if (instance.MiscSources[i].Source.clip == clip)
            {
                if (instance.MiscSources[i].Override)
                {
                    instance.MiscSources[i].Source.Stop();
                    instance.MiscSources[i].Source.Play();
                }
                return;
            }

        }
        instance.MiscSources.Add(new MiscAudioSource(instance.gameObject.AddComponent<AudioSource>(), Overridable));
        instance.MiscSources[instance.MiscSources.Count - 1].Source.clip = clip;
        instance.MiscSources[instance.MiscSources.Count - 1].Override = Overridable;
        instance.MiscSources[instance.MiscSources.Count - 1].Source.Play();
    }

    [System.Serializable]
    public class Soundtrack
    {
        [SerializeField]
        public string Name;
        [SerializeField]
        public AudioClip Track;
    }

    public class MiscAudioSource
    {
        public AudioSource Source;
        public bool Override;

        public MiscAudioSource(AudioSource s, bool ov) { Source = s; Override = ov; }
    }
}

[System.Serializable]
public class MiscAudioClip
{
    [SerializeField]
    public AudioClip Clip;
    [SerializeField]
    public bool Overridable = false;
    [SerializeField]
    public float Volume = 1.0f;
    [SerializeField]
    public float Pitch = 1.0f;
}