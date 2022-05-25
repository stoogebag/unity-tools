using System.Collections;
using System.Collections.Generic;
using stoogebag;
using UnityEngine;

public class FollowMouseLocationInWorld : MonoBehaviour
{
    private Camera _mainCamera;
    private float distanceToCam = 10f;

    [SerializeField]
    private float y = 0f;

    // Start is called before the first frame update
    void Start()
    {
        _mainCamera = Camera.main;
        distanceToCam = _mainCamera.transform.position.y - y;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var loc = _mainCamera.ScreenToWorldPoint(Input.mousePosition.WithZ(distanceToCam));
        transform.position = loc;
    }
}
