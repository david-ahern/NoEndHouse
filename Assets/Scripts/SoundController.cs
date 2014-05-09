using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class SoundController : MonoBehaviour 
{
    static public SoundController instance;

    public List<Soundtrack> SoundTracks;

    private AudioSource SoundtrackSource;

    private List<MiscSource> MiscSounds;

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

            MiscSounds = new List<MiscSource>();
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

        for (int i = 0; i < instance.MiscSounds.Count; ++i)
        {
            if (!instance.MiscSounds[i].Source.isPlaying)
            {
                Destroy(instance.MiscSounds[i].Source);
                instance.MiscSounds.RemoveAt(i);
                --i;
            }
        }
	}

    static public void PlaySoundtrack(int track)
    {
        instance.SoundtrackSource.clip = instance.SoundTracks[track].Track;
        instance.SoundtrackSource.Play();
    }

    static public void PlayClip(AudioClip clip, bool Overridable)
    {
        for (int i = 0; i < instance.MiscSounds.Count; ++i)
        {
            if (instance.MiscSounds[i].Source.clip == clip)
            {
                if (instance.MiscSounds[i].Override)
                {
                    instance.MiscSounds[i].Source.Stop();
                    instance.MiscSounds[i].Source.Play();
                }
                return;
            }

        }
        instance.MiscSounds.Add(new MiscSource(instance.gameObject.AddComponent<AudioSource>(), true));
        instance.MiscSounds[instance.MiscSounds.Count - 1].Source.clip = clip;
        instance.MiscSounds[instance.MiscSounds.Count - 1].Override = Overridable;
        instance.MiscSounds[instance.MiscSounds.Count - 1].Source.Play();
    }

    [System.Serializable]
    public class Soundtrack
    {
        [SerializeField]
        public string Name;
        [SerializeField]
        public AudioClip Track;
    }

    public class MiscSource
    {
        public AudioSource Source;
        public bool Override;

        public MiscSource(AudioSource s, bool ov) { Source = s; Override = ov; }
    }
}
