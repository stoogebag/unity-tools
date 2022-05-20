using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotY : MonoBehaviour
{

    public AnimationCurve plot;

    void Start()
    {
        
    }

    void Update()
    {
        plot.AddKey(Time.realtimeSinceStartup, transform.position.y);
    }
}
