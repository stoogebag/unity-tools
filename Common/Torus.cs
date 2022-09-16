using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using stoogebag;

public class Torus : MonoBehaviour
{
    private BoxCollider boundsCollider;
    private Bounds bounds;

    void Start()
    {
        boundsCollider = FindObjectOfType<StageBounds>().gameObject.GetComponent<BoxCollider>();
        bounds = boundsCollider.bounds;

    }

    // Update is called once per frame
    void Update()
    {
        var p = transform;
        var pos = p.position;
        if (pos.x >= bounds.max.x)
        {
            p.position = p.position.WithX(p.position.x - bounds.size.x);
        }
        else if (pos.x <= bounds.min.x)
        {
            p.position = p.position.WithX(p.position.x + bounds.size.x);
        }

        if (pos.z >= bounds.max.z)
        {
            p.position = p.position.WithZ(p.position.z - bounds.size.z);
        }
        else if (pos.z <= bounds.min.z)
        {
            p.position = p.position.WithZ(p.position.z + bounds.size.z);
        }

    }
}
