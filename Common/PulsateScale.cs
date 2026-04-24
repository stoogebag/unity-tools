
using System;
using stoogebag.Common;
using UnityEngine;

public class PulsateScale : MonoBehaviour, ISpeedProvider
{

    public float proportion = 1.05f;
    public float period;

    private float startTime = Single.MinValue;
    
    [SerializeField] //curve
    private AnimationCurve curve;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Begin();
    }

    private void Begin()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (startTime == Single.MinValue) return;

        var timeSinceStart = Time.time - startTime;
        var t = Mathf.Repeat(timeSinceStart, period)/period;

        var value = curve.Evaluate(t);
        transform.localScale = new Vector3(value, value, value) * proportion;
        
    }

    public void SetSpeed(Vector3 newSpeed)
    {
        period = 100f/newSpeed.magnitude;
        GetComponent<DestroyAfterSeconds>().Time = period;
    }
}
