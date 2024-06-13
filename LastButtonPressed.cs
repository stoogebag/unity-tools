#if INCONTROL_EXISTS
using System.Collections;
using System.Collections.Generic;
using InControl;
using UnityEngine;

public class LastButtonPressed : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(InputManager.ActiveDevice.Action1.WasPressed) print($"pressed 1");
        if(InputManager.ActiveDevice.Action2.WasPressed) print($"pressed 2");
        if(InputManager.ActiveDevice.Action3.WasPressed) print($"pressed 3");
        if(InputManager.ActiveDevice.Action4.WasPressed) print($"pressed 4");
    }
}

#endif