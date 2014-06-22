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

    List<GameObject> Areas;

    void OnEnable()
    {
        GetAreas();
    }


    void OnGUI()
    {
        foreach (GameObject obj in Areas)
        {
            EditorGUILayout.BeginToggleGroup(obj.name, true);
            EditorGUILayout.ObjectField(obj, typeof(AreaController), false);
            EditorGUILayout.EndToggleGroup();
        }
    }

    void GetAreas()
    {
        Areas = MiscEditorMethods.LoadAllPrefabsWithComponent(typeof(AreaController));
    }
}
