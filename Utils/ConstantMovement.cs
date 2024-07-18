using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantMovement : MonoBehaviour
{
        public Vector3 speed = new Vector3(10, 10, 10);

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = transform.position + speed * Time.fixedDeltaTime;
    }
}
