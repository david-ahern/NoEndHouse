using UnityEngine;
using System.Collections;
using System;

public class MiscMethods
{
    static public string SecondsToMinsString(float seconds, bool roundup = false)
    {
        TimeSpan span = TimeSpan.FromSeconds(seconds);

        string time = span.Minutes.ToString();
        
        if (!roundup)
            time += ":" + span.Seconds.ToString("d2");

        return time;
    }
}
