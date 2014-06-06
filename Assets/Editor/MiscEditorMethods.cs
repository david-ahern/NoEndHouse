using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class MiscEditorMethods  
{
    static public List<Object> LoadAllPrefabsAt(string path)
    {
        EditorProgressBar.Init();
        EditorProgressBar.Description = "Searching through project directory.";
        List<FileInfo> files = DirSearch(new DirectoryInfo(Application.dataPath + path), "*.prefab");

        List<Object> assetRefs = new List<Object>();

        EditorProgressBar.Description = "Converting files to prefabs.";
        for (int i = 0; i < files.Count; i++)
        {
            EditorProgressBar.Progress = i / files.Count;
            if (files[i].Name.StartsWith(".")) continue;
            assetRefs.Add(AssetDatabase.LoadMainAssetAtPath(getRelativeAssetPath(files[i].FullName)));
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
            EditorProgressBar.Progress = i / dis.Length;
            foundItems.AddRange(DirSearch(dis[i], searchFor));
        }

        return foundItems;
    }
}
