using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(SoundController))]
public class SoundControllerGUI : Editor 
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
    }
}
