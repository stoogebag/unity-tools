using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUID : MonoBehaviour
{
    public Guid Guid;

    public bool RefreshOnStart;

    public string GuidString;

    private void Start()
    {
        if(RefreshOnStart)
            RefreshGUID();
    }

    public void RefreshGUID()
    {
        Guid = Guid.NewGuid();
        GuidString = Guid.ToString();
        
    }

    private void Reset()
    {
        RefreshGUID();
    }
}
