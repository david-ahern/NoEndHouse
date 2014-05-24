using UnityEngine;
using System.Collections;

public class DoorHandle : MonoBehaviour 
{
    private bool _hasTurned;
    private bool _isTurned;

    public bool IsTurned
    {
        get { return _isTurned; }
        set { _hasTurned = value; }
    }

    void Update()
    {
        if (_hasTurned) _isTurned = true;
        else _isTurned = false;
        _hasTurned = false;
    }
}
