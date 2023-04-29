using UnityEngine;

namespace stoogebag.Common
{
	public class Spinner : MonoBehaviour {

		public float SpinSpeedX = 0; //degrees per second
		public float SpinSpeedY = 50; //degrees per second
		public float SpinSpeedZ = 0; //degrees per second


		// Use this for initialization
		void Start () {
		
		}
	
		// Update is called once per frame
		void Update () {
			transform.Rotate (new Vector3 (Time.deltaTime * SpinSpeedX,Time.deltaTime * SpinSpeedY,Time.deltaTime * SpinSpeedZ));
		}
	}
}