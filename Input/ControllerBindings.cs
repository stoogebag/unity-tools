#if INCONTROL_EXISTS
using System.Collections.Generic;
using InControl;
using UnityEngine;

[CreateAssetMenu()]
public class ControllerBindings : ScriptableObject
{
    
    
    public InputControlType AxisMoveLeft = InputControlType.LeftStickLeft;
    public InputControlType AxisMoveRight = InputControlType.LeftStickRight;
    public InputControlType AxisMoveUp = InputControlType.LeftStickUp;
    public InputControlType AxisMoveDown = InputControlType.LeftStickDown;
    public InputControlType AxisLookLeft = InputControlType.RightStickLeft;
    public InputControlType AxisLookRight = InputControlType.RightStickRight;
    public InputControlType AxisLookUp = InputControlType.RightStickUp;
    public InputControlType AxisLookDown = InputControlType.RightStickDown;

    public InputControlType AxisSprint = InputControlType.LeftTrigger;
    //public static InputControlType AxisShoot = InputControlType.RightTrigger;

    public List<InputControlType> ButtonPing = new()
    {
        InputControlType.LeftBumper,
        InputControlType.Button4
    };

    public List<InputControlType> ButtonShoot = new()
    {
        InputControlType.RightTrigger,
        InputControlType.Button7
    };

    public List<InputControlType> ButtonItem = new()
    {
        InputControlType.Action4,
        //InputControlType.Button4
    };

    public List<InputControlType> ButtonSprint = new()
    {
        InputControlType.LeftTrigger,
        //InputControlType.Button4
    };
}
#endif