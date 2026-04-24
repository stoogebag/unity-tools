using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using stoogebag.Extensions;
using UnityEngine;

public class CameraFitToBounds : MonoBehaviour
{
    public Renderer target;
    //public float padding = 1.1f;
    //public float smoothing = 5f;
    private Camera _cam;
    [SerializeField] private bool FitOnAwake = true;

    private void Awake()
    {
        if(FitOnAwake) Fit();
    }

    [Button]
    public void Fit(Renderer fitTarget = null)
    {
        _cam = gameObject.FirstOrDefault<Camera>();
        
        var theTarget = fitTarget ?? target;
        if (theTarget == null|| _cam == null) return;

        var bounds = theTarget.bounds;
        float frustumHeight = Mathf.Tan(_cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        
        // Calculate max distance required by either height or width
        float distance = Mathf.Max(bounds.extents.y, bounds.extents.x / _cam.aspect) / frustumHeight;
        
        Vector3 targetPos = bounds.center - (transform.forward * distance);
        
        //todo: maybe tween it
        if(!Application.isPlaying)
            transform.position = targetPos;
        else
        {
            transform.DOMove(targetPos, 0.5f);
        }
    }
}