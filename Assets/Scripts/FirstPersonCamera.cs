using UnityEngine;
using System.Collections;

public class FirstPersonCamera : MonoBehaviour {

	Vector2 MouseAbsolute;
	Vector2 SmoothMouse;

	public Vector2 ClampInDegrees = new Vector2(360, 180);
	public bool lockCursor;
	public Vector2 Smoothing = new Vector2(2, 2);
	public Vector2 TargetDirection;
	public Vector2 TargetCharacterDirection;

	public GameObject CharacterBody;

	void Start () 
	{
		TargetDirection = gameObject.transform.localRotation.eulerAngles;

		if (CharacterBody) TargetCharacterDirection = CharacterBody.transform.localRotation.eulerAngles;
	}
	
	// Update is called once per frame
	void Update () 
	{
		Screen.lockCursor = lockCursor;

		Quaternion targetOrientation = Quaternion.Euler(TargetDirection);
		Quaternion targetCharacterOrientation = Quaternion.Euler(TargetCharacterDirection);

		Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

		SmoothMouse.x = Mathf.Lerp (SmoothMouse.x, mouseDelta.x, 1f / Smoothing.x);
		SmoothMouse.y = Mathf.Lerp (SmoothMouse.y, mouseDelta.y, 1f / Smoothing.y);

		MouseAbsolute += SmoothMouse;

		if (ClampInDegrees.x < 360)
			MouseAbsolute.x = Mathf.Clamp(MouseAbsolute.x, -ClampInDegrees.x * 0.5f, ClampInDegrees.x * 0.5f);

		Quaternion xRotation = Quaternion.AngleAxis(-MouseAbsolute.y, targetOrientation * Vector3.right);
		transform.localRotation = xRotation;

		if (ClampInDegrees.y < 360)
			MouseAbsolute.y = Mathf.Clamp(MouseAbsolute.y, -ClampInDegrees.y * 0.5f, ClampInDegrees.y * 0.5f);

		transform.localRotation *= targetOrientation;

		if (CharacterBody)
		{
			Quaternion yRotation = Quaternion.AngleAxis(MouseAbsolute.x, CharacterBody.transform.up);
			CharacterBody.transform.localRotation = yRotation;
			CharacterBody.transform.localRotation *= targetCharacterOrientation;
		}
		else
		{
			Quaternion yRotation = Quaternion.AngleAxis (MouseAbsolute.x, transform.InverseTransformDirection(Vector3.up));
			transform.localRotation *= yRotation;
		}

	}


}
