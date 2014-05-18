using UnityEngine;
using System.Collections;

public class HUDController : MonoBehaviour 
{
    public static HUDController instance;
    public GameObject HandIcon;
    public GameObject HandIconDrop;
    [Range(1, 5)]
    public float HandIconDistance = 3.0f;

    public GameObject Char;

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
        HandleChar();
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
                if ((!Globals.Player.LeftHand.IsEquipped || !Globals.Player.RightHand.IsEquipped) && !Hit.collider.gameObject.GetComponent<Item>().InHolder)
                {
                    HandIcon.transform.localPosition = new Vector3(0, 0, Hit.distance * HandIconDistance);
                    HandIcon.renderer.enabled = true;
                }
                else
                    HandIcon.renderer.enabled = false;
            }
            else if (Hit.collider.tag == "ItemHolder")
            {
                if ((Globals.Player.LeftHand.IsEquipped || Globals.Player.RightHand.IsEquipped) && !Hit.collider.gameObject.GetComponent<ItemHolder>().HasItem)
                {
                    HandIconDrop.transform.localPosition = new Vector3(0, 0, Hit.distance * HandIconDistance);
                    HandIconDrop.renderer.enabled = true;
                }
                else
                {
                    HandIconDrop.renderer.enabled = false;
                }
            }
            else
            {
                HandIcon.renderer.enabled = false;
                HandIconDrop.renderer.enabled = false;
            }
        }
        else
        {
            HandIcon.renderer.enabled = false;
            HandIconDrop.renderer.enabled = false;
        }
    }

    private void HandleChar()
    {
        if (SoundController.SoundtrackName == "Hush" && SoundController.SoundtrackPlayPosition > 94.7f && SoundController.SoundtrackPlayPosition < 96.7f)
        {
            Char.gameObject.SetActive(true);
        }
        else
            Char.gameObject.SetActive(false);
    }
}
