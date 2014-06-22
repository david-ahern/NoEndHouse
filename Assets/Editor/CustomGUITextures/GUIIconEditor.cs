using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[InitializeOnLoad]
public class GUIIconEditor : EditorWindow 
{
    static private string HolderPath = "Assets/Editor/CustomGUITextures/Holders/IconHolder.asset";
    static private string TexturesPath = "Assets/Editor/CustomGUITextures/Textures";

    [SerializeField]
    static GUIIconHolder _IconHolder;

    [MenuItem("Custom/GUI Icon Editor")]
    public static void ShowWindow()
    {
        EditorWindow window = EditorWindow.GetWindow(typeof(GUIIconEditor));
        window.minSize = new Vector2(522, 187);
    }

    static GUIIconEditor()
    {
        _IconHolder = (GUIIconHolder)AssetDatabase.LoadAssetAtPath(HolderPath, typeof(GUIIconHolder));

        if (_IconHolder == null)
        {
            _IconHolder = ScriptableObject.CreateInstance<GUIIconHolder>();
            _IconHolder.Keys = new List<string>();
            _IconHolder.Textures = new List<Texture>();
            AssetDatabase.CreateAsset(_IconHolder, HolderPath);

            if (_IconHolder == null)
                Debug.Log("Failed to load or create an IconHolder");
        }

        GetAllTextures();
    }


    Vector2 KeyListScrollPos = new Vector2(0, 0);

    static string Selected = "";
    string newKey = "";
    Texture newTex = null;

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();

        GUILayout.Label("Add Icon:", new GUILayoutOption[] { GUILayout.Width(100) });

        newKey = GUILayout.TextField(newKey, 25, new GUILayoutOption[] { GUILayout.Width(150) });
        newTex = (Texture)EditorGUILayout.ObjectField(newTex, typeof(Texture), false, new GUILayoutOption[] { GUILayout.Width(150) });

        if (GUILayout.Button("Add", new GUILayoutOption[] { GUILayout.Width(100) }))
            if (newKey == "")
                EditorUtility.DisplayDialog("Invalid Entry", "The key field has been left blank.", "Ok");
            else if (newTex == null)
                EditorUtility.DisplayDialog("Invalid Entry", "The texture field has been left blank.", "Ok");
            else
            {
                _IconHolder.Keys.Add(newKey);
                _IconHolder.Textures.Add(newTex);
                //AssetDatabase.AddObjectToAsset(_IconHolder.Icons[_IconHolder.Icons.Count - 1], _IconHolder);
                EditorUtility.SetDirty(_IconHolder);
                AssetDatabase.SaveAssets();
                Selected = newKey;
                newKey = "";
                newTex = null;
            }

        EditorGUILayout.EndHorizontal();


        EditorGUILayout.BeginHorizontal();

        KeyListScrollPos = EditorGUILayout.BeginScrollView(KeyListScrollPos, GUILayout.Width(150), GUILayout.Height(this.position.height - 40));

        foreach (string key in _IconHolder.Keys)
            if (GUILayout.Button(key, GUILayout.Width(120))) Selected = key;

        EditorGUILayout.EndScrollView();

        if (Selected == "")
            Selected = _IconHolder.Keys[0];

        EditorGUILayout.BeginVertical();

        if (Selected != "")
        {
            /*GUIIcon SelectedHolder = new GUIIcon("", null);
            foreach (GUIIcon icon in _IconHolder.Icons)
                if (icon.Key == Selected)
                    SelectedHolder = icon;*/

            _IconHolder.Select(Selected);

            EditorGUILayout.BeginHorizontal(GUILayout.Width(150));
            GUILayout.Label("Key:");
            GUILayout.Label(Selected);
            EditorGUILayout.EndHorizontal();

            Texture temp;
            temp = (Texture)EditorGUILayout.ObjectField(_IconHolder.Selected, typeof(Texture), false);

            if (_IconHolder.Selected != temp)
            {
                _IconHolder.ReplaceSelected(temp);
                EditorUtility.SetDirty(_IconHolder);
                AssetDatabase.SaveAssets();
            }

            GUILayout.Label("Texture Info:");

            EditorGUILayout.BeginHorizontal(GUILayout.Width(150));
            GUILayout.Label("Width:\t\t");
            GUILayout.Label(_IconHolder.Selected.width.ToString());
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal(GUILayout.Width(150));
            GUILayout.Label("Height:\t\t");
            GUILayout.Label(_IconHolder.Selected.height.ToString());
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal(GUILayout.Width(150));
            GUILayout.Label("AntisoLevel:\t");
            GUILayout.Label(_IconHolder.Selected.anisoLevel.ToString());
            EditorGUILayout.EndHorizontal();

            EditorGUI.DrawTextureTransparent(new Rect(400, 65, 100, 100), _IconHolder.Selected);

            if (GUILayout.Button("Delete", GUILayout.Width(200)))
            {
                _IconHolder.Remove(Selected);
                //Object.DestroyImmediate(SelectedHolder, true);
                EditorUtility.SetDirty(_IconHolder);
                AssetDatabase.SaveAssets();
                Selected = "";
            }
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Load New", GUILayout.Width(120))) GetAllTextures();
        EditorGUILayout.Space();
    }

    static void GetAllTextures()
    {
        string temp = TexturesPath.Replace("Assets", "");
        string[] filenames = Directory.GetFiles(Application.dataPath + temp, "*.png", SearchOption.AllDirectories);
        int progress = 0;

        foreach (string path in filenames)
        {
            if (EditorUtility.DisplayCancelableProgressBar("Hold on", path, (float)progress / filenames.Length)) break;

            progress++;

            string texPath = "Assets" + path.Replace(Application.dataPath, "").Replace('\\', '/');

            Texture newTex = (Texture)AssetDatabase.LoadAssetAtPath(texPath, typeof(Texture));
            string newKey = newTex.name;
            
            if (!_IconHolder.Keys.Contains(newKey))
            {
                _IconHolder.Keys.Add(newKey);
                _IconHolder.Textures.Add(newTex);
                EditorUtility.SetDirty(_IconHolder);
                AssetDatabase.SaveAssets();
                Selected = newKey;
            }
        }
        EditorUtility.ClearProgressBar();
    }

    static public Texture GetIcon(string key)
    {
        return _IconHolder.GetTex(key);
        /*foreach (GUIIcon icon in _IconHolder.Icons)
            if (icon.Key == key)
                return icon.Tex;
        return null;*/
    }

    static public void DoSomething()
    {
        GUILayout.Button("Button");
    }
}


