using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SphereCollider))]
public class Item : MonoBehaviour 
{
    public MiscAudioClip Clip;

    public enum ItemType { Box, Ball };

    public bool InHolder = false;
}
