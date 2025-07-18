#if CINEMACHINE

using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks.Triggers;
using Unity.Cinemachine;
using UnityEngine;

public class UIFollowParent : MonoBehaviour
{
    [SerializeField]
    private Transform toFollow;

    private CanvasGroup _canvasGroup;
    
    [SerializeField]
    private Camera camera1;
    [SerializeField]
    private Camera mainCam;

    [SerializeField] private bool cinemachine;

    // Update is called once per frame
    private void Awake()
    {
        if(camera1 == null) camera1 = Camera.main;
        mainCam = Camera.main;
        _canvasGroup = GetComponent<CanvasGroup>();
        
        #if CINEMACHINE
        CinemachineCore.CameraUpdatedEvent.AddListener(OnCameraUpdated);
        
        #endif

    }

    private void OnEnable()
    {
        
        SetPosition(mainCam);
    }

    private void SetPosition(Camera cam)
    {
        var point = cam.WorldToScreenPoint(toFollow.position);

        if (_canvasGroup != null)
        {
            if (point.z < 0) _canvasGroup.alpha = 0;
            else _canvasGroup.alpha = 1;
        }
        
        transform.position = point;

    }


#if CINEMACHINE
    void OnCameraUpdated(CinemachineBrain brain)
    {
        var cam = brain.GetComponent<Camera>();
        SetPosition(cam);   
    }
#endif

    void LateUpdate()
    {
        if (cinemachine) return;
        if (camera1 == null) return;
        var point = camera1.WorldToScreenPoint(toFollow.position);

        if (_canvasGroup != null)
        {
            if (point.z < 0) _canvasGroup.alpha = 0;
            else _canvasGroup.alpha = 1;
        }
        
        transform.position = point;
    }

    #if CINEMACHINE
    private void OnDestroy()
    {
        CinemachineCore.CameraUpdatedEvent.RemoveListener(OnCameraUpdated);
    }
    #endif
    
}
#endif