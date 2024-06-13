using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFollowParent : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        transform.position = Camera.main.WorldToScreenPoint(transform.parent.parent.position);
    }
}
