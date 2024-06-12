using UnityEngine;

namespace stoogebag.Common
{
	[RequireComponent(typeof(Rigidbody))]
	public class StartVelocity : MonoBehaviour
	{
		private Rigidbody rb;
		// Use this for initialization
		public Vector3 startDirection = Vector3.zero;
		public float startSpeed = 1;
	
		private void Awake()
		{
			rb = GetComponent<Rigidbody>();
		}

		void Start ()
		{
			var dir = transform.TransformDirection(startDirection);
			if (dir == Vector3.zero) dir = transform.forward;
			rb.linearVelocity += dir * startSpeed;
		}
	
		// Update is called once per frame
	}
}