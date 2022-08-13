
#if INCONTROL_EXISTS
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;
using InControl;
using System.Linq;

public class ControllerInputs : InputSchemeBase
{
    InputDevice controller;

    public static InputControlType AxisMoveLeft = InputControlType.LeftStickLeft;
    public static InputControlType AxisMoveRight = InputControlType.LeftStickRight;
    public static InputControlType AxisMoveUp = InputControlType.LeftStickUp;
    public static InputControlType AxisMoveDown = InputControlType.LeftStickDown;

    public static InputControlType AxisLookLeft = InputControlType.RightStickLeft;
    public static InputControlType AxisLookRight = InputControlType.RightStickRight;
    public static InputControlType AxisLookUp = InputControlType.RightStickUp;
    public static InputControlType AxisLookDown = InputControlType.RightStickDown;
    
    public static InputControlType AxisSprint = InputControlType.LeftTrigger;
    //public static InputControlType AxisShoot = InputControlType.RightTrigger;

    public static List<InputControlType> ButtonPing = new()
    {
        InputControlType.LeftBumper,
        InputControlType.Button4
    };

    public static List<InputControlType> ButtonShoot = new()
    {
        InputControlType.RightTrigger,
        InputControlType.Button7
    };

    public static List<InputControlType> ButtonItem = new()
    {
        InputControlType.Action4,
        //InputControlType.Button4
    };
    public static List<InputControlType> ButtonSprint = new()
    {
        InputControlType.LeftTrigger,
        //InputControlType.Button4
    };






    private void Start()
    {
        PingButtonValue = new UniRx.ReactiveProperty<bool>(false);
        ShootButtonValue = new UniRx.ReactiveProperty<bool>(false);
        SprintButtonValue = new UniRx.ReactiveProperty<bool>(false);
        AbilityButtonValue = new UniRx.ReactiveProperty<bool>(false);
        ItemButtonValue = new UniRx.ReactiveProperty<bool>(false);


        //PingButtonValue.Subscribe(t => print("!" +t));
    }

    public void Setup(InputDevice c)
    {
        controller = c;



    }


    public override float GetHorizontal()
    {
        if (controller == null) return 0;

        var val = 0f;
        val -= controller.GetControl(AxisMoveLeft).Value;
        val += controller.GetControl(AxisMoveRight).Value;
        return val;
    }

    public override float GetVertical()
    {
        if (controller == null) return 0;

        var val = 0f;
        val -= controller.GetControl(AxisMoveDown).Value;
        val += controller.GetControl(AxisMoveUp).Value;

        return val;
    }

    public override Vector3 GetLookTarget()
    {
        if (controller == null) return transform.position;

        var vert = 0f;
        vert -= controller.GetControl(AxisLookDown).Value;
        vert += controller.GetControl(AxisLookUp).Value;

        var hori = 0f;
        hori -= controller.GetControl(AxisLookLeft).Value;
        hori += controller.GetControl(AxisLookRight).Value;
        
        return transform.position + new Vector3(hori, 0, vert);
    }

    // Update is called once per frame
    void Update()
    {
        if(controller == null)
        {
            if(InputManager.ActiveDevice.IsAttached) controller = InputManager.ActiveDevice;

            //print(controller);

            //print(controller.GUID + " ---- " + controller.GetFirstPressedButton().Control);
            //print(controller.LeftBumper.Value);
            //print(controller.GUID + " ---- " + controller.GetFirstPressedAnalog().Control);
        }

        if (controller == null) return;
        if (!controller.IsAttached) return;

        //print(GetVertical());

        //print(controller.GetFirstPressedButton());
        PingButtonValue.Value = ButtonPing.Any(b=> controller.GetControl(b).Value > 0);
        ShootButtonValue.Value = ButtonShoot.Any(b => controller.GetControl(b).Value > 0);
        SprintButtonValue.Value = ButtonSprint.Any(b => controller.GetControl(b).Value > 0);
        ItemButtonValue.Value = ButtonItem.Any(b => controller.GetControl(b).Value > 0);



    }

    public void Bind(InputDevice input)
    {
        controller = input;
    }
}
#endif