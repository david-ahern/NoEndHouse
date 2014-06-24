using UnityEngine;
using System.Collections;
using System;

public class MiscMethods
{
    static public string TimeAsString(float seconds, bool milliseconds = false)
    {
        TimeSpan span = TimeSpan.FromSeconds(seconds);

        string time = span.Minutes.ToString() + ":" + span.Seconds.ToString("d2");

        if (milliseconds)
            time += span.Milliseconds.ToString("d2");

        return time;
    }

    static public float GetLerpTimeValue(float Current, float Start, float Duration)
    {
        return (Current - Start) / Duration;
    }
}
