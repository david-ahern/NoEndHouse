using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour 
{
    private HingeJoint Hinge;
    private JointLimits Limits;
    private JointSpring Spring;

    private DoorHandle Handle;


    public float DefaultAngle;
    public float HalfWidth;

    public float MaxAngle;
    public float MinAngle;

    public bool _locked;

    private bool IsOpen;

    public bool IsLocked
    {
        get { return _locked; }
        set { _locked = value; }
    }

    void Awake()
    {
        Hinge = gameObject.GetComponent<HingeJoint>();

        if (!Hinge)
            Hinge = gameObject.GetComponentInChildren<HingeJoint>();

        if (!Hinge)
            Hinge = gameObject.AddComponent<HingeJoint>();

        Hinge.axis = new Vector3(0, 1, 0);
        Hinge.anchor = new Vector3(HalfWidth, 0, 0);

        Limits.max = DefaultAngle;
        Limits.min = DefaultAngle;
        Hinge.limits = Limits;

        Spring.targetPosition = DefaultAngle;
        Spring.spring = 100;
        Spring.damper = 10;
        Hinge.spring = Spring;
        Hinge.useSpring = true;

        Handle = gameObject.GetComponentInChildren<DoorHandle>();

        if (!Handle)
            Debug.Log("No handle found, you probably wont be able to open the door, you fool");
    }

	void Update () 
    {
        if (Handle.IsTurned && !IsLocked && !IsOpen)
            StartCoroutine(OpenDoor());
	}

    private IEnumerator OpenDoor()
    {
        IsOpen = true;

        Limits.max = MaxAngle;
        Limits.min = MinAngle;
        Hinge.limits = Limits;

        while (Handle.IsTurned)
            yield return new WaitForEndOfFrame();

        Hinge.useSpring = true;
        Hinge.spring = Spring;

        while (Hinge.angle < DefaultAngle - 1 || Hinge.angle > DefaultAngle + 1)
            yield return new WaitForEndOfFrame();

        Limits.max = DefaultAngle;
        Limits.min = DefaultAngle;
        Hinge.limits = Limits;

        IsOpen = false;
    }
}
