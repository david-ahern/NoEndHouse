using UnityEngine;
using System.Collections;

public class HUDController : MonoBehaviour 
{
    public static HUDController instance;
    public GameObject HandIcon;
    [Range(1, 5)]
    public float HandIconDistance = 3.0f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        if (instance != this)
            DestroyImmediate(this.gameObject);            
    }

	void Start () 
    {
        FadeCamera.FadeColor(Color.black, Color.clear, 10);
    }

	void Update () 
    {
        HandleHandIcon();

        if (Input.GetKeyDown(KeyCode.F))
        {
            FadeCamera.FadeColor(Color.black, Color.clear, 10);
        }
	}

    private void HandleHandIcon()
    {
        Debug.DrawRay(Globals.MainCamera.position, Globals.MainCamera.forward * Globals.Player.Reach, Color.green);

        RaycastHit Hit;
        if (Physics.Raycast(Globals.MainCamera.position, Globals.MainCamera.forward, out Hit, Globals.Player.Reach))
        {
            if (Hit.collider.tag == "Item")
            {
                HandIcon.transform.localPosition = new Vector3(0, 0, Hit.distance * HandIconDistance);
                HandIcon.renderer.enabled = true;
            }
            else
                HandIcon.renderer.enabled = false;
        }
        else
            HandIcon.renderer.enabled = false;

    }
}
