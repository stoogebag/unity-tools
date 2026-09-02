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
    }

    private Vector2 dir;
    // Update is called once per frame
    public void ManagedFixedUpdate()
    {
        if (rb != null)
        {
            return;
        }
        
        Vector3 delta;

        if (relative)
        {
            // Move in this object's local space (respecting its rotation)
            delta = transform.rotation * speed * Time.fixedDeltaTime; // rotate speed into world space [web:20]
            transform.position += delta;
        }
        else
        {
            // Move in world space using the speed as-is
            delta = speed * Time.fixedDeltaTime;
            transform.position += delta;
        }
    }


    public void SetSpeed(Vector3 newSpeed)
    {
        speed = newSpeed;
    }

}

public interface ISpeedProvider
{
    public void SetSpeed(Vector3 newSpeed);
}