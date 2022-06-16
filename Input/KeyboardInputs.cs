
#if INCONTROL_EXISTS
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using stoogebag;
using UniRx;

public class KeyboardInputs : InputScheme
{
    public KeyCode KeyLeft = KeyCode.A;
    public KeyCode KeyRight = KeyCode.D;
    public KeyCode KeyUp = KeyCode.W;
    public KeyCode KeyDown = KeyCode.S;

    public KeyCode KeyPing = KeyCode.Space;
    public KeyCode KeySprint = KeyCode.LeftShift;
    public KeyCode KeyAbility = KeyCode.E;


    private Camera _mainCamera;
    private float distanceToCam = 10f;

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
        val += Input.GetKey(KeyLeft) ? -1 : 0;
        val += Input.GetKey(KeyRight) ? +1 : 0;

        return val;
    }

    public override float GetVertical()
    {
        var val = 0;
        val += Input.GetKey(KeyDown) ? -1 : 0;
        val += Input.GetKey(KeyUp) ? +1 : 0;

        return val;
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        PingButtonValue.Value = Input.GetKey(KeyPing);
        SprintButtonValue.Value = Input.GetKey(KeySprint);
        AbilityButtonValue.Value = Input.GetKey(KeyAbility);
        ShootButtonValue.Value = Input.GetMouseButton(0);
    }

    public override Vector3 GetLookTarget()
    {
        //use mouse location.
        var loc = _mainCamera.ScreenToWorldPoint(Input.mousePosition.WithZ(distanceToCam));
        return loc;
    }
}
#endif