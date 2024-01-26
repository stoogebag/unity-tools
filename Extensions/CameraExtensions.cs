using UnityEngine;

namespace stoogebag.Extensions
{
    public static class CameraExtensions
    {

        /// <summary>
        /// https://forum.unity.com/threads/fit-object-exactly-into-perspective-cameras-field-of-view-focus-the-object.496472/#post-8181533
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="bounds"></param>
        public static void FitToBounds(this Camera cam, Bounds bounds)
        {
            float virtualsphereRadius = Vector3.Magnitude(bounds.max-bounds.center);
            float minD = (virtualsphereRadius )/ Mathf.Sin(Mathf.Deg2Rad*cam.fieldOfView/2);
            Vector3 normVectorBoundsCenter2CurrentCamPos= (cam.transform.position - bounds.center) / Vector3.Magnitude(cam.transform.position -  bounds.center);
            cam.transform.position =  minD*normVectorBoundsCenter2CurrentCamPos;
            cam.transform.LookAt(bounds.center);
            cam.nearClipPlane = minD- virtualsphereRadius;
        }

        public static Vector3 GetWorldMousePosition(this Camera cam, float distance)
        {
            return cam.ScreenToWorldPoint(UnityEngine.Input.mousePosition.WithZ(distance));
        }

        public static RaycastHit Raycast(this Camera cam, Vector3 screenPos, LayerMask layers, float distance = 100)
        {
            var ray = cam.ScreenPointToRay(screenPos);
        
            if (Physics.Raycast(ray, out var hit, distance, layers))
            {
                return hit;
            }
            else return default;
        }
        
        public static bool Raycast(this Camera cam, Vector3 screenPos, LayerMask layers, float distance,  out Vector3 worldPos)
        {
            var ray = cam.ScreenPointToRay(screenPos);
        
            if (Physics.Raycast(ray, out var hit, distance, layers))
            {
                worldPos = hit.point;
                return true;
            }
            else
            {
                worldPos = default;
                return false;
            }
        }
    
    
    }
}
