using UnityEngine;
using System.Collections;

public class AreaController : MonoBehaviour {

    private bool Active;
    public string AreaName;

    public bool IsActive
    {
        get { return Active; }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Active = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            Active = false;
        }
    }
}
