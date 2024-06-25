using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFollowParent : MonoBehaviour
{
    [SerializeField]
    private Transform toFollow;

    private CanvasGroup _canvasGroup;
    private Camera camera1;

    // Update is called once per frame
    private void Awake()
    {
        camera1 = Camera.main;
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    void Update()
    {
        if (camera1 == null) return;
        var point = camera1.WorldToScreenPoint(toFollow.position);
        
        if (point.z < 0) _canvasGroup.alpha = 0;
        else _canvasGroup.alpha = 1;
        
        transform.position = point;
    }
}
