using System;
using UnityEngine;

public class ApplyForce : MonoBehaviour
{
    public Vector3 force = new Vector3(1,0,0);

    public bool ApplyOnAwake = true;

    private void Awake()
    {
        if(ApplyOnAwake) Apply();
    }

    // Update is called once per frame
    void Apply()
    {
        if (TryGetComponent<Rigidbody2D>(out var rb2d))
        {
            rb2d.AddForce(force);
        }
        
        if (TryGetComponent<Rigidbody>(out var rb))
        {
            rb.AddForce(force);
        }


    }
}
