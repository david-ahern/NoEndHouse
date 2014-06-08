using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundController : MonoBehaviour
{
    static public SoundController instance;

    public List<Soundtrack> SoundTracks;
    private AudioSource SoundtrackSource;
    public int _currentSountrackIndex = 0;

    private AudioListener SfxListener;

    static public float SoundtrackPlayPosition
    {
        get { return (instance != null ? instance.SoundtrackSource.time : 0); }
    }

    static public string SoundtrackName
    {
        get { return (instance != null ? instance.SoundtrackSource.clip.name : "None"); }
    }

    static public int CurrentSoundtrack
    {
        get { return (instance != null ? instance._currentSountrackIndex : 0); }
    }

    private List<MiscAudioSource> MiscSources;

    [Range(0, 1)]
    public float _masterVolume = 1.0f;
    static public float MasterVolume
    {
        get { return (instance != null ? instance._masterVolume : 0); }
        set
        {
            if (instance!= null)
            {
                instance._masterVolume = value;
                if (instance.SoundtrackSource != null) instance.SoundtrackSource.volume = instance._musicVolume * instance._masterVolume;
                AudioListener.volume = instance._sfxVolume * instance._masterVolume;
            }
        }
    }

    [Range(0, 1)]
    public float _musicVolume = 1.0f;
    static public float MusicVolume
    {
        get { return (instance != null ? instance._musicVolume : 0); }
        set
        {
            if (instance != null) 
            {
                instance._musicVolume = value;
                if (instance.SoundtrackSource != null) instance.SoundtrackSource.volume = instance._musicVolume * instance._masterVolume;
            }
        }
    }

    [Range(0, 1)]
    public float _sfxVolume = 1.0f;

    [SerializeField]
    static public float SfxVolume
    {
        get { return (instance != null ? instance._sfxVolume : 0); }
        set 
        {
            if (instance != null)
            {
                instance._sfxVolume = value;
                AudioListener.volume = instance._sfxVolume * instance._masterVolume;
            }
                 
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;

            SoundtrackSource = gameObject.AddComponent<AudioSource>();
            SoundtrackSource.volume = _musicVolume;
            SoundtrackSource.ignoreListenerVolume = true;

            MiscSources = new List<MiscAudioSource>();
        }
        if (instance == this)
            DontDestroyOnLoad(this.gameObject);
        else
            Destroy(this.gameObject);
    }

	void Start () 
    {
        PlaySoundtrack();
	}
	
	void Update () 
    {
        MusicVolume = _musicVolume;
        SfxVolume = _sfxVolume;

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
        if (instance != null)
        {
            instance._currentSountrackIndex = track;
            instance.SoundtrackSource.clip = instance.SoundTracks[track].Track;
            instance.SoundtrackSource.Play();
        }
    }

    static public void PlaySoundtrack()
    {
        if (instance != null)
        {
            PlaySoundtrack(Random.Range(0, instance.SoundTracks.Count));
        }
    }

    static public void PlaySoundtrack(Soundtrack track)
    {
        if (instance != null)
        {
            instance._currentSountrackIndex = instance.SoundTracks.IndexOf(track);
            instance.SoundtrackSource.clip = track.Track;
            instance.SoundtrackSource.Play();
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

    static public void PlayClip(MiscAudioClip clip, AudioSource source)
    {
        if (instance != null)
        {
            source.clip = clip.Clip;
            source.volume = clip.Volume;
            source.pitch = clip.Pitch;
            source.loop = clip.Loop;

            source.Play();
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

    public void AddSoundtrack(AudioClip clip)
    {
        Soundtrack track = new Soundtrack();
        track.Name = clip.name;
        track.Track = clip;
        SoundTracks.Add(track);
    }

    [System.Serializable]
    public class Soundtrack
    {
        [SerializeField]
        public string Name;
        [SerializeField]
        public AudioClip Track;

        public void Play()
        {
            SoundController.PlaySoundtrack(this);
        }
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