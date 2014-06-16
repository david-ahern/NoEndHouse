using System.Collections;
using System.IO;
using UnityEngine;
using System;
public class CopyOutputLog 
{
    static public void CopyOutputLogFile()
    {
        Debug.Log(Application.dataPath);

        string FileName = "/output_log.txt";

        string LogPath = Application.dataPath;
        string DestPath = Application.dataPath.Replace("NoEndHouse_Data", "") + "Output Log " + DateTime.Now.ToString("h_mm_ss");

        if (!Directory.Exists(DestPath))
            Directory.CreateDirectory(DestPath);

        File.Copy(LogPath + FileName, DestPath + FileName);
    }
}
