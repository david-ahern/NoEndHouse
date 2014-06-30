using UnityEngine;
using System.Collections;

using System.Collections.Generic;

public class Thing : MonoBehaviour 
{
    public List<Transform> NavPoints;

    public List<Transform> EscapePoints;

    private int NavPointIndex = 0;

    public float WalkSpeed = 0;
    public float RunSpeed = 0;

    private bool Running;

    public Animator anim;
    void Start()
    {
        anim.SetFloat("Speed", WalkSpeed);
        anim.SetTrigger("Move");
    }

    void Update()
    {
        if (!Running)
        {
            anim.SetFloat("Speed", WalkSpeed);

            gameObject.transform.forward = (NavPoints[NavPointIndex].position - gameObject.transform.position).normalized;
            gameObject.transform.position += gameObject.transform.forward * WalkSpeed * Time.deltaTime;

            if ((gameObject.transform.position - NavPoints[NavPointIndex].position).magnitude < 1)
            {
                NavPointIndex++;
                if (NavPointIndex >= NavPoints.Count)
                    NavPointIndex = 0;
            }
        }
        else
        {
            anim.SetFloat("Speed", RunSpeed);

            gameObject.transform.forward = (EscapePoints[NavPointIndex].position - gameObject.transform.position).normalized;
            gameObject.transform.position += gameObject.transform.forward * RunSpeed * Time.deltaTime;

            if ((gameObject.transform.position - EscapePoints[NavPointIndex].position).magnitude < 1)
            {
                NavPointIndex++;
                if (NavPointIndex == EscapePoints.Count)
                    GameObject.Destroy(gameObject.transform.parent.gameObject);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Running = true;
            NavPointIndex = 0;
        }
    }

    void DoSomething(float i)
    {
        Debug.Log("Doing something " + i);
    }
}
