using UnityEngine;
using UnityEditor;
using System.Collections;

[InitializeOnLoad]
public class Autosave : EditorWindow
{
    static private int AutosaveIntervalMins;
    static private float AutosaveIntervalSecs;
    static private float NextAutosaveTime;
    static private float AutosaveIntervalStart;

    static private bool AutosaveOn;

    static private float tempIntervalMins;

    Autosave()
    {
        EditorApplication.update += EditorUpdate;

        SyncValues();
        CalculateNextTime();
    }

    void EditorUpdate()
    {
        if (AutosaveOn && EditorApplication.timeSinceStartup > NextAutosaveTime)
            SaveScene();
    }

    void SaveScene()
    {
        EditorApplication.SaveScene();
        CalculateNextTime();
    }

    [MenuItem("Autosave/Open Autosave Window")]
    public static void OpenWindow()
    {
        EditorWindow window = EditorWindow.GetWindow(typeof(Autosave));
        window.position = new Rect(0, 0, 263, 100);
        window.minSize = new Vector2(263, 100);
    }


    void OnGUI()
    {
        SyncValues();
        Texture AddIcon = GUIIconEditor.GetIcon("Add Icon");
        Texture LessIcon = GUIIconEditor.GetIcon("Remove Icon");

        EditorGUILayout.Space();
        if (GUILayout.Button("Autosave " + (AutosaveOn ? "On" : "Off"), GUILayout.Width(100)))
        {
            AutosaveOn = !AutosaveOn;
            SaveValues();
            if (AutosaveOn)
            {
                CalculateNextTime();
            }
        }

        EditorGUILayout.Space();

        if (AutosaveOn)
        {
            Repaint();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Interval: " + AutosaveIntervalMins + " Mins.");
            if (GUILayout.Button(LessIcon, GUILayout.Width(20), GUILayout.Height(20)))
            {
                CalculateNextTime(-1);
            }
            if (GUILayout.Button(AddIcon, GUILayout.Width(20), GUILayout.Height(20)))
            {
                CalculateNextTime(1);
            }

            EditorGUILayout.EndHorizontal();

            EditorGUI.ProgressBar(new Rect(5, 70, this.position.width - 10, 20), ((float)EditorApplication.timeSinceStartup - AutosaveIntervalStart) / (NextAutosaveTime - AutosaveIntervalStart), "Next Autosave In: " + MiscMethods.TimeAsString((AutosaveIntervalSecs - ((float)(EditorApplication.timeSinceStartup - AutosaveIntervalStart)))));
        }

        EditorUtility.SetDirty(this);
    }

    private void CalculateNextTime(int TimeChange = 0)
    {
        if (TimeChange == 0)
        {
            if(EditorApplication.timeSinceStartup > NextAutosaveTime)
                AutosaveIntervalStart = (float)EditorApplication.timeSinceStartup;

            AutosaveIntervalSecs = AutosaveIntervalMins * 60;
            NextAutosaveTime = (float)EditorApplication.timeSinceStartup + AutosaveIntervalSecs;
        }
        else
        {
            AutosaveIntervalMins += TimeChange;
            if (AutosaveIntervalMins < 1)
                AutosaveIntervalMins = 1;
            AutosaveIntervalSecs = AutosaveIntervalMins * 60;
            NextAutosaveTime = AutosaveIntervalStart + AutosaveIntervalSecs;
        }
        SaveValues();
    }

    private void SaveValues()
    {
        EditorPrefs.SetBool("AutosaveOn", AutosaveOn);
        EditorPrefs.SetInt("AutosaveIntervalMins", AutosaveIntervalMins);
    }

    private void SyncValues()
    {
        AutosaveOn = EditorPrefs.GetBool("AutosaveOn", true);
        AutosaveIntervalMins = EditorPrefs.GetInt("AutosaveIntervalMins", 10);

        AutosaveIntervalSecs = AutosaveIntervalMins * 60;
    }
}
