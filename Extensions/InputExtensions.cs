using System;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

namespace stoogebag.Extensions
{
    public static class InputExtensions
    {
        public static bool GetKeyComboDown(params KeyCode[] keys)
        {
            return GetKeyCombo(keys) && keys.Any(k => UnityEngine.Input.GetKeyDown(k));
        }

        public static bool GetKeyCombo(params KeyCode[] keys)
        {
            foreach (var key in keys)
            {
                if (!UnityEngine.Input.GetKey(key)) return false;
            }

            return true;
        }


        public static IObservable<InputAction.CallbackContext> OnPerformedAsObservable(this InputAction action, int throttleInMilliseconds = 100 )
        {
            action.Enable();

            return Observable.FromEvent<InputAction.CallbackContext>(
                h => action.performed += h,
                h => action.performed -= h
            ).ThrottleFirst(System.TimeSpan.FromMilliseconds(throttleInMilliseconds));
        }
    }
}