using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(SoundController))]
public class SoundControllerGUI : Editor 
{
    private SoundController Target
    {
        get { return (SoundController)target; }
    }

    static float GUIMasterVolume = 0;
    AudioClip addedClip = null;

    static List<bool> Foldouts = new List<bool>();
    public override void OnInspectorGUI()
    {
        Texture RemoveIcon = GUIIconEditor.GetIcon("Remove");
        Texture MuteIcon = GUIIconEditor.GetIcon("Mute");
        Texture UnmuteIcon = GUIIconEditor.GetIcon("Unmute");
        Texture PrevIcon = GUIIconEditor.GetIcon("PlayPrev");
        Texture NextIcon = GUIIconEditor.GetIcon("PlayNext");
        Texture UpArrow = GUIIconEditor.GetIcon("UpArrow");
        Texture DownArrow = GUIIconEditor.GetIcon("DownArrow");

        if (Target.ShowDefaultInspector)
        {
            DrawDefaultInspector();
            return;
        }

        Target.transform.hideFlags = HideFlags.HideInInspector;
        Repaint();

        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("Volumes", EditorStyles.boldLabel);
        
        GUILayout.Label("Master Volume");
        Target._masterVolume = GUILayout.HorizontalSlider(Target._masterVolume, 0, 1, GUILayout.ExpandWidth(true));
        
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Music Volume");
        GUILayout.Label("SFX Volume");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        Target._musicVolume = GUILayout.HorizontalSlider(Target._musicVolume, 0, 1);
        Target._sfxVolume = GUILayout.HorizontalSlider(Target._sfxVolume, 0, 1);
        EditorGUILayout.EndHorizontal();

        bool mute = Target._muted;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        if (GUILayout.Button((mute ? UnmuteIcon : MuteIcon), 
            (mute ? GUILayout.Width(UnmuteIcon.width / 2) : GUILayout.Width(MuteIcon.width)), 
            (mute ? GUILayout.Height(UnmuteIcon.height / 2) : GUILayout.Height(MuteIcon.height)))) 
                SetMute(!mute);
        EditorGUILayout.Space();
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();

        GUILayout.Box(GUIContent.none, new GUILayoutOption[]{GUILayout.ExpandWidth(true), GUILayout.Height(1)});

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Sountracks", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Currently Playing:");
        GUILayout.Label(SoundController.SoundtrackName);
        GUILayout.Label(SoundController.SoundtrackPlayPosition.ToString());
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(PrevIcon)) ChangeTrack(-1);
        EditorGUILayout.Space();
        if (GUILayout.Button(NextIcon)) ChangeTrack(1);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Availible Tracks:");

        int remove = -1;

        foreach (var track in Target.SoundTracks)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(track.Name);
            GUILayout.Label(track.Track.length.ToString());
            if (GUILayout.Button(UpArrow, GUILayout.Width(UpArrow.width), GUILayout.Height(UpArrow.height))) 
                MoveTrackUp(Target.SoundTracks.IndexOf(track));
            if (GUILayout.Button(DownArrow, GUILayout.Width(DownArrow.width), GUILayout.Height(DownArrow.height))) 
                MoveTrackDown(Target.SoundTracks.IndexOf(track));
            if (GUILayout.Button(RemoveIcon, GUILayout.Width(RemoveIcon.width), GUILayout.Height(RemoveIcon.height)))
                remove = Target.SoundTracks.IndexOf(track);
            EditorGUILayout.EndHorizontal();
        }

        if (remove > -1)
            Target.SoundTracks.RemoveAt(remove);

        EditorGUILayout.Space();
        
        addedClip = (AudioClip)EditorGUILayout.ObjectField("Add Soundtrack:", addedClip, typeof(AudioClip), false);

        if (addedClip != null)
            Target.AddSoundtrack(addedClip);
        addedClip = null;

        EditorGUILayout.Space();

        try
        {
            if (Target.MiscSources.Count > 0)
            {
                if (Foldouts.Count < Target.MiscSources.Count)
                    for (int i = Foldouts.Count; i < Target.MiscSources.Count; i++)
                    {
                        bool fout = false;
                        Foldouts.Add(fout);
                    }
                else if (Foldouts.Count > Target.MiscSources.Count)
                    for (int i = Foldouts.Count; i > Target.MiscSources.Count; i--)
                    {
                        Foldouts.RemoveAt(i - 1);
                    }

                GUILayout.Box(GUIContent.none, new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Currently Playing Sources", EditorStyles.boldLabel);

                EditorGUILayout.Space();
                for (int i = 0; i < Target.MiscSources.Count; ++i)
                {
                    Foldouts[i] = EditorGUILayout.Foldout(Foldouts[i], Target.MiscSources[i].Source.clip.name);
                    if (Foldouts[i])
                    {
                        EditorGUILayout.BeginHorizontal();

                        EditorGUILayout.BeginVertical();
                        GUILayout.Label("Volume:\t" + Target.MiscSources[i].Source.volume.ToString("n2"));
                        GUILayout.Label("Pitch:\t\t" + Target.MiscSources[i].Source.pitch.ToString("n2"));
                        EditorGUILayout.EndVertical();

                        EditorGUILayout.BeginVertical();
                        GUILayout.Label("Play time:");
                        GUILayout.Label(Target.MiscSources[i].Source.time.ToString("n2") + " / " + Target.MiscSources[i].Source.clip.length.ToString("n2") + " seconds");
                        EditorGUILayout.EndVertical();

                        EditorGUILayout.EndHorizontal();

                        GUILayout.Label("Flags:");
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label("Loop:");
                        EditorGUILayout.Toggle(Target.MiscSources[i].Source.loop);
                        GUILayout.Label("Override");
                        EditorGUILayout.Toggle(Target.MiscSources[i].Override);
                        GUILayout.Label("Create Another");
                        EditorGUILayout.Toggle(Target.MiscSources[i].CreateAnother);
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.Space();
                        if (GUILayout.Button((Target.MiscSources[i].Muted ? UnmuteIcon : MuteIcon),
                            (Target.MiscSources[i].Muted ? GUILayout.Width(UnmuteIcon.width) : GUILayout.Width(MuteIcon.width)),
                            (Target.MiscSources[i].Muted ? GUILayout.Height(UnmuteIcon.height) : GUILayout.Height(MuteIcon.height)))) 
                                Target.MiscSources[i].Mute(!Target.MiscSources[i].Muted);
                        EditorGUILayout.Space();
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
        }
        catch { }

        EditorUtility.SetDirty(Target);
    }
    public void SetMute(bool mute = true)
    {
        if (!mute)
        {
            Target._muted = false;
            Target._masterVolume = GUIMasterVolume;
        }
        else
        {
            Target._muted = true;
            GUIMasterVolume = Target._masterVolume;
            Target._masterVolume = 0;
        }
    }


    void ChangeTrack(int target)
    {
        target = Target._currentSountrackIndex + target;

        if (target >= Target.SoundTracks.Count)
            target = 0;
        else if (target < 0)
            target = Target.SoundTracks.Count - 1;

        Target.SoundTracks[target].Play();
    }

    void MoveTrackUp(int index)
    {
        if (index == 0) return;

        var temp = Target.SoundTracks[index - 1];
        Target.SoundTracks[index - 1] = Target.SoundTracks[index];
        Target.SoundTracks[index] = temp;
    }

    void MoveTrackDown(int index)
    {
        if (index == Target.SoundTracks.Count - 1) return;

        var temp = Target.SoundTracks[index + 1];
        Target.SoundTracks[index + 1] = Target.SoundTracks[index];
        Target.SoundTracks[index] = temp;
    }
}
