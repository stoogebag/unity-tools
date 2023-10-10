using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantMovement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //print("hissds");
        transform.position = transform.position + new Vector3(20, 10, 03) * Time.fixedDeltaTime;
    }
}
