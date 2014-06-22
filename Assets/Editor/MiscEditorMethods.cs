using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class MiscEditorMethods  
{
    static public List<GameObject> LoadAllPrefabsAt(string path = "")
    {
        return LoadPrefabs(path);
    }

    static public List<GameObject> LoadAllPrefabsWithComponent(System.Type componentType, string path = "")
    {
        List<GameObject> assetRefs = LoadPrefabs(path);
        List<GameObject> retList = new List<GameObject>();
        foreach (GameObject obj in assetRefs)
            if (((GameObject)obj).GetComponent(componentType))
                retList.Add(obj);

        return retList;
    }

    static List<GameObject> LoadPrefabs(string path)
    {
        List<FileInfo> files = DirSearch(new DirectoryInfo(Application.dataPath + path), "*.prefab");

        List<GameObject> assetRefs = new List<GameObject>();

        for (int i = 0; i < files.Count; i++)
        {
            if (files[i].Name.StartsWith(".")) continue;
            assetRefs.Add((GameObject)AssetDatabase.LoadMainAssetAtPath(getRelativeAssetPath(files[i].FullName)));
        }
        return assetRefs;
    }

    static public List<Object> GetObjectOfType(string Type, string Path = "", bool SearchSubdirectories = true)
    {
        List<Object> Objs = new List<Object>();

        Path = Path.Replace("Assets", "");

        string[] filenames = Directory.GetFiles(Application.dataPath + Path, Type, (SearchSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly));

        int progress = 0;

        foreach (string path in filenames)
        {
            if (EditorUtility.DisplayCancelableProgressBar("Hold On", path, (float)progress++ / filenames.Length)) { Objs.Clear(); break; }

            string objPath = "Assets" + path.Replace(Application.dataPath, "").Replace('\\', '/');

            Objs.Add(AssetDatabase.LoadAssetAtPath(objPath, typeof(Object)));
        }
        EditorUtility.ClearProgressBar();

        return Objs;
    }

    static private string fixSlashes(string s)
    {
        const string forwardSlash = "/";
        const string backSlash = "\\";
        return s.Replace(backSlash, forwardSlash);
    }

    static private string getRelativeAssetPath(string rootPath)
    {
        return fixSlashes(rootPath).Replace(Application.dataPath, "Assets");
    }

    static private List<FileInfo> DirSearch(DirectoryInfo d, string searchFor)
    {
        List<FileInfo> foundItems = d.GetFiles(searchFor).ToList();
        DirectoryInfo[] dis = d.GetDirectories();

        for (int i = 0; i < dis.Length; i++)
        {
            foundItems.AddRange(DirSearch(dis[i], searchFor));
        }

        return foundItems;
    }

    public static void OpenScriptInVS(MonoBehaviour Script, int GoToLine = 0)
    {
        string[] filenames = AssetDatabase.GetAllAssetPaths();
        Directory.GetFiles(Application.dataPath, Script.GetType().ToString() + ".cs", SearchOption.AllDirectories);
        
        if (filenames.Length == 1)
        {
            string finalFileName = Path.GetFullPath(filenames[0]);

            System.Diagnostics.Process.Start("devenv", " /edit \"" + finalFileName + "\" /command \"edit.goto" + GoToLine.ToString() + " \" ");
        }
        else
            EditorUtility.DisplayDialog("Unable to open file", "Files found: " + filenames.Length + (filenames.Length == 0 ? "Please ensure the file exists." : "Too many files found, this should not happen"), "Ok");
    }

    public static void ApplyChangesToObject(GameObject obj)
    {
        Object parent = PrefabUtility.GetPrefabParent(obj);

        PrefabUtility.ReplacePrefab(obj, parent);
    }
}
