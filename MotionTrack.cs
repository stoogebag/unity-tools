using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionTrack : MonoBehaviour
{

    [SerializeField] Transform _startPoint;
    [SerializeField] Transform _endPoint;

    public float Period = 1f;
    
    
    public MotionType type;
    private float _startTime;

    
    private Platform _platform;
    
    private void Start()
    {
     
        _platform = GetComponentInChildren<Platform>(); 

        Begin();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(_startPoint.position, 0.1f);
        Gizmos.DrawSphere(_endPoint.position, 0.1f);
        Gizmos.DrawLine(_startPoint.position, _endPoint.position);
    }

    public void Begin()
    {
        _startTime = Time.time;
    }
    
    
    private void Update()
    {
        var time = Time.time - _startTime;
        if (type == MotionType.ConstantPeriod)
        {
            var t = (time % Period)/Period;
            _platform.transform.position = GetPoint(t);
        }
    }

    public Vector3 GetPoint(float t)
    {
        return _startPoint.position + (t * (_endPoint.position - _startPoint.position));
    }


    public enum MotionType
    {
        ConstantSpeed,
        ConstantPeriod,
    }
}
