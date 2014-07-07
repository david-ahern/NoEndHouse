using UnityEngine;
using UnityEditor;
using System.Collections;

[InitializeOnLoad]
public class MyGUIStyles
{
    static private GUIStyle boldButton;
    static public GUIStyle BoldButton
    {
        get
        {
            if (boldButton == null)
            {
                boldButton = new GUIStyle(EditorStyles.miniButton);
                boldButton.fontStyle = FontStyle.Bold;
                boldButton.fontSize = 10;
                boldButton.alignment = TextAnchor.MiddleCenter;
            }
            return boldButton;
        }
    }

    static private GUIStyle button;
    static public GUIStyle Button
    {
        get
        {
            if (button == null)
            {
                button = new GUIStyle(EditorStyles.miniButton);
                boldButton.alignment = TextAnchor.MiddleCenter;
                boldButton.fontSize = 10;
            }
            return button;
        }
    }

    static private GUIStyle foldout;

    static public GUIStyle BoldFoldout
    {
        get
        {
            if (foldout == null)
            {
                foldout = new GUIStyle(EditorStyles.foldout);
                foldout.fontStyle = FontStyle.Bold;
            }
            return foldout;
        }
    }

    static public GUILayoutOption[] Options(float Width)
    {
        GUILayoutOption[] options = new GUILayoutOption[1];

        options[0] = GUILayout.Width(Width);

        return options;
    }

    static public GUILayoutOption[] Options(float Width, float Height)
    {
        GUILayoutOption[] options = new GUILayoutOption[2];

        options[0] = GUILayout.Width(Width);
        options[1] = GUILayout.Height(Height);

        return options;
    }

    static public void Seperator()
    {
        GUILayout.Box(GUIContent.none, new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
    }

    static public void Tab(int size = 1)
    {
        GUILayout.Space(size * 10);
    }
}
