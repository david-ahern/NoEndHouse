using UnityEngine;
using System.Collections;

public class LoadAreaTrigger : MonoBehaviour 
{
    public GameObject AreaToLoad;
    public Vector3 RelativePosition;
    public float Delay;

    public AreaController LoadedArea;
    private AreaController ParentArea;

    void Awake()
    {
        ParentArea = gameObject.transform.parent.gameObject.GetComponent<AreaController>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (LoadedArea == null)
            {
                LoadedArea = ParentArea.LoadArea(AreaToLoad, RelativePosition);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            if (!LoadedArea.IsActive)
                GameObject.Destroy(LoadedArea.gameObject);
        }
    }
}
