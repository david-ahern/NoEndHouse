using UnityEngine;
using System.Collections;

public class FirstPersonController : MonoBehaviour 
{
	public float ForwardMovementSpeed;
	public float SideMovementSpeed;
	public float RunSpeedMultiplyer;
	public float JumpForce;

	public bool Jumped;
	public bool Landed;
	public bool isGrounded;
	public bool wasGrounded;

	private Vector3 Velocity;

    public AreaController CurrentArea;

	// Use this for initialization 
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
		CheckGrounded();
		HandleMovement();
		HandleJump();

		gameObject.rigidbody.velocity += Velocity;
		Velocity = Vector3.zero;
	}

	private void HandleMovement()
	{
		float yrotrad = gameObject.transform.rotation.eulerAngles.y * Mathf.Deg2Rad;

		yrotrad = -yrotrad;

		float VertAxis = Input.GetAxis("Vertical");
		float HorAxis = Input.GetAxis("Horizontal");

		float Running = 1.0f;

		if (Input.GetKey(KeyCode.LeftShift))
			Running = RunSpeedMultiplyer;

		if (VertAxis != 0 && !Jumped)
			Velocity += new Vector3(-Mathf.Sin(yrotrad) * VertAxis * ForwardMovementSpeed, 0, Mathf.Cos(yrotrad) * VertAxis * ForwardMovementSpeed) * Running * Time.deltaTime;
		if (HorAxis != 0 && !Jumped)
			Velocity += new Vector3(Mathf.Cos(yrotrad) * HorAxis * SideMovementSpeed, 0, Mathf.Sin(yrotrad) * HorAxis * SideMovementSpeed) * Running * Time.deltaTime;
	}

	private void HandleJump()
	{
		if (Input.GetKey(KeyCode.Space) && !Jumped)
		{
			Velocity += new Vector3(0, JumpForce, 0);
			Jumped = true;
			isGrounded = false;
		}
	}

	private void CheckGrounded()
	{
		RaycastHit Hit;
		float Distance = 0.5f;
		Vector3 Direction = -Vector3.up;

		Debug.DrawRay(gameObject.transform.position, Direction * Distance, Color.green);

		if (Physics.Raycast(gameObject.transform.position, Direction, out Hit, Distance))
		{
			if (Hit.collider.tag == "Ground")
				isGrounded = true;
			else
				isGrounded = false;
		}
		else
			isGrounded = false;

		if (!wasGrounded && isGrounded)
		{
			Landed = true;
		}
		else if (Landed)
		{
			Landed = false;
			Jumped = false;
		}

		wasGrounded = isGrounded;
	}
	
}
