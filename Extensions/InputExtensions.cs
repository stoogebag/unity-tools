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

        
        public static IObservable<InputAction.CallbackContext> OnStartedAsObservable(this InputAction action, int throttleInMilliseconds = 100 )
        {
            action.Enable();
            if (throttleInMilliseconds == 0)
            {
                return Observable.FromEvent<InputAction.CallbackContext>(
                    h => action.started += h,
                    h => action.started -= h
                );
            }
            else return Observable.FromEvent<InputAction.CallbackContext>(
                h => action.started += h,
                h => action.started -= h
            ).ThrottleFirst(System.TimeSpan.FromMilliseconds(throttleInMilliseconds));
        }
        
        public static IObservable<InputAction.CallbackContext> OnCanceledAsObservable(this InputAction action, int throttleInMilliseconds = 100 )
        {
            action.Enable();
            if (throttleInMilliseconds == 0)
            {
                return Observable.FromEvent<InputAction.CallbackContext>(
                    h => action.canceled += h,
                    h => action.canceled -= h
                );
            }
            else return Observable.FromEvent<InputAction.CallbackContext>(
                h => action.canceled += h,
                h => action.canceled -= h
            ).ThrottleFirst(System.TimeSpan.FromMilliseconds(throttleInMilliseconds));
        }
        

        public static IObservable<InputAction.CallbackContext> OnPerformedAsObservable(this InputAction action, int throttleInMilliseconds = 100 )
        {
            action.Enable();

            if (throttleInMilliseconds == 0)
            {
                return Observable.FromEvent<InputAction.CallbackContext>(
                    h => action.performed += h,
                    h => action.performed -= h
                );
            }
            else return Observable.FromEvent<InputAction.CallbackContext>(
                h => action.performed += h,
                h => action.performed -= h
            ).ThrottleFirst(System.TimeSpan.FromMilliseconds(throttleInMilliseconds));
        }


        public static IObservable<T> RepeatOnHold<T>(this InputAction action, Func<T,T> inputProcessor = null,  int periodInMilliseconds = 100, params int[] initialDelays) where T:struct 
        {
            action.Enable();
            return action.OnPerformedAsObservable(0)
                .Select(t=>t.ReadValue<T>())
                .Select(t=>inputProcessor?.Invoke(t) ?? t)
                .Merge(action.OnCanceledAsObservable(0).Select(_ => default(T)))
                .DistinctUntilChanged()
                .Select(value =>
                {
                    var initialSequence = initialDelays.Select(d => 
                        Observable.Timer(System.TimeSpan.FromMilliseconds(d))
                            .Select(_ => value)
                    );

                    var loop = Observable.Interval(System.TimeSpan.FromMilliseconds(periodInMilliseconds))
                        .Select(_ => value);

                    return Observable.Return(value) 
                            .Concat(Observable.Concat(initialSequence))
                            .Concat(loop);
                })
                .Switch()
                .TakeUntil(action.OnCanceledAsObservable(0))
                .Repeat()
                .ThrottleFirst(System.TimeSpan.FromMilliseconds(10));
            
        }
        
        public static IObservable<Unit> RepeatOnHold(this InputAction action, int periodInMilliseconds = 100, params int[] initialDelays)
        {
            action.Enable();
            return action.OnPerformedAsObservable()
                .SelectMany(_ =>
                {
                    var initialSequence = initialDelays.Select(d => 
                        Observable.Timer(TimeSpan.FromMilliseconds(d))
                            .Select(__ => Unit.Default)
                    );

                    var loop = Observable.Interval(TimeSpan.FromMilliseconds(periodInMilliseconds))
                        .Select(__ => Unit.Default);

                    return Observable.Return(Unit.Default)
                        .Concat(Observable.Concat(initialSequence))
                        .Concat(loop);
                })
                .TakeUntil(action.OnCanceledAsObservable())
                .Repeat()
                .ThrottleFirst(TimeSpan.FromMilliseconds(10));
        }
        
    }
}