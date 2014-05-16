using UnityEngine;
using System.Collections;

public class HeadLook : MonoBehaviour {

	void Update () 
    {
        gameObject.transform.forward = Globals.Player.transform.position - gameObject.transform.position;
	}
}
