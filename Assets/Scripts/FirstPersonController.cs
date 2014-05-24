using UnityEngine;
using System.Collections;

public class FirstPersonController : MonoBehaviour 
{
    private Rigidbody RigidBody;
    private Animator anim;

	public float ForwardMovementSpeed;
	public float SideMovementSpeed;
	public float RunSpeedMultiplyer;
	public float JumpForce;

	public bool Jumped;
	public bool Landed;
	public bool isGrounded;
	public bool wasGrounded;

    public Hand LeftHand;
    public Hand RightHand;
    [Range(0, 5)]
    public float Reach = 2.0f;
	private Vector3 Velocity;

    public bool DisableMovement;

    public AreaController CurrentArea;

    void Awake()
    {
        RigidBody = gameObject.GetComponentInChildren<Rigidbody>();
        anim = gameObject.GetComponentInChildren<Animator>();
    }

	void Start () 
	{
	}

	void Update () 
	{
		CheckGrounded();
		HandleMovement();
		HandleJump();

        HandleInteraction();

        //gameObject.transform.position -= Velocity;
		Velocity = Vector3.zero;
	}

	private void HandleMovement()
	{
        if (!DisableMovement)
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

            anim.SetFloat("Speed", VertAxis);
        }
	}

	private void HandleJump()
	{
		if (Input.GetAxis("Jump") > 0.5f && !Jumped && !DisableMovement)
		{
            Debug.Log("Jump");
            anim.SetFloat("Speed", 1);
            anim.SetBool("Jump", true);
			Velocity += new Vector3(0, JumpForce * 1000, 0);
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

        if (!isGrounded)
        {
            anim.SetBool("Jump", false);
            anim.SetBool("Jumped", true);
        }

		if (!wasGrounded && isGrounded)
		{
			Landed = true;
            anim.SetBool("Jump", false);
            anim.SetBool("Land", true);
		}
		else if (Landed)
		{
            anim.SetBool("Land", false);
			Landed = false;
			Jumped = false;
		}

		wasGrounded = isGrounded;
	}
	
    private void HandleInteraction()
    {
        if (!DisableMovement)
        {
            if (Input.GetMouseButtonUp(0))
            {
                RaycastHit Hit;
                if (Physics.Raycast(Globals.MainCamera.position, Globals.MainCamera.forward, out Hit, Reach))
                {
                    if (!LeftHand.IsEquipped && Hit.collider.tag == "Item")
                    {
                        LeftHand.EquipItem(Hit.collider.gameObject);
                    }
                    else if (LeftHand.IsEquipped && Hit.collider.tag == "ItemHolder")
                    {
                        Hit.collider.gameObject.GetComponent<ItemHolder>().PlaceItem(LeftHand.GiveItem());
                    }
                    else if (LeftHand.IsEquipped)
                        LeftHand.DropItem();
                }
                else if (LeftHand.IsEquipped)
                    LeftHand.DropItem();
            }
            else if (Input.GetMouseButtonUp(1))
            {
                RaycastHit Hit;
                if (Physics.Raycast(Globals.MainCamera.position, Globals.MainCamera.forward, out Hit, Reach))
                {
                    if (!RightHand.IsEquipped && Hit.collider.tag == "Item")
                    {
                        RightHand.EquipItem(Hit.collider.gameObject);
                    }
                    else if (RightHand.IsEquipped && Hit.collider.tag == "ItemHolder")
                    {
                        Hit.collider.gameObject.GetComponent<ItemHolder>().PlaceItem(RightHand.GiveItem());
                    }
                    else if (RightHand.IsEquipped)
                        RightHand.DropItem();
                }
                else if (RightHand.IsEquipped)
                    RightHand.DropItem();
            }
            else if (Input.GetMouseButtonUp(2))
            {
                if (RightHand.IsEquipped)
                    RightHand.DropItem();
                if (LeftHand.IsEquipped)
                    LeftHand.DropItem();
            }
            else if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                RaycastHit Hit;
                if (Physics.Raycast(Globals.MainCamera.position, Globals.MainCamera.forward, out Hit, Reach))
                {
                    if (Hit.collider.tag == "DoorHandle")
                    {
                        Hit.collider.gameObject.GetComponent<DoorHandle>().IsTurned = true;
                    }
                }
            }
        }
    }
}
