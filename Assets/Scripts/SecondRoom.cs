using UnityEngine;
using System.Collections;

public class SecondRoom : Puzzle 
{
    public ItemHolder Holder1;
    public ItemHolder Holder2;
    public ItemHolder Holder3;

    public Door door;
	void Start () 
    {
        door.IsLocked = true;
	}
	
	void Update () 
    {
	    if (Holder1.HasItem && Holder2.HasItem && Holder3.HasItem)
        {
            door.IsLocked = false;
            _Completed = true;
        }
        else
        {
            door.IsLocked = true;
            _Completed = false;
        }
	}
}
