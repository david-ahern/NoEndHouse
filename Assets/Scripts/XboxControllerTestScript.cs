using UnityEngine;
using System.Collections;

public class XboxControllerTestScript : MonoBehaviour 
{
	
	void Update () 
    {
        if (Input.GetButtonDown("Xbox-AButton"))
            Debug.Log("A Pressed - P1");
        if (Input.GetButtonDown("Xbox-AButtonP2"))
            Debug.Log("A Pressed - P2");
        if (Input.GetButtonDown("Xbox-BButton"))
            Debug.Log("B Pressed");
        if (Input.GetButtonDown("Xbox-XButton"))
            Debug.Log("X Pressed");
        if (Input.GetButtonDown("Xbox-YButton"))
            Debug.Log("Y Pressed");
        if (Input.GetButtonDown("Xbox-LBump"))
            Debug.Log("Left Bumper Pressed");
        if (Input.GetButtonDown("Xbox-RBump"))
            Debug.Log("Right Bumper Pressed");
        if (Input.GetButtonDown("Xbox-BackButton"))
            Debug.Log("Back Pressed");
        if (Input.GetButtonDown("Xbox-StartButton"))
            Debug.Log("Start Pressed");
        if (Input.GetButtonDown("Xbox-LStick"))
            Debug.Log("Left Stick Pressed");
        if (Input.GetButtonDown("Xbox-RStick"))
            Debug.Log("Right Stick Pressed");

        if (Input.GetAxis("Xbox-LTrigger") > 0)
            Debug.Log("Left trigger: " + Input.GetAxis("Xbox-LTrigger"));
        if (Input.GetAxis("Xbox-RTrigger") > 0)
            Debug.Log("Right Trigger: " + Input.GetAxis("Xbox-RTrigger"));

        if (Input.GetAxis("Xbox-DPadLeft") > 0)
            Debug.Log("D Pad Left");
        if (Input.GetAxis("Xbox-DPadRight") > 0)
            Debug.Log("D Pad Right");
        if (Input.GetAxis("Xbox-DPadUp") > 0)
            Debug.Log("D Pad Up");
        if (Input.GetAxis("Xbox-DPadDown") > 0)
            Debug.Log("D Pad Down");


        if (Input.GetAxis("Xbox-RStickHorizontal") != 0)
            Debug.Log("Right Stick H: " + Input.GetAxis("Xbox-RStickHorizontal"));
        if (Input.GetAxis("Xbox-RStickVertical") != 0)
            Debug.Log("Right Stick V: " + Input.GetAxis("Xbox-RStickVertical"));

        if (Input.GetAxis("Xbox-LStickHorizontal") != 0)
            Debug.Log("Left Stick H: " + Input.GetAxis("Xbox-LStickHorizontal"));
        if (Input.GetAxis("Xbox-LStickVertical") != 0)
            Debug.Log("Left Stick V: " + Input.GetAxis("Xbox-LStickVertical"));
	}
}
