using System.Collections;
using System.Collections.Generic;
using stoogebag.Extensions;
using UnityEngine;

public class SaveTransformInfo : MonoBehaviour
{
    public Dictionary<string, TransformInfo> Saved = new Dictionary<string, TransformInfo>();
    
    
    // Start is called before the first frame update
    void Awake()
    {
        if (Time == InitialTransformTime.Awake) SaveTransform("initial");
    }
    void Start()
    {
        if (Time == InitialTransformTime.Start) SaveTransform("initial");
    }

    public InitialTransformTime Time;


    public void SaveTransform(string s)
    {
        Saved[s] = new TransformInfo(transform);
    }

    public TransformInfo LoadTransform(string s)
    {
        return Saved[s];
    }

    public enum InitialTransformTime
    {
        Awake,
        Start,
        Manual,
    }
}

public struct TransformInfo
{
    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 Scale;
    
    public TransformInfo(Transform t)
    {
        Position = t.position;
        Rotation = t.rotation;
        Scale = t.localScale;
    }
    
    public void ApplyTo(Transform t)
    {
        t.position = Position;
        t.rotation = Rotation;
        t.localScale = Scale;
    }

    public bool EqualsTransform(Transform originalTransform)
    {
        if (!Position.EqualsApprox(originalTransform.position)) return false;
        if (!Rotation.Equals(originalTransform.rotation)) return false;
        if (!Scale.EqualsApprox(originalTransform.localScale)) return false;
        return true;
    }
}