#if CINEMACHINE


#if NEW_CINEMACHINE
using Unity.Cinemachine;
#else 
using Cinemachine;
#endif
using UnityEngine;

public class UIFollow : MonoBehaviour
{
    public enum Mode
    {
        Transform,
        Point
    }

    [SerializeField]
    private Mode mode = Mode.Transform;

    [SerializeField]
    private Transform toFollow;

    [SerializeField]
    private Vector3 worldOffset;

    [SerializeField]
    private Camera camera1;

    [SerializeField] private bool cinemachine;

    private static readonly Vector3 OffScreen = new Vector3(-10000f, 0f, 0f);
    private Vector3 _point;

    public void SetPoint(Vector3 worldPoint)
    {
        mode = Mode.Point;
        _point = worldPoint;
    }

    public void SetTarget(Transform target)
    {
        mode = Mode.Transform;
        toFollow = target;
    }

    public void SetWorldOffset(Vector3 offset)
    {
        worldOffset = offset;
    }

    private Vector3 WorldPosition
    {
        get
        {
            var basePosition = mode == Mode.Transform && toFollow != null ? toFollow.position : _point;
            return basePosition + worldOffset;
        }
    }

    private void Awake()
    {
        if (camera1 == null) camera1 = Camera.main;

#if CINEMACHINE
        CinemachineCore.CameraUpdatedEvent.AddListener(OnCameraUpdated);
#endif
    }

    private void OnEnable()
    {
        SetPosition(camera1);
    }

    private void SetPosition(Camera cam)
    {
        if (cam == null)
        {
            return;
        }

        var point = cam.WorldToScreenPoint(WorldPosition);
        transform.position = point.z < 0f ? OffScreen : point;
    }

#if CINEMACHINE
    private void OnCameraUpdated(CinemachineBrain brain)
    {
        SetPosition(brain.GetComponent<Camera>());
    }
#endif

    private void LateUpdate()
    {
        if (cinemachine)
        {
            return;
        }

        SetPosition(camera1);
    }

#if CINEMACHINE
    private void OnDestroy()
    {
        CinemachineCore.CameraUpdatedEvent.RemoveListener(OnCameraUpdated);
    }
#endif
}
#endif
