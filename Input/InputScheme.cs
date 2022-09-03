using UnityEngine;
using UniRx;
using System;
using System.Xml.Serialization;
using FishNet.Serializing;

public abstract class InputSchemeBase 
{
    public abstract float GetHorizontal();
    public abstract float GetVertical();

    public abstract Vector3 GetLookTarget();

    public ReactiveProperty<bool> PingButtonValue;
    public ReactiveProperty<bool> ShootButtonValue;
    public ReactiveProperty<bool> ItemButtonValue;

    public ReactiveProperty<bool> SprintButtonValue;
    public ReactiveProperty<bool> AbilityButtonValue;

    /// <summary>
    /// this should be unique to the controller i think
    /// </summary>
    /// <returns></returns>
    public abstract string GetID();

}
