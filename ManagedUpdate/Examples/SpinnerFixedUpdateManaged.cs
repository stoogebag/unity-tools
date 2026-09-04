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

        private Transform _tf;
        private Transform _axis;
        private Quaternion _rot;
        private Vector3 _step;
        private float _angleStep;

        private void Awake()
        {
            _tf = transform;
            _axis = axisTransform;
            _rot = _tf.localRotation;
            RefreshStep();
        }

        private void OnEnable()
        {
            if (_tf == null) return;
            _rot = _tf.localRotation;
            _axis = axisTransform;
            RefreshStep();
        }

        public void RefreshStep()
        {
            _step = new Vector3(SpinSpeedX, SpinSpeedY, SpinSpeedZ) * Time.fixedDeltaTime;
            _angleStep = Time.fixedDeltaTime * SpinSpeed;
        }

        public void ManagedFixedUpdate()
        {
            if (UseAxis && _axis != null)
            {
                _tf.RotateAround(_axis.position, _axis.forward, _angleStep);
            }
            else
            {
                _rot *= Quaternion.Euler(_step);
                _tf.localRotation = _rot;
            }
        }
    }
}
