using UnityEngine;
using UniRx;
using System;
using System.Xml.Serialization;

public abstract class InputSchemeBase : MonoBehaviour
{
    public abstract float GetHorizontal();
    public abstract float GetVertical();

    public abstract Vector3 GetLookTarget();

    public ReactiveProperty<bool> ActionButtonValue = new();
    public ReactiveProperty<bool> ShootButtonValue = new();
    public ReactiveProperty<bool> ItemButtonValue= new();

    public ReactiveProperty<bool> SprintButtonValue= new();
    public ReactiveProperty<bool> AbilityButtonValue= new();

    public ReactiveProperty<bool> UpButtonValue= new();
    public ReactiveProperty<bool> DownButtonValue= new();
    public ReactiveProperty<bool> LeftButtonValue= new();
    public ReactiveProperty<bool> RightButtonValue= new();
   
    
    /// <summary>
    /// this should be unique to the controller i think
    /// </summary>
    /// <returns></returns>
    public abstract string GetID();

}
