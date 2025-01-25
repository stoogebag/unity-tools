using System.Collections;
using System.Collections.Generic;
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

    

    // Update is called once per frame
    private void Awake()
    {
        if(camera1 == null) camera1 = Camera.main;
        mainCam = Camera.main;
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    void LateUpdate()
    {
        if (camera1 == null) return;
        var point = camera1.WorldToScreenPoint(toFollow.position);

        if (_canvasGroup != null)
        {
            if (point.z < 0) _canvasGroup.alpha = 0;
            else _canvasGroup.alpha = 1;
        }

        transform.position = point;
    }
}
