using System;
using System.Collections;
using System.Collections.Generic;
using Stoogebag.ManagedUpdate;
using UnityEngine;

[RequireComponent(typeof(ManagedUpdateLifecycle))]
public class ConstantMovement : MonoBehaviour, ISpeedProvider,IFixedUpdateManaged
{
    //must have a parent for this to work!
    [SerializeField] private bool relative = false;

    public Vector3 speed = new Vector3(10, 10, 10);
    private Rigidbody2D rb;

    private Transform _tf;
    private Vector3 _pos;
    private Vector3 _step;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null && rb.bodyType != RigidbodyType2D.Dynamic) rb = null;
        if (rb != null && rb.bodyType == RigidbodyType2D.Dynamic)
        {
            dir = relative ? transform.rotation * speed : speed;
#if UNITY_6000
            rb.linearVelocity = dir;
#else
            rb.velocity = dir;
#endif
        }

        _tf = transform;
        _pos = _tf.position;
        RefreshStep();
    }

    private void OnEnable()
    {
        if (_tf == null) return;
        _pos = _tf.position;
        RefreshStep();
    }

    private void OnTransformParentChanged()
    {
        RefreshStep();
    }

    public void RefreshStep()
    {
        var stepDir = relative ? (Vector3)(_tf.rotation * speed) : speed;
        _step = stepDir * Time.fixedDeltaTime;
    }

    private Vector2 dir;
    public void ManagedFixedUpdate()
    {
        if (rb != null) return;

        _pos += _step;
        _tf.position = _pos;
    }

    public ManagerBase CreateManager() => new ManagedUpdateManager<ConstantMovement>();


    public void SetSpeed(Vector3 newSpeed)
    {
        speed = newSpeed;
        RefreshStep();
    }

}

public interface ISpeedProvider
{
    public void SetSpeed(Vector3 newSpeed);
}