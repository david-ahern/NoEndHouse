using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundController : MonoBehaviour
{
#if UNITY_EDITOR
    [HideInInspector]
    public bool ShowDefaultInspector = false;

    [ContextMenu("Switch Inspector View")]
    void ShowDefault()
    {
        ShowDefaultInspector = !ShowDefaultInspector;
    }
#endif

    static public SoundController instance;

    public List<Soundtrack> SoundTracks;
    private AudioSource SoundtrackSource;
    public int _currentSountrackIndex = 0;
    private bool _soundtrackPaused = false;

    public bool _muted = false;
    public float StoredMasterVolume;

    private AudioSource DialogueSource;
    public List<Dialogue> DialogueQueue;

    public List<MiscAudioSource> MiscSources;

    static public bool Muted
    {
        get { return instance._muted; }
        set
        {
            SetMute(value);
        }
    }

    static public float CurrentSoundtrackPlayPosition
    {
        get { return (instance != null ? instance.SoundtrackSource.time : 0); }
    }

    static public string CurrentSoundtrackName
    {
        get { return (instance != null ? instance.SoundtrackSource.clip.name : "None"); }
    }

    static public int CurrentSoundtrackIndex
    {
        get { return (instance != null ? instance._currentSountrackIndex : 0); }
    }

    static public Soundtrack CurrentSoundtrack
    {
        get { return (instance != null ? instance.SoundTracks[instance._currentSountrackIndex] : null); }
    }

    static public bool SoundtrackPaused
    {
        get { return (instance != null ? instance._soundtrackPaused : true); }
        set
        {
            if (instance != null)
            {
                if (value)
                    instance.SoundtrackSource.Pause();
                else
                    instance.SoundtrackSource.Play();
                instance._soundtrackPaused = value;
            }
        }
    }

    

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
    public float _dialogueVolume = 1.0f;
    [SerializeField]
    static public float DialogueVolume
    {
        get { return (instance != null ? instance._dialogueVolume : 0); }
        set
        {
            if (instance != null)
            {
                instance._dialogueVolume = value;
                if (instance.DialogueSource != null) instance.DialogueSource.volume = instance._dialogueVolume;
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
            SoundtrackSource.hideFlags = HideFlags.HideInInspector;
            SoundtrackSource.volume = _musicVolume;
            SoundtrackSource.ignoreListenerVolume = true;

            DialogueSource = gameObject.AddComponent<AudioSource>();
            DialogueSource.hideFlags = HideFlags.HideInInspector;
            DialogueSource.volume = _dialogueVolume;
            DialogueSource.ignoreListenerVolume = true;

            DialogueQueue = new List<Dialogue>();

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
        MasterVolume = _masterVolume;
        MusicVolume = _musicVolume;
        DialogueVolume = _dialogueVolume;
        SfxVolume = _sfxVolume;

        if (Muted && MasterVolume > 0)
            _muted = false;

        if (!SoundtrackSource.isPlaying && !_soundtrackPaused)
            PlaySoundtrack(Random.Range(0, SoundTracks.Count));

        if (!DialogueSource.isPlaying && DialogueQueue.Count > 0)
            PlayNextDialogue();
        else if (!DialogueSource.isPlaying)
            HUDController.CloseSubtitle();

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
            if (!instance._soundtrackPaused)
                instance.SoundtrackSource.Play();
        }
    }

    static public void StopSoundtrack()
    {
        if (instance != null)
        {
            instance.SoundtrackSource.Stop();
            instance._soundtrackPaused = true;
        }
    }


    static public void PlayNextDialogue()
    {
        if (instance != null)
        {
            instance.DialogueSource.clip = instance.DialogueQueue[0].Clip;
            instance.DialogueSource.volume = instance.DialogueQueue[0].Volume;

            instance.DialogueSource.PlayDelayed(instance.DialogueQueue[0].Delay);

            HUDController.DisplaySubtitle(instance.DialogueQueue[0].Subtitle, instance.DialogueQueue[0].Delay);

            instance.DialogueQueue.RemoveAt(0);
        }
    }

    static public void PlayDialogue(Dialogue clip)
    {
        if (instance != null)
        {
            if (!instance.DialogueSource.isPlaying)
            {
                instance.DialogueSource.clip = clip.Clip;
                instance.DialogueSource.volume = clip.Volume;

                instance.DialogueSource.PlayDelayed(clip.Delay);

                HUDController.DisplaySubtitle(instance.DialogueQueue[0].Subtitle, instance.DialogueQueue[0].Delay);
            }
            else
            {
                instance.DialogueQueue.Add(clip);
            }
        }
    }

    static public void AddDialouge(List<Dialogue> list, bool SkipQueue = false, bool StopCurrent = false, bool ClearQueue = false)
    {
        if (instance != null)
        {
            if (!instance.DialogueSource.isPlaying)
            {
                instance.DialogueSource.clip = list[0].Clip;
                instance.DialogueSource.volume = list[0].Volume;

                instance.DialogueSource.PlayDelayed(list[0].Delay);
                HUDController.DisplaySubtitle(list[0].Subtitle, list[0].Delay);
                list.RemoveAt(0);
            }
        }

        instance.DialogueQueue.AddRange(list);
    }


    static public void PlayClip(MiscAudioClip clip, float delay = 0.0f)
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
                        instance.MiscSources[i].Source.PlayDelayed(delay);
                    }
                    else if (instance.MiscSources[i].CreateAnother)
                    {
                        instance.MiscSources.Add(CreateSource(clip));
                        instance.MiscSources[instance.MiscSources.Count - 1].Source.PlayDelayed(delay);
                        return;
                    }
                    instance.MiscSources[i].Source.volume = clip.Volume;
                    instance.MiscSources[i].Source.pitch = clip.Pitch;
                    instance.MiscSources[i].Source.loop = clip.Loop;
                    return;
                }
            }
            instance.MiscSources.Add(CreateSource(clip));
            instance.MiscSources[instance.MiscSources.Count - 1].Source.PlayDelayed(delay);
        }
    }

    static public void PlayClip(AudioClip clip, float delay = 0.0f)
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
                        instance.MiscSources[i].Source.PlayDelayed(delay);
                    }
                    else if (instance.MiscSources[i].CreateAnother)
                    {
                        instance.MiscSources.Add(CreateSource(clip));
                        instance.MiscSources[instance.MiscSources.Count - 1].Source.PlayDelayed(delay);
                    }
                    return;
                }

            }
            instance.MiscSources.Add(CreateSource(clip));
            instance.MiscSources[instance.MiscSources.Count - 1].Source.PlayDelayed(delay);
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



    static public void SetMute(bool mute = true)
    {
        if (!mute)
        {
            instance._muted = false;
            instance._masterVolume = instance.StoredMasterVolume;
        }
        else
        {
            instance._muted = true;
            instance.StoredMasterVolume = instance._masterVolume;
            instance._masterVolume = 0;
        }
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
        public bool Muted;
        public MiscAudioSource(AudioSource source, bool Overridable, bool createAnother) 
        { 
            Source = source;
            Source.hideFlags = HideFlags.HideInInspector;
            Override = Overridable;
            CreateAnother = createAnother;
        }

        public void Mute(bool mute = true)
        {
            if (mute)
            {
                Muted = true;
                Source.mute = true;
            }
            else
            {
                Muted = false;
                Source.mute = false;
            }
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

[System.Serializable]
public class Dialogue
{
    [SerializeField]
    public string Key = "";
    [SerializeField]
    public AudioClip Clip = null;
    [SerializeField]
    [Range(0, 1)]
    public float Volume = 1.0f;
    [SerializeField]
    public float Delay = 0.0f;
    [SerializeField]
    public string Subtitle = "";
}
