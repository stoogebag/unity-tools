using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class StartVelocity : MonoBehaviour
{
	private Rigidbody rb;
	// Use this for initialization
	Vector3 startDirection = Vector3.zero;
	float startSpeed = 1;
	
	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
	}

	void Start ()
	{
		var dir = startDirection;
		if (dir == Vector3.zero) dir = transform.forward;
		rb.velocity = dir * startSpeed;
	}
	
	// Update is called once per frame
}
