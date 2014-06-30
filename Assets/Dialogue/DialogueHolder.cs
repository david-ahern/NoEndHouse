using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DialogueHolder : ScriptableObject 
{
    public List<Dialogue> Dialogues;

    public Dialogue selected;

    Dialogue temp;

    public Dialogue dialogue(string key)
    {
        if (temp.Key == key)
            return temp;

        foreach(Dialogue d in Dialogues)
            if(d.Key == key)
            {
                temp = d;
                return temp;
            }

        return null;
    }
}
