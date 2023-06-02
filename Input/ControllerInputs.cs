
#if INCONTROL_EXISTS
using System.Collections;
using UnityEngine;
using System;
using UniRx;
using InControl;
using System.Linq;
using stoogebag.Input;

public class ControllerInputs : InputSchemeBase
{
    public InputDevice controller;
    public ControllerBindings Bindings;// = DefaultBindings;

    //public static ControllerBindings DefaultBindings = ControllerBindings.CreateInstance<ControllerBindings>();

    public void Bind(InputDevice controller, ControllerBindings bindings)
    {
        this.controller = controller;
        this.Bindings = bindings;
    }

    public override float GetHorizontal()
    {
        if (controller == null) return 0;

        var val = 0f;
        val -= controller.GetControl(Bindings.AxisMoveLeft).Value;
        val += controller.GetControl(Bindings.AxisMoveRight).Value;
        return val;
    }

    public override float GetVertical()
    {
        if (controller == null) return 0;

        var val = 0f;
        val -= controller.GetControl(Bindings.AxisMoveDown).Value;
        val += controller.GetControl(Bindings.AxisMoveUp).Value;

        return val;
    }

    public override Vector3 GetLookTarget()
    {
        var vert = 0f;
        vert -= controller.GetControl(Bindings.AxisLookDown).Value;
        vert += controller.GetControl(Bindings.AxisLookUp).Value;

        var hori = 0f;
        hori -= controller.GetControl(Bindings.AxisLookLeft).Value;
        hori += controller.GetControl(Bindings.AxisLookRight).Value;
        
        return new Vector3(hori, 0, vert);
    }

    public override string GetID()
    {
        return "Controller-" + controller.GUID.ToString() + Bindings.name;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Application.isFocused) return;
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
        ActionButtonValue.Value = Bindings.ButtonPing.Any(b=> controller.GetControl(b).Value > 0);
        ShootButtonValue.Value = Bindings.ButtonShoot.Any(b => controller.GetControl(b).Value > 0);
        SprintButtonValue.Value = Bindings.ButtonSprint.Any(b => controller.GetControl(b).Value > 0);
        ItemButtonValue.Value = Bindings.ButtonItem.Any(b => controller.GetControl(b).Value > 0);


        var hori = GetHorizontal();
        LeftButtonValue.Value = hori < -ButtonPressThreshold;
        RightButtonValue.Value = hori > ButtonPressThreshold;

        var vert = GetVertical();
        DownButtonValue.Value =  vert < -ButtonPressThreshold;
        UpButtonValue.Value = vert > ButtonPressThreshold;

    }

    public static float ButtonPressThreshold = .5f;
    public void Bind(InputDevice input)
    {
        controller = input;
    }
}

#endif