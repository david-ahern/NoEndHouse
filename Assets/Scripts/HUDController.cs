using UnityEngine;
using System.Collections;

public class HUDController : MonoBehaviour 
{
    public static HUDController instance;
    public Renderer Crosshair;
    public Renderer HandIcon;
    public Renderer HandIconDrop;

    [Range(1, 10)]
    public float HandIconDistance = 5.0f;
    [Range(1, 10)]
    public float HandDropDistance = 5.0f;

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
        HandleHandIcons();

        if (Input.GetKeyDown(KeyCode.F))
        {
            FadeCamera.FadeColor(Color.black, Color.clear, 5);
        }
	}

    private void HandleHandIcons()
    {
        Debug.DrawRay(Globals.MainCamera.position, Globals.MainCamera.forward * Globals.Player.Reach, Color.green);

        RaycastHit Hit;
        if (Physics.Raycast(Globals.MainCamera.position, Globals.MainCamera.forward, out Hit, Globals.Player.Reach))
        {
            if (Hit.collider.tag == "Item")
            {
                if ((!Globals.Player.LeftHand.IsEquipped || !Globals.Player.RightHand.IsEquipped))
                {
                    HandIcon.transform.localPosition = new Vector3(0, 0, Hit.distance * HandIconDistance);
                    HandIcon.enabled = true;
                    HandIconDrop.enabled = false;
                    Crosshair.enabled = false;
                }
                else
                {
                    HandIcon.enabled = false;
                    HandIconDrop.enabled = false;
                    Crosshair.enabled = true;
                }
            }
            else if (Hit.collider.tag == "ItemHolder")
            {
                if ((Globals.Player.LeftHand.IsEquipped || Globals.Player.RightHand.IsEquipped) && !Hit.collider.gameObject.GetComponent<ItemHolder>().HasItem)
                {
                    HandIconDrop.transform.localPosition = new Vector3(0, 0, Hit.distance * HandDropDistance);
                    HandIconDrop.enabled = true;
                    HandIcon.enabled = false;
                    Crosshair.enabled = false;
                }
                else
                {
                    HandIcon.enabled = false;
                    HandIconDrop.enabled = false;
                    Crosshair.enabled = true;
                }
            }
            else
            {
                HandIcon.enabled = false;
                HandIconDrop.enabled = false;
                Crosshair.enabled = true;
            }
        }
        else
        {
            HandIcon.enabled = false;
            HandIconDrop.enabled = false;
            Crosshair.enabled = true;
        }
    }
}
