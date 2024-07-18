using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Velocity : MonoBehaviour
{
    private Vector3 _oldPos;
    public Vector3 V { get; private set; }
    
    private void Awake()
    {
        _oldPos = transform.position;
    }

    private void FixedUpdate()
    {

        var pos = transform.position; 
        V = (pos - _oldPos) / Time.fixedDeltaTime; 
        _oldPos = pos;


    }
}
