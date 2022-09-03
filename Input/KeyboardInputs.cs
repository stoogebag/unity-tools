
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using stoogebag;
using UniRx;

public class KeyboardInputs : InputSchemeBase
{

    private Camera _mainCamera;
    private float distanceToCam = 10f;
    
    [ItemCanBeNull] public List<KeyBindings> Bindings = new(){DefaultBindings};

    public KeyboardInputs(KeyBindings binding)
    {
        Bindings = new() { binding };
    }

    public static KeyBindings DefaultBindings = ScriptableObject.CreateInstance<KeyBindings>();
    
    [SerializeField]
    private float y = 0f;

    private void Start()
    {
        PingButtonValue = new UniRx.ReactiveProperty<bool>(false);
        ShootButtonValue = new UniRx.ReactiveProperty<bool>(false);
        SprintButtonValue = new UniRx.ReactiveProperty<bool>(false);
        AbilityButtonValue = new UniRx.ReactiveProperty<bool>(false);
        ItemButtonValue = new UniRx.ReactiveProperty<bool>(false);

        _mainCamera = Camera.main;
        distanceToCam = _mainCamera.transform.position.y - y;
    }

    public override float GetHorizontal()
    {
        var val = 0;
        val += Bindings.Any(t=> Input.GetKey(t.KeyLeft)) ? -1 : 0;
        val += Bindings.Any(t=> Input.GetKey(t.KeyRight)) ? 1 : 0;

        return val;
    }

    public override float GetVertical()
    {
        var val = 0;
        val += Bindings.Any(t=> Input.GetKey(t.KeyDown)) ? -1 : 0;
        val += Bindings.Any(t=> Input.GetKey(t.KeyUp)) ? 1 : 0;

        return val;
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        PingButtonValue.Value =    Bindings.Any(t=>Input.GetKey(t.KeyAction));
        SprintButtonValue.Value =  Bindings.Any(t=>Input.GetKey(t.KeySprint));
        AbilityButtonValue.Value = Bindings.Any(t=>Input.GetKey(t.KeyAbility));
        ShootButtonValue.Value =   Input.GetMouseButton(0);
    }

    public override Vector3 GetLookTarget()
    {
        //use mouse location.
        var loc = _mainCamera.ScreenToWorldPoint(Input.mousePosition.WithZ(distanceToCam));
        return loc;
    }

    public override string GetID()
    {
        return "Keyboard"; //todo, make this more unique!
    }
}