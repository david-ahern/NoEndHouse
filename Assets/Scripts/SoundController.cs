using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class SoundController : MonoBehaviour 
{
    static public SoundController instance;

    public List<Soundtrack> SoundTracks;

    private AudioSource SoundtrackSource;

    static public float SoundtrackPlayPosition
    {
        get { return instance.SoundtrackSource.time; }
    }

    static public string SoundtrackName
    {
        get { return instance.SoundtrackSource.clip.name; }
    }

    private List<MiscAudioSource> MiscSources;

    [Range(0,1)]
    public float _musicVolume = 1.0f;
    static public float MusicVolume
    {
        get { if (instance != null) return instance._musicVolume; else return -1; }
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
        PlaySoundtrack(0);
	}
	
	void Update () 
    {
        if (Input.GetKeyUp(KeyCode.P))
            if (MusicVolume > 0)
                _musicVolume = 0.5f;
            else
                _musicVolume = 0;

        if (!SoundtrackSource.isPlaying)
            PlaySoundtrack(Random.Range(0, SoundTracks.Count));
        
        SoundtrackSource.volume = _musicVolume;

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
        if (instance != null)
        {
            instance.SoundtrackSource.clip = instance.SoundTracks[track].Track;
            instance.SoundtrackSource.Play();
            instance.SoundtrackSource.time = 80;
        }
    }

    static public void PlaySoundtrack()
    {
        if (instance != null)
        {
            PlaySoundtrack(Random.Range(0, instance.SoundTracks.Count));
        }
    }

    static public void StopSoundtrack()
    {
        if (instance != null)
        {
            instance.SoundtrackSource.Stop();
        }
    }

    static public void PlayClip(MiscAudioClip clip)
    {
        if (instance != null)
        {
            for (int i = 0; i < instance.MiscSources.Count; ++i)
            {
                if (instance.MiscSources[i].Source.clip == clip.Clip)
                {
                    if (instance.MiscSources[i].Override)
                    {
                        instance.MiscSources[i].Source.Stop();
                        instance.MiscSources[i].Source.Play();
                    }
                    else if (instance.MiscSources[i].CreateAnother)
                    {
                        instance.MiscSources.Add(CreateSource(clip));
                        instance.MiscSources[instance.MiscSources.Count - 1].Source.Play();
                    }
                    instance.MiscSources[i].Source.volume = clip.Volume;
                    instance.MiscSources[i].Source.pitch = clip.Pitch;
                    instance.MiscSources[i].Source.loop = clip.Loop;
                    return;
                }
            }
            instance.MiscSources.Add(CreateSource(clip));
            instance.MiscSources[instance.MiscSources.Count - 1].Source.Play();
        }
    }

    static public void PlayClip(AudioClip clip)
    {
        if (instance != null)
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
                    else if (instance.MiscSources[i].CreateAnother)
                    {
                        instance.MiscSources.Add(CreateSource(clip));
                        instance.MiscSources[instance.MiscSources.Count - 1].Source.Play();
                    }
                    return;
                }

            }
            instance.MiscSources.Add(CreateSource(clip));
            instance.MiscSources[instance.MiscSources.Count - 1].Source.Play();
        }
    }

    static private MiscAudioSource CreateSource(AudioClip Clip, bool Loop = false, bool Override = false, bool CreateAnother = false, float Volume = 1.0f, float Pitch = 1.0f)
    {
        MiscAudioSource source = new MiscAudioSource(instance.gameObject.AddComponent<AudioSource>(), Override, CreateAnother);
        source.Source.clip = Clip;
        source.Source.loop = Loop;
        source.Source.volume = Volume;
        source.Source.pitch = Pitch;
        return source;
    }

    static private MiscAudioSource CreateSource(MiscAudioClip clip)
    {
        MiscAudioSource source = new MiscAudioSource(instance.gameObject.AddComponent<AudioSource>(), clip.Overridable, clip.CreateAnother);
        source.Source.clip = clip.Clip;
        source.Source.loop = clip.Loop;
        source.Source.volume = clip.Volume;
        source.Source.pitch = clip.Pitch;
        return source;
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
        public bool CreateAnother;

        public MiscAudioSource(AudioSource source, bool Overridable, bool createAnother) 
        { 
            Source = source; 
            Override = Overridable;
            CreateAnother = createAnother;
        }
    }
}

[System.Serializable]
public class MiscAudioClip
{
    [SerializeField]
    public AudioClip Clip;
    [SerializeField]
    public bool Loop = false;
    [SerializeField]
    public bool Overridable = false;
    [SerializeField]
    public bool CreateAnother = false;
    [SerializeField]
    public float Volume = 1.0f;
    [SerializeField]
    public float Pitch = 1.0f;
}