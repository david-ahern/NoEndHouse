using UnityEngine;
using System.Collections;

public class HUDController : MonoBehaviour 
{
    public static HUDController instance;
    public Renderer InteractIcon;

    [Range(1, 10)]
    public float InteractIconDistance = 5.0f;
    [Range(0, 1)]
    public float IconBlendEasingSpeed = 0.5f;

    public enum IconStates { Default = 1, Pickup = 0, Drop = 2 };
    public IconStates CurrentIconState = IconStates.Pickup;

    public Texture DefaultIcon;
    public Texture PickupIcon;
    public Texture DropIcon;
    private Material InteractionIconMaterial;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Debug.Log("Attempting to create shader");
            InteractionIconMaterial = new Material(Shader.Find("HUD/InteractIcon"));
            InteractionIconMaterial.SetTexture("_MainTex", DefaultIcon);
            InteractionIconMaterial.SetTexture("_PickupTex", PickupIcon);
            InteractionIconMaterial.SetTexture("_DropTex", DropIcon);

            InteractIcon.material = InteractionIconMaterial;
        }

        if (instance != this)
            DestroyImmediate(this.gameObject);            
    }

	void Start () 
    {
        FadeCamera.FadeColor(Color.black, Color.clear, 10);
        SwitchIcon(IconStates.Default);
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

        InteractIcon.transform.localPosition = new Vector3(0, 0, Globals.Player.Reach * InteractIconDistance);

        RaycastHit Hit;
        if (Physics.Raycast(Globals.MainCamera.position, Globals.MainCamera.forward, out Hit, Globals.Player.Reach))
        {        
            if (Hit.collider.tag == "Item")
                if ((!Globals.Player.LeftHand.IsEquipped || !Globals.Player.RightHand.IsEquipped))
                {
                    SwitchIcon(IconStates.Pickup);
                    InteractIcon.transform.localPosition = new Vector3(0, 0, Hit.distance * InteractIconDistance);
                }
                else
                    SwitchIcon(IconStates.Default);
            else if (Hit.collider.tag == "ItemHolder")
                if ((Globals.Player.LeftHand.IsEquipped || Globals.Player.RightHand.IsEquipped) && !Hit.collider.gameObject.GetComponent<ItemHolder>().HasItem)
                {
                    SwitchIcon(IconStates.Drop);
                    InteractIcon.transform.localPosition = new Vector3(0, 0, Hit.distance * InteractIconDistance);
                }
                else
                    SwitchIcon(IconStates.Default);
            else
                SwitchIcon(IconStates.Default);
        }
        else
        {
            InteractIcon.transform.localPosition = new Vector3(0, 0, Globals.Player.Reach * InteractIconDistance);
            SwitchIcon(IconStates.Default);
        }
    }

    private void SwitchIcon(IconStates state)
    {
        if (state != CurrentIconState)
        {
            CurrentIconState = state;
            StopCoroutine("coSwitchIcon");
            StartCoroutine("coSwitchIcon", state);
        }
    }

    private IEnumerator coSwitchIcon(IconStates state)
    {
        float target = (float)state / 2;
        float currentBlend = InteractIcon.material.GetFloat("_Blend");
        while (currentBlend > target + 0.01f || currentBlend < target - 0.01f)
        {
            currentBlend += (target - currentBlend - 0.001f) * IconBlendEasingSpeed;
            InteractIcon.material.SetFloat("_Blend", currentBlend);
            yield return new WaitForEndOfFrame();
        }

        InteractIcon.material.SetFloat("_Blend", target);
    }
}
