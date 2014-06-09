﻿using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(SoundController))]
public class SoundControllerGUI : Editor 
{
    private SoundController Target
    {
        get { return (SoundController)target; }
    }

    static float GUIMasterVolume = 0;
    AudioClip addedClip = null;
    public override void OnInspectorGUI()
    {
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
        if (GUILayout.Button((mute ? "Unmute" : "Mute"))) SetMute(!mute);
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
        if (GUILayout.Button("Prev")) ChangeTrack(-1);
        EditorGUILayout.Space();
        if (GUILayout.Button("Next")) ChangeTrack(1);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Availible Tracks:");

        int remove = -1;

        foreach (var track in Target.SoundTracks)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(track.Name);
            GUILayout.Label(track.Track.length.ToString());
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Up")) MoveTrackUp(Target.SoundTracks.IndexOf(track));
            if (GUILayout.Button("Down")) MoveTrackDown(Target.SoundTracks.IndexOf(track));
            if (GUILayout.Button("Remove")) remove = Target.SoundTracks.IndexOf(track);
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
                GUILayout.Box(GUIContent.none, new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Currently Playing Sources", EditorStyles.boldLabel);

                EditorGUILayout.Space();
                foreach (var source in Target.MiscSources)
                {
                    GUILayout.Label(source.Source.clip.name + ":");
                    EditorGUILayout.BeginHorizontal();

                        EditorGUILayout.BeginVertical();
                           GUILayout.Label("Volume:\t" + source.Source.volume.ToString("n2"));
                           GUILayout.Label("Pitch:\t\t" + source.Source.pitch.ToString("n2"));
                        EditorGUILayout.EndVertical();

                        EditorGUILayout.BeginVertical();
                            GUILayout.Label("Play time:");
                            GUILayout.Label(source.Source.time.ToString("n2") + " / " + source.Source.clip.length.ToString("n2") + " seconds");
                        EditorGUILayout.EndVertical();

                    EditorGUILayout.EndHorizontal();

                    GUILayout.Label("Flags:");
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("Loop:");
                    EditorGUILayout.Toggle(source.Source.loop);
                    GUILayout.Label("Override");
                    EditorGUILayout.Toggle(source.Override);
                    GUILayout.Label("Create Another");
                    EditorGUILayout.Toggle(source.CreateAnother);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
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
