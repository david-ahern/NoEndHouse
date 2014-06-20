using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DialogueTrigger : MonoBehaviour 
{
    public bool Trigger = true;

    public List<string> Dialogues;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && Trigger)
        foreach (string s in Dialogues) { }
            //get dialogue from holder and send to sound controller
    }

    void TriggerDialogue()
    {
        if (!Trigger)
        {
            foreach (string s in Dialogues) { }
            //get dialogue from holder and send to sound controllers
        }
    }
}
