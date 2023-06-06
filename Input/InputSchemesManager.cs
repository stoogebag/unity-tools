#if INCONTROL_EXISTS
using System.Collections;
using System.Collections.Generic;
using stoogebag.Input;
using stoogebag.Utils;
using UnityEngine;

public class InputSchemesManager : Singleton<InputSchemesManager>
{
    
    public List<KeyBindings> AvailableKeyboardBindings; 
    public List<ControllerBindings> AvailableControllerBindings;
    public Dictionary<string, InputSchemeBase> ConnectedInputs { get; } = new Dictionary<string, InputSchemeBase>();
}
#endif