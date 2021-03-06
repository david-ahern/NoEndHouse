﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider))]
public class DialogueTrigger : MonoBehaviour 
{
    [HideInInspector]
    public bool ShowDefaultInspector = false;
    [ContextMenu("Switch Inspector View")]
    void ShowDefault()
    {
        ShowDefaultInspector = !ShowDefaultInspector;
    }
    public bool Trigger = true;

    public List<string> Keys = new List<string>();

    public bool SkipQueue = false;
    public bool StopCurrent = false;
    public bool ClearQueue = false;

    private bool Played = false;

    void Start()
    {
        if (!Trigger)
            gameObject.collider.enabled = false;
        else
        {
            gameObject.collider.enabled = true;
            gameObject.collider.isTrigger = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && Trigger && !Played)
        {
            Played = true;
            SoundController.AddDialouge(Keys, SkipQueue, StopCurrent, ClearQueue);
        }
    }

    public void TriggerDialogue()
    {
        if (!Trigger && !Played)
        {
            Played = true;
            SoundController.AddDialouge(Keys, SkipQueue, StopCurrent, ClearQueue);
        }
    }
}
