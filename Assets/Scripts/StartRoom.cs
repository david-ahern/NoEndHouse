using UnityEngine;
using System.Collections;

public class StartRoom : Puzzle
{
    public Window Win1;
    public Window Win2;

    public Door door;
    public Message CompletedMessage;
    public Message HintMessage;

    void Start()
    {
        door.IsLocked = true;

        StartTime = Time.time;
    }
	void Update () 
    {
        if (Win1.IsOpen && Win2.IsOpen)
        {
            door.IsLocked = false;
            _Completed = true;
            CompletedMessage.ShowMessages();
        }
        else
        {
            _Completed = false;
            door.IsLocked = true;
            CompletedMessage.StopMessages();
        }

        if (Time.time - StartTime > HintAfterTime)
        {
            HintMessage.ShowMessages();
        }
	}
}
