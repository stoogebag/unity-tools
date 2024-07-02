using UnityEngine;

namespace stoogebag.Common
{
	public class Spinner : MonoBehaviour {

		public float SpinSpeedX = 0; //degrees per second
		public float SpinSpeedY = 50; //degrees per second
		public float SpinSpeedZ = 0; //degrees per second

		public bool UseAxis = false;

		public Transform axisTransform;
		public float SpinSpeed = 50; //degrees per second
		
		// Use this for initialization
		void Start () {
		
		}
	
		// Update is called once per frame
		void Update () {
			if (UseAxis)
			{
				transform.RotateAround(axisTransform.position,axisTransform.forward,Time.deltaTime * SpinSpeed);
			}
			else
			{
				transform.Rotate(new Vector3(Time.deltaTime * SpinSpeedX, Time.deltaTime * SpinSpeedY,
					Time.deltaTime * SpinSpeedZ));
			}
		}
	}
}