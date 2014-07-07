using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(AreaController))]
public class AreaControllerGUI : Editor {

	private AreaController Target
    {
        get { return (AreaController)target; }
    }

    static List<AreaTriggerFoldout> Triggers;

    static bool ShowTriggers = false;

    public override void OnInspectorGUI()
    {
        Target.AreaName = Target.gameObject.name;
        GetTriggers();

        GUILayout.Label("Area Name:\t" + Target.AreaName);
        EditorGUILayout.Space();

        ShowTriggers = EditorGUILayout.Foldout(ShowTriggers, "Area Triggers", MyGUIStyles.BoldFoldout);

        if (ShowTriggers)
        {
            foreach (AreaTriggerFoldout t in Triggers)
            {
                EditorGUI.indentLevel++;
                t.Foldout = EditorGUILayout.Foldout(t.Foldout, t.Trigger.name);

                if (t.Foldout)
                {
                    EditorGUI.indentLevel++;
                    GUILayout.Label("Trigger Name:\t\t" + t.Trigger.gameObject.name);
                    if (t.Trigger.AreaToLoad)
                        GUILayout.Label("Area to Load\t\t" + t.Trigger.AreaToLoad.name);
                    else
                        GUILayout.Label("No area selected to load");

                    EditorGUILayout.Vector3Field("Relative Position", t.Trigger.RelativePosition);
                    EditorGUI.indentLevel--;
                }
                EditorGUI.indentLevel--;
            }
        }

        if (GUILayout.Button("Add Trigger")) AddTrigger();

        EditorUtility.SetDirty(this);
    }

    private void GetTriggers()
    {
        List<LoadAreaTrigger> tmpTriggers = new List<LoadAreaTrigger>(Target.GetComponentsInChildren<LoadAreaTrigger>());

        if (Triggers == null)
            Triggers = new List<AreaTriggerFoldout>();

        if (tmpTriggers.Count != Triggers.Count)
        {
            Triggers.Clear();

            foreach (LoadAreaTrigger t in tmpTriggers)
            {
                Triggers.Add(new AreaTriggerFoldout(t));
            }
        }
    }

    private void AddTrigger()
    {
        GameObject newTrigger = new GameObject();
        newTrigger.name = "NewTrigger";
        newTrigger.transform.localPosition = Vector3.zero;
        newTrigger.AddComponent<LoadAreaTrigger>();

        GameObject par = GameObject.Find(Target.gameObject.name + "LoadAreaTriggers");

        newTrigger.transform.parent = par.transform;

        Triggers.Add(new AreaTriggerFoldout(newTrigger.GetComponent<LoadAreaTrigger>()));
    }

    class AreaTriggerFoldout
    {
        public LoadAreaTrigger Trigger;
        public bool Foldout = false;

        public AreaTriggerFoldout(LoadAreaTrigger t) { this.Trigger = t; }
    }
}
