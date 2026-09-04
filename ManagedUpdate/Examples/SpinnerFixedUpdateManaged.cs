using UnityEngine;

namespace Stoogebag.ManagedUpdate.Examples
{
    [RequireComponent(typeof(ManagedUpdateLifecycle))]
    public class SpinnerFixedUpdateManaged : MonoBehaviour, IFixedUpdateManaged
    {
        public float SpinSpeedX = 0; //degrees per second
        public float SpinSpeedY = 50; //degrees per second
        public float SpinSpeedZ = 0; //degrees per second

        public bool UseAxis = false;

        public Transform axisTransform;
        public float SpinSpeed = 50; //degrees per second

        public ManagerBase CreateManager() => new ManagedUpdateManager<SpinnerFixedUpdateManaged>();

        // Update is called once per frame
        public void ManagedFixedUpdate()
        {
            if (UseAxis)
            {
                transform.RotateAround(axisTransform.position, axisTransform.forward, Time.deltaTime * SpinSpeed);
            }
            else
            {
                transform.Rotate(new Vector3(Time.deltaTime * SpinSpeedX, Time.deltaTime * SpinSpeedY,
                    Time.deltaTime * SpinSpeedZ));
            }
        }
    }
}