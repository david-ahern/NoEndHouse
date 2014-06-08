using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(SoundController))]
public class SoundControllerGUI : Editor 
{
    bool Muted;
    static float GUIMasterVolume = 0;
    AudioClip addedClip = null;
    public override void OnInspectorGUI()
    {
        EditorGUILayout.Space();
        
        SoundController controller = (SoundController)target;
        
        EditorGUILayout.LabelField("Volumes", EditorStyles.boldLabel);
        
        GUILayout.Label("Master Volume");
        controller._masterVolume = GUILayout.HorizontalSlider(controller._masterVolume, 0, 1, GUILayout.ExpandWidth(true));
        
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Music Volume");
        GUILayout.Label("SFX Volume");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        controller._musicVolume = GUILayout.HorizontalSlider(controller._musicVolume, 0, 1);
        controller._sfxVolume = GUILayout.HorizontalSlider(controller._sfxVolume, 0, 1);
        EditorGUILayout.EndHorizontal();
        
        Muted = (controller._masterVolume != 0 ? false : true);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        if (GUILayout.Button((Muted ? "Unmute" : "Mute"))) SwitchMute();
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
        if (GUILayout.Button("Prev")) ChangeTrack(controller, -1);
        EditorGUILayout.Space();
        if (GUILayout.Button("Next")) ChangeTrack(controller, 1);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Availible Tracks:");

        int remove = -1;

        foreach (var track in controller.SoundTracks)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(track.Name);
            GUILayout.Label(track.Track.length.ToString());
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Up")) MoveTrackUp(controller, controller.SoundTracks.IndexOf(track));
            if (GUILayout.Button("Down")) MoveTrackDown(controller, controller.SoundTracks.IndexOf(track));
            if (GUILayout.Button("Remove")) remove = controller.SoundTracks.IndexOf(track);
            EditorGUILayout.EndHorizontal();
        }

        if (remove > -1)
            controller.SoundTracks.RemoveAt(remove);

        EditorGUILayout.Space();

        
        addedClip = (AudioClip)EditorGUILayout.ObjectField("Add Soundtrack:", addedClip, typeof(AudioClip), false);

        if (addedClip != null)
            controller.AddSoundtrack(addedClip);
        addedClip = null;        
    }

    void SwitchMute()
    {
        SoundController controller = (SoundController)target;
        if (Muted)
        {
            Muted = false;
            controller._masterVolume = GUIMasterVolume;
        }
        else
        {
            Muted = true;

            GUIMasterVolume = controller._masterVolume;

            controller._masterVolume = 0;
        }
    }

    void ChangeTrack(SoundController controller, int target)
    {
        target = controller._currentSountrackIndex + target;

        if (target >= controller.SoundTracks.Count)
            target = 0;
        else if (target < 0)
            target = controller.SoundTracks.Count - 1;

        controller.SoundTracks[target].Play();
    }

    void MoveTrackUp(SoundController controller, int index)
    {
        if (index == 0) return;

        var temp = controller.SoundTracks[index - 1];
        controller.SoundTracks[index - 1] = controller.SoundTracks[index];
        controller.SoundTracks[index] = temp;
    }

    void MoveTrackDown(SoundController controller, int index)
    {
        if (index == controller.SoundTracks.Count - 1) return;

        var temp = controller.SoundTracks[index + 1];
        controller.SoundTracks[index + 1] = controller.SoundTracks[index];
        controller.SoundTracks[index] = temp;
    }
}
