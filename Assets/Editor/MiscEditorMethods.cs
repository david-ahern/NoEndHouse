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
        EditorProgressBar.Close();
        return assetRefs;
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


}
