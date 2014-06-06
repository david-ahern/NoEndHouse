using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class PuzzleEditorGUI : EditorWindow 
{
    [MenuItem("Puzzles/Puzzle Editor")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(PuzzleEditorGUI));
    }
    [MenuItem("Puzzles/Puzzle Editor", true)]
    public static bool ValidateMenu()
    {
        return !Application.isPlaying;
    }


    string str = "Hello world";
    bool groupEnabled;
    bool aBool = true;
    float aFloat = 1.034f;

    List<string> Areas = new List<string>();

    void OnEnable()
    {
        GetAreas();
    }

    void OnGUI()
    {
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        str = EditorGUILayout.TextField("Text Field", str);

        groupEnabled = EditorGUILayout.BeginToggleGroup("OptionalSettings", groupEnabled);
        aBool = EditorGUILayout.Toggle("Toggle", aBool);
        aFloat = EditorGUILayout.Slider("Slider", aFloat, 0, 10);
        EditorGUILayout.EndToggleGroup();
    }

    void GetAreas()
    {
        List<Object> objs = MiscEditorMethods.LoadAllPrefabsAt("");

        foreach (Object o in objs)
            Debug.Log(o.name);
    }
}
