using System.Collections;
using System.Collections.Generic;
using InControl;
using stoogebag.Input;
using stoogebag.Utils;
using UnityEngine;

public class LatestInput : Singleton<LatestInput>
{
    public ControllerInputs Controller;
    public KeyboardInputs KB;

    public InputSchemeBase CurrentInput;
    

    // Update is called once per frame
    void Update()
    {
        if (InputManager.AnyKeyIsPressed)
        {
            CurrentInput = KB;
        }

        if (InputManager.ActiveDevice.AnyButton || InputManager.ActiveDevice.LeftStick.HasChanged || InputManager.ActiveDevice.RightStick.HasChanged)
        {
            CurrentInput = Controller;
            Controller.Bind(InputManager.ActiveDevice, Controller.Bindings);
        }
    }
}
