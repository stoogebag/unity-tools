using UnityEngine;

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

    
    
}
