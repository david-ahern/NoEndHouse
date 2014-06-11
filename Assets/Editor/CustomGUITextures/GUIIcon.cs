using UnityEngine;
using System.Collections;

[System.Serializable]
public class GUIIcon : ScriptableObject
{
    [SerializeField]
    public string Key;
    [SerializeField]
    public Texture Tex;

    public GUIIcon(string newKey, Texture newTex)
    {
        Key = newKey;
        Tex = newTex;
    }
}
