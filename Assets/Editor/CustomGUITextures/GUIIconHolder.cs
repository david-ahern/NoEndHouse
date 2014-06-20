using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIIconHolder: ScriptableObject
{
    public List<string> Keys;
    public List<Texture> Textures;

    public Texture Selected;

    public Texture GetTex(string Key)
    {
        foreach (string key in Keys)
            if (Key == key)
                return Textures[Keys.IndexOf(key)];
        return null;
    }

    public string GetKey(Texture Tex)
    {
        foreach (Texture tex in Textures)
            if (Tex == tex)
                return Keys[Textures.IndexOf(tex)];
        return "";
    }

    public Texture Select(string Key)
    {
        return Selected = GetTex(Key);
    }

    public void Remove(string Key)
    {
       foreach (string key in Keys)
           if (key == Key)
           {
               if (Selected == GetTex(key))
                   Selected = null;

               Textures.RemoveAt(Keys.IndexOf(key));
               Keys.Remove(key);
               break;
           }
    }

    public void ReplaceSelected(Texture Tex)
    {
        foreach (Texture tex in Textures)
            if (tex == Selected)
                Textures[Textures.IndexOf(tex)] = Tex;
    }
}

