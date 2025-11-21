using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantMovement : MonoBehaviour
{

    [SerializeField] private bool relative = false;
    public Vector3 speed = new Vector3(10, 10, 10);

    // Update is called once per frame
    void FixedUpdate()
    {
        if(relative)
            transform.localPosition = transform.localPosition + speed * Time.fixedDeltaTime;
        else 
            transform.position = transform.position + speed * Time.fixedDeltaTime;
    }
}
