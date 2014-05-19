using UnityEngine;
using System.Collections;

public class Globals : MonoBehaviour 
{
	static public FirstPersonController Player = GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonController>();

	static public Transform MainCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;

    static public HUDController HUD = GameObject.FindGameObjectWithTag("HUD").GetComponent<HUDController>();


    static public float SoundSpeed = 340.29f;
}
