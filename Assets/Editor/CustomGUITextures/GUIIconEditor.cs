using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[InitializeOnLoad]
public class GUIIconEditor : EditorWindow 
{
    static private string Path = "Assets/Editor/CustomGUITextures/Holders/IconHolder.asset";

    [SerializeField]
    static GUIIconHolder _IconHolder;

    [MenuItem("Custom/GUI Icon Editor")]
    public static void ShowWindow()
    {
        _IconHolder = (GUIIconHolder)AssetDatabase.LoadAssetAtPath(Path, typeof(GUIIconHolder));

        if (_IconHolder == null)
        {
            _IconHolder = ScriptableObject.CreateInstance<GUIIconHolder>();
            _IconHolder.Icons = new List<GUIIcon>();
            AssetDatabase.CreateAsset(_IconHolder, Path);

            if (_IconHolder == null)
                Debug.Log("Failed to load or create an IconHolder");
        }

        if (_IconHolder.Icons.Count > 0)
            Selected = _IconHolder.Icons[0].Key;
        else
            Selected = "";

        EditorWindow window = EditorWindow.GetWindow(typeof(GUIIconEditor));
        window.minSize = new Vector2(522, 522);
    }

    static GUIIconEditor()
    {
        Debug.Log("Init");
        _IconHolder = (GUIIconHolder)AssetDatabase.LoadAssetAtPath(Path, typeof(GUIIconHolder));

        if (_IconHolder == null)
        {
            Debug.Log("Icon holder is null");
            _IconHolder = ScriptableObject.CreateInstance<GUIIconHolder>();
            Debug.Log("Icon holder created");
            _IconHolder.Icons = new List<GUIIcon>();
            Debug.Log("List created");
            AssetDatabase.CreateAsset(_IconHolder, Path);
            Debug.Log("Asset created");

            if (_IconHolder == null)
                Debug.Log("Failed to load or create an IconHolder");
        }
        Debug.Log("Done");
    }


    Vector2 KeyListScrollPos = new Vector2(0, 0);

    static string Selected = "";
    string newKey = "";
    Texture newTex = null;

    int newSizeX = 0;
    int newSizeY = 0;

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();

        GUILayout.Label("Add Icon:", new GUILayoutOption[] { GUILayout.Width(100) });

        newKey = GUILayout.TextField(newKey, 25, new GUILayoutOption[] { GUILayout.Width(150) });
        newTex = (Texture)EditorGUILayout.ObjectField(newTex, typeof(Texture), new GUILayoutOption[] { GUILayout.Width(150) });

        if (GUILayout.Button("Add", new GUILayoutOption[] { GUILayout.Width(100) }))
            if (newKey == "")
                EditorUtility.DisplayDialog("Invalid Entry", "The key field has been left blank.", "Ok");
            else if (newTex == null)
                EditorUtility.DisplayDialog("Invalid Entry", "The texture field has been left blank.", "Ok");
            else
            {
                _IconHolder.Icons.Add(new GUIIcon(newKey, newTex));
                AssetDatabase.AddObjectToAsset(_IconHolder.Icons[_IconHolder.Icons.Count - 1], _IconHolder);
                EditorUtility.SetDirty(_IconHolder);
                AssetDatabase.SaveAssets();
                Selected = newKey;
                newKey = "";
                newTex = null;
            }

        EditorGUILayout.EndHorizontal();


        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginScrollView(KeyListScrollPos, GUILayout.Width(120), GUILayout.Height(this.position.height - 40));

        foreach (GUIIcon icon in _IconHolder.Icons)
            if (GUILayout.Button(icon.Key)) Selected = icon.Key;

        EditorGUILayout.EndScrollView();

        EditorGUILayout.BeginVertical();

        if (Selected != "")
        {
            GUIIcon SelectedHolder = new GUIIcon("", null);
            foreach (GUIIcon icon in _IconHolder.Icons)
                if (icon.Key == Selected)
                    SelectedHolder = icon;

            EditorGUILayout.BeginHorizontal(GUILayout.Width(150));
            GUILayout.Label("Key:");
            GUILayout.Label(SelectedHolder.Key);
            EditorGUILayout.EndHorizontal();

            Texture temp;
            temp = (Texture)EditorGUILayout.ObjectField(SelectedHolder.Tex, typeof(Texture));

            if (SelectedHolder.Tex != temp)
            {
                SelectedHolder.Tex = temp;
                EditorUtility.SetDirty(SelectedHolder);
                AssetDatabase.SaveAssets();
            }

            GUILayout.Label("Texture Info:");

            EditorGUILayout.BeginHorizontal(GUILayout.Width(150));
            GUILayout.Label("Width:\t\t");
            GUILayout.Label(SelectedHolder.Tex.width.ToString());
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal(GUILayout.Width(150));
            GUILayout.Label("Height:\t\t");
            GUILayout.Label(SelectedHolder.Tex.height.ToString());
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal(GUILayout.Width(150));
            GUILayout.Label("AntisoLevel:\t");
            GUILayout.Label(SelectedHolder.Tex.anisoLevel.ToString());
            EditorGUILayout.EndHorizontal();
            
            if (GUILayout.Button("Delete", GUILayout.Width(250)))
            {
                _IconHolder.Icons.Remove(SelectedHolder);
                Object.DestroyImmediate(SelectedHolder, true);
                EditorUtility.SetDirty(_IconHolder);
                AssetDatabase.SaveAssets();
                Selected = "";
            }

            EditorGUI.DrawTextureTransparent(new Rect(400, 65, 100, 100), SelectedHolder.Tex);
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();


        GUILayout.Label(this.position.width.ToString());
        GUILayout.Label(this.position.height.ToString());
    }

    static public Texture GetIcon(string key)
    {
        foreach (GUIIcon icon in _IconHolder.Icons)
            if (icon.Key == key)
                return icon.Tex;
        return null;
    }
}


