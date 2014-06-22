using UnityEngine;
using UnityEditor;
using System.Collections;

public class ApplyPrefabChanges
{
    [MenuItem("Custom/Apply Prefab Changes")]
    public static void ApplyChanges()
    {
        if (EditorUtility.DisplayDialog("Apply Prefab Changes",
            "Are you sure you want to apply changes? This action cannot be undone.",
            "Yes", "No"))
        {
            GameObject ob = Selection.activeGameObject;
            Object parent = PrefabUtility.GetPrefabParent(ob);
            if (ob.GetComponent<SoundController>())

            PrefabUtility.ReplacePrefab(ob, parent);

            EditorGUIUtility.PingObject(parent);
        }
    }


    [MenuItem("Custom/Apply Prefab Changes", true)]
    public static bool ValidateMenu()
    {
        GameObject ob = Selection.activeGameObject;
        return  ob != null && PrefabUtility.GetPrefabParent(ob) != null && Application.isPlaying;
    }

    public static void ApplyChangesToObject()
    {
        if (EditorUtility.DisplayDialog("Apply Prefab Changes",
            "Are you sure you want to apply changes? This action cannot be undone.",
            "Yes", "No"))
        {
            GameObject ob = Selection.activeGameObject;
            Object parent = PrefabUtility.GetPrefabParent(ob);
            if (ob.GetComponent<SoundController>())

                PrefabUtility.ReplacePrefab(ob, parent);

            EditorGUIUtility.PingObject(parent);
        }
    }
}
