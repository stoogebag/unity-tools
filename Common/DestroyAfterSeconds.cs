using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using stoogebag;
using System;

public class DestroyAfterSeconds : MonoBehaviour
{
    public float Time = 30;
    private DateTime started;

    void Start()
    {
        //started = DateTime.UtcNow;
        Destroy(gameObject, Time);
    }

    void Update()
    {
    }
}
