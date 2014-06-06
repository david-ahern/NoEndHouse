using UnityEngine;
using UnityEditor;
using System.Collections;

public class EditorProgressBar : EditorWindow 
{
    static float progress = 0;
    static string description = "";
    static bool Show = false;

    static public float Progress
    {
        set { progress = value; }
    }

    static public string Description
    {
        set { description = value; }
    }

    internal static void Init()
    {
        Show = true;
    }

    internal static void Close()
    {
        Show = false;
    }

    void OnGUI()
    {
        if (Show)
        {
            Debug.Log("Show");
            EditorUtility.DisplayProgressBar("Hold On", description, progress);
        }
    }

}
