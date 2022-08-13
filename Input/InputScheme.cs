using UnityEngine;
using UniRx;
using System;

public abstract class InputSchemeBase : MonoBehaviour{
    public abstract float GetHorizontal();
    public abstract float GetVertical();

    public abstract Vector3 GetLookTarget();

    public ReactiveProperty<bool> PingButtonValue;
    public ReactiveProperty<bool> ShootButtonValue;
    public ReactiveProperty<bool> ItemButtonValue;

    public ReactiveProperty<bool> SprintButtonValue;
    public ReactiveProperty<bool> AbilityButtonValue;


}
