using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class AudioInspector : EditorWindow 
{

    [MenuItem("Audio/Audio Inspector")]
    public static void OpenWindow()
    {
        EditorWindow window = EditorWindow.GetWindow(typeof(AudioInspector));
    }


    AudioClip Clip;

    Vector2 ScrollPos = Vector2.zero;

    void OnGUI()
    {
        Clip = (AudioClip)EditorGUILayout.ObjectField(Clip, typeof(AudioClip));

        if (Clip != null)
        {
            ScrollPos = EditorGUILayout.BeginScrollView(ScrollPos);

            GUILayout.Label(Clip.samples.ToString());

            EditorGUILayout.EndScrollView();
        }

    }

    void SetData()
    {
        Debug.Log("setting data");
        int numData = 20000;

        float[] Data = new float[numData];

        for (int i = 0; i < Data.Length - 1; i += 2)
        {
            Debug.Log("doing it");
            Data[i] = Mathf.Lerp(0, 1, i / numData);
            Data[i + 1] = -Mathf.Lerp(0, 1, i / numData);
        }

        Clip.SetData(Data, 0);
    }
}
