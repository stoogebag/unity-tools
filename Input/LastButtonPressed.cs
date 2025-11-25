#if INCONTROL_EXISTS
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        //print(InputManager.ActiveDevice.GUID);;
        //print(InputManager.ActiveDevice.Name);

        if (!InputManager.Devices.Any())
        {
            print("no devices!");
            return;
        }

        //print(InputManager.ActiveDevice.GetControl(InputControlType.LeftTrigger).RawValue);
        
        
         foreach (var dev in InControl.InputManager.Devices) {
          //   Debug.Log($"{dev.Name}: {string.Join(", ", dev.Controls.Where(c=>c.HasChanged).Select(c=>$"{c.Handle}={c.Value:F2}"))}");
         }
         Debug.Log(InputManager.ActiveDevice.AnyButtonIsPressed); // Should show device when you press buttons

        
        if(InputManager.ActiveDevice.AnyButtonIsPressed) print("somethni");
        
        if(InputManager.Devices[0].AnyButtonIsPressed) print("somethni");
        
        if(InputManager.ActiveDevice.Action1.WasPressed) print($"pressed 1");
        if(InputManager.ActiveDevice.Action2.WasPressed) print($"pressed 2");
        if(InputManager.ActiveDevice.Action3.WasPressed) print($"pressed 3");
        if(InputManager.ActiveDevice.Action4.WasPressed) print($"pressed 4");
    }
}

#endif