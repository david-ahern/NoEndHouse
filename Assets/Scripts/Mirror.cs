using UnityEngine;
using System.Collections;
using UnityEditor;

public class Mirror : MonoBehaviour 
{
    public Camera Cam;

    private Material MirrorMaterial;
    private RenderTexture MirrorTexture;

    void Awake()
    {
        Cam = gameObject.GetComponentInChildren<Camera>();

        MirrorTexture = new RenderTexture(1024, 1024, 24);
        MirrorTexture.antiAliasing = 2;
        MirrorTexture.Create();

        MirrorMaterial = new Material(Shader.Find("Diffuse"));

        Cam.targetTexture = MirrorTexture;
    }

    void Start()
    {
        MirrorMaterial.SetTexture(0, MirrorTexture);
        gameObject.renderer.material = MirrorMaterial;
    }
	
	void Update () 
    {
        Vector3 temp = gameObject.transform.position - Globals.MainCamera.position;
        temp.x *= -1;

        Cam.gameObject.transform.localPosition = temp;
        Cam.fieldOfView = (1 / Cam.gameObject.transform.localPosition.magnitude) * 56;
        Cam.nearClipPlane = Cam.gameObject.transform.localPosition.magnitude;
        Cam.gameObject.transform.forward = -Cam.gameObject.transform.localPosition;
	}

    void EditorUpdate()
    {
        if (Cam != null)
        {
            
        }
    }
}
