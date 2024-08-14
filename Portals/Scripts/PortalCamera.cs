using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using RenderPipeline = UnityEngine.Rendering.RenderPipelineManager;

public class PortalCamera : MonoBehaviour
    {
    
        [SerializeField]
        private List<Portal> portals = new List<Portal>();

    
        [SerializeField]
        private Camera portalCamera;

        [SerializeField]
        private int iterations = 7;


        private Camera mainCamera;

        private void Awake()
        {
            mainCamera = GetComponent<Camera>();
            portals = FindObjectsOfType<Portal>().ToList();

        }

        private void Start()
        {
        }

        private void OnEnable()
        {
            RenderPipeline.beginCameraRendering += UpdateCamera;
        }

        private void OnDisable()
        {
            RenderPipeline.beginCameraRendering -= UpdateCamera;
        }

        void UpdateCamera(ScriptableRenderContext SRC, Camera camera)
        {
            // if (!portals[0].IsPlaced || !portals[1].IsPlaced)
            // {
            //     return;
            // }

            foreach (var p in portals)
            {
            
                //for (var j = 0; j < pair.Portals.Length; j++)
                {
                    if (p.Renderer.isVisible)
                    {
                        portalCamera.targetTexture = p.RenderTex;
                        //portalCamera.targetTexture.activeTextureColorSpace;
                        for (int i = iterations - 1; i >= 0; --i)
                        {
                            //print($"{j}, {i}");
                            RenderCamera(p, p.OtherPortal, i, SRC);
                        }
                    }
                }
            }
        

        }

        private void RenderCamera(Portal inPortal, Portal outPortal, int iterationID, ScriptableRenderContext SRC)
        {
            Transform inTransform = inPortal.transform;
            Transform outTransform = outPortal.transform;

            Transform cameraTransform = portalCamera.transform;
            cameraTransform.position = transform.position;
            cameraTransform.rotation = transform.rotation;

            for(int i = 0; i <= iterationID; ++i)
            {
                // Position the camera behind the other portal.
                Vector3 relativePos = inTransform.InverseTransformPoint(cameraTransform.position);
                relativePos = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativePos;
                cameraTransform.position = outTransform.TransformPoint(relativePos);

                // Rotate the camera to look through the other portal.
                Quaternion relativeRot = Quaternion.Inverse(inTransform.rotation) * cameraTransform.rotation;
                relativeRot = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativeRot;
                cameraTransform.rotation = outTransform.rotation * relativeRot;
            
                //scale it?
                Vector3 relativeScale = new Vector3(inTransform.lossyScale.x / outTransform.lossyScale.x,
                    inTransform.lossyScale.y / outTransform.lossyScale.y,
                    inTransform.lossyScale.z / outTransform.lossyScale.z);

                //cameraTransform.localScale = relativeScale; //todo: account for localscale if it matter
                //portalCamera.worldToCameraMatrix.y = 2;
            }

            // Set the camera's oblique view frustum.
            Plane p = new Plane(-outTransform.forward, outTransform.position);
            Vector4 clipPlaneWorldSpace = new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance);
            Vector4 clipPlaneCameraSpace =
                Matrix4x4.Transpose(Matrix4x4.Inverse(portalCamera.worldToCameraMatrix)) * clipPlaneWorldSpace;

            var newMatrix = mainCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);

            // newMatrix[0, 0] = newMatrix[0, 0] * nm00scale;
            // newMatrix[1, 1] = newMatrix[1, 1] * nm11scale;
            // newMatrix[2, 2] = newMatrix[2, 2] * nm22scale;
            portalCamera.projectionMatrix = newMatrix;

            // Render the camera to its render target.
            UniversalRenderPipeline.RenderSingleCamera(SRC, portalCamera);
        }

        // public float nm00scale = 1;
        // public float nm11scale = 1;
        // public float nm22scale = 1;
    }

