using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFollowParent : MonoBehaviour
{
    [SerializeField]
    private Transform toFollow;

    private CanvasGroup _canvasGroup;
    
    // Update is called once per frame
    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    void Update()
    {
        var point = Camera.main.WorldToScreenPoint(toFollow.position);
        
        if (point.z < 0) _canvasGroup.alpha = 0;
        else _canvasGroup.alpha = 1;
        
        transform.position = point;
    }
}
