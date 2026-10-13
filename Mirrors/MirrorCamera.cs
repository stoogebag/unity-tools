using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace stoogebag.Mirrors
{
    [RequireComponent(typeof(Camera))]
    public class MirrorCamera : MonoBehaviour
    {
        [SerializeField] private Camera mirrorCamera;
        [SerializeField] private LayerMask cullingMask = ~0;

        private Camera _mainCamera;
        private bool _logged;

        private void Awake()
        {
            _mainCamera = GetComponent<Camera>();
            if (mirrorCamera == null)
                mirrorCamera = CreateMirrorCamera();
        }

        private Camera CreateMirrorCamera()
        {
            var go = new GameObject($"{name}_MirrorCamera");
            go.transform.SetParent(transform, false);

            var cam = go.AddComponent<Camera>();
            cam.enabled = false;
            cam.cullingMask = cullingMask;
            cam.clearFlags = CameraClearFlags.SolidColor;

            var data = cam.GetUniversalAdditionalCameraData();
            data.renderType = CameraRenderType.Base;
            data.renderPostProcessing = false;

            return cam;
        }

        private void OnEnable()
        {
            RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
        }

        private void OnDisable()
        {
            RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
        }

        private void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            if (camera != _mainCamera || mirrorCamera == null)
                return;

            mirrorCamera.cullingMask = cullingMask;
            mirrorCamera.clearFlags = _mainCamera.clearFlags;
            mirrorCamera.backgroundColor = _mainCamera.backgroundColor;

            foreach (var mirror in Mirror.Active)
            {
                if (mirror == null || mirror.Renderer == null || !mirror.Renderer.isVisible)
                    continue;

                mirror.EnsureRenderTexture();
                RenderMirror(mirror);

                if (!_logged)
                {
                    Debug.Log($"[Mirror] Rendering '{mirror.name}' at {mirror.transform.position}");
                    _logged = true;
                }
            }
        }

        private void RenderMirror(Mirror mirror)
        {
            var mirrorTransform = mirror.transform;
            var planePoint = mirrorTransform.position;
            var normal = mirrorTransform.up;

            var source = _mainCamera.transform;
            var position = source.position - 2f * Vector3.Dot(source.position - planePoint, normal) * normal;
            var forward = Vector3.Reflect(source.forward, normal);
            var up = Vector3.Reflect(source.up, normal);

            var camTransform = mirrorCamera.transform;
            camTransform.SetPositionAndRotation(position, Quaternion.LookRotation(forward, up));

            var clipPlane = new Plane(normal, planePoint);
            var clipPlaneWorld = new Vector4(clipPlane.normal.x, clipPlane.normal.y, clipPlane.normal.z, clipPlane.distance);
            var clipPlaneCamera = Matrix4x4.Transpose(Matrix4x4.Inverse(mirrorCamera.worldToCameraMatrix)) * clipPlaneWorld;

            mirrorCamera.projectionMatrix = _mainCamera.projectionMatrix;
            mirrorCamera.projectionMatrix = mirrorCamera.CalculateObliqueMatrix(clipPlaneCamera);

            var request = new UniversalRenderPipeline.SingleCameraRequest { destination = mirror.RenderTex };
            if (RenderPipeline.SupportsRenderRequest(mirrorCamera, request))
                RenderPipeline.SubmitRenderRequest(mirrorCamera, request);
        }
    }
}
