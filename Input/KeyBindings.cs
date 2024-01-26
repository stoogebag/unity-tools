using UnityEngine;
using UnityEngine.Serialization;

namespace stoogebag.Input
{
    [CreateAssetMenu()]
    public class KeyBindings : ScriptableObject
    {
        public KeyCode KeyLeft = KeyCode.A;
        public KeyCode KeyRight = KeyCode.D;
        public KeyCode KeyUp = KeyCode.W;
        public KeyCode KeyDown = KeyCode.S;

        public KeyCode KeyAction = KeyCode.Space;
        public KeyCode KeySprint = KeyCode.LeftShift;
        public KeyCode KeyAbility = KeyCode.E;

        public KeyCode KeyLook = KeyCode.E;
        public KeyCode KeyCrouch = KeyCode.LeftControl;

        public KeyCode KeyTab = KeyCode.Tab;
        
        public KeyCode StartKey = KeyCode.Space;
        [FormerlySerializedAs("CancelKey")] public KeyCode KeyCancel = KeyCode.Escape;

        
        public KeyCode KeySelectLeft = KeyCode.Q;
        public KeyCode KeySelectRight = KeyCode.E;
        
    }
}