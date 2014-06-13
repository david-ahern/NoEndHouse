using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
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
            Globals.Player.CurrentArea = this;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            Active = false;
        }
    }

    public AreaController LoadArea(GameObject AreaToLoad, Vector3 RelativePosition)
    {
        AreaController temp = ((GameObject)GameObject.Instantiate(AreaToLoad)).GetComponent<AreaController>();
        temp.gameObject.transform.position = gameObject.transform.position + RelativePosition;

        LoadAreaTrigger[] triggersInNewArea = temp.gameObject.GetComponentsInChildren<LoadAreaTrigger>();

        foreach (LoadAreaTrigger trigger in triggersInNewArea)
            if (trigger.AreaToLoad.name == AreaName)
                trigger.LoadedArea = this;

        return temp;
    }
}
