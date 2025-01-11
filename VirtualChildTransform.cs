using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//behaves like a child transform, but isn't really.
//todo: when to update? use observables? for now, Update()
//todo: weird scale stuff? idk man. i'll just do position for now and see if something comes up.
[ExecuteInEditMode]
public class VirtualChildTransform : MonoBehaviour
{

    [SerializeField]
    Transform parent;

    // Update is called once per frame
    void Update()
    {
        transform.position = parent.position;
    }
}
