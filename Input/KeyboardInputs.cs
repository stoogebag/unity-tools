using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using stoogebag_MonuMental.stoogebag.Extensions;
using UnityEngine;

namespace stoogebag_MonuMental.stoogebag.Input
{
    public class KeyboardInputs : InputSchemeBase
    {

        private Camera _mainCamera;
        private float distanceToCam = 10f;
    
        [ItemCanBeNull] public List<KeyBindings> Bindings;// = new(){DefaultBindings};

        public void Bind(KeyBindings binding)
        {
            Bindings = new List<KeyBindings>() { binding };
        }

        //public static KeyBindings DefaultBindings = ScriptableObject.CreateInstance<KeyBindings>();
    
        [SerializeField]
        private float y = 0f;

        public static float ButtonPressThreshold = .5f;

        private void Start()
        {
            // ActionButtonValue = new UniRx.ReactiveProperty<bool>(false);
            // ShootButtonValue = new UniRx.ReactiveProperty<bool>(false);
            // SprintButtonValue = new UniRx.ReactiveProperty<bool>(false);
            // AbilityButtonValue = new UniRx.ReactiveProperty<bool>(false);
            // ItemButtonValue = new UniRx.ReactiveProperty<bool>(false);

            _mainCamera = Camera.main;
            distanceToCam = _mainCamera.transform.position.y - y;
        }

        public override float GetHorizontal()
        {
            var val = 0;
            val += Bindings.Any(t=> UnityEngine.Input.GetKey(t.KeyLeft)) ? -1 : 0;
            val += Bindings.Any(t=> UnityEngine.Input.GetKey(t.KeyRight)) ? 1 : 0;

            return val;
        }

        public override float GetVertical()
        {
            var val = 0;
            val += Bindings.Any(t=> UnityEngine.Input.GetKey(t.KeyDown)) ? -1 : 0;
            val += Bindings.Any(t=> UnityEngine.Input.GetKey(t.KeyUp)) ? 1 : 0;

            return val;
        }


        // Update is called once per frame
        void FixedUpdate()
        {
            ActionButtonValue.Value =    Bindings.Any(t=>UnityEngine.Input.GetKey(t.KeyAction));
            SprintButtonValue.Value =  Bindings.Any(t=>UnityEngine.Input.GetKey(t.KeySprint));
            AbilityButtonValue.Value = Bindings.Any(t=>UnityEngine.Input.GetKey(t.KeyAbility));
            ShootButtonValue.Value =   UnityEngine.Input.GetMouseButton(0);

            var hori = GetHorizontal();
            LeftButtonValue.Value = hori < -ButtonPressThreshold;
            RightButtonValue.Value = hori > ButtonPressThreshold;

            var vert = GetVertical();
            DownButtonValue.Value =  vert < -ButtonPressThreshold;
            UpButtonValue.Value = vert > ButtonPressThreshold;

        }
    

        public override Vector3 GetLookTarget()
        {
            //use mouse location.
            var loc = _mainCamera.ScreenToWorldPoint(UnityEngine.Input.mousePosition.WithZ(distanceToCam));
            return loc;
        }

        public override string GetID()
        {
            return "Keyboard" + Bindings[0].name; //todo, make this more unique!
        }
    }
}