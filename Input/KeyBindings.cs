using UnityEngine;

namespace stoogebag_MonuMental.stoogebag.Input
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

        public KeyCode StartKey = KeyCode.Space;
        public KeyCode CancelKey = KeyCode.Escape;

    }
}