using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantMovement : MonoBehaviour, ISpeedProvider
{
    //must have a parent for this to work!
    [SerializeField] private bool relative = false;

    public Vector3 speed = new Vector3(10, 10, 10);

    // Update is called once per frame
    void FixedUpdate()
    {
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