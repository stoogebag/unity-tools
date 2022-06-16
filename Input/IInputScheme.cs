using UnityEngine;
using UniRx;
using System;

public abstract class InputScheme : MonoBehaviour{
    public abstract float GetHorizontal();
    public abstract float GetVertical();

    public abstract Vector3 GetLookTarget();

    //public abstract void Bind();

    public ReactiveProperty<bool> PingButtonValue;
    public ReactiveProperty<bool> ShootButtonValue;
    public ReactiveProperty<bool> ItemButtonValue;

    public ReactiveProperty<bool> SprintButtonValue;
    public ReactiveProperty<bool> AbilityButtonValue;


}