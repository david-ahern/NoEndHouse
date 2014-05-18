using UnityEngine;
using System.Collections;

public class FadeCamera : MonoBehaviour 
{
    static public FadeCamera instance;

    public Renderer Layer;

    private Material FadeMaterial;
    private Texture FadeTexture;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;

            FadeTexture = new Texture();

            FadeMaterial = new Material(Shader.Find("Transparent/Diffuse"));
            FadeMaterial.SetTexture(0, FadeTexture);

            Layer.material = FadeMaterial;

            Layer.enabled = true;
            FadeMaterial.color = Color.clear;
        }

        if (instance == this)
            DontDestroyOnLoad(this.gameObject);
        else
            DestroyImmediate(this.gameObject);
    }

    static public void FadeColor(Color start, Color end, float time)
    {
        instance.StartCoroutine(instance.coFadeColor(start, end, time));
    }

    public IEnumerator coFadeColor(Color start, Color end, float time)
    {
        float EndTime = Time.time + time;

        while (Time.time < EndTime)
        {
            float blendV = 1 - ((EndTime - Time.time) / time);
            Debug.Log(blendV);
            FadeMaterial.color = new Color(Mathf.Lerp(start.r, end.r, blendV), Mathf.Lerp(start.g, end.g, blendV), Mathf.Lerp(start.b, end.b, blendV), Mathf.Lerp(start.a, end.a, blendV));
            yield return new WaitForEndOfFrame();
        }
    }
}
