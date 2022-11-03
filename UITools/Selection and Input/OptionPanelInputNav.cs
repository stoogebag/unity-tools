using stoogebag_MonuMental.stoogebag.Input;
using stoogebag_MonuMental.stoogebag.UITools.ElementBindingComponents;
using UnityEngine;

namespace stoogebag_MonuMental.stoogebag.UITools.Selection_and_Input
{
    [RequireComponent(typeof(OptionPanel))]
    public class OptionPanelInputNav : SelectableUIElement
    {
        private OptionPanel _panel;

        public InputSchemeBase Input;

        public Orientation Orientation = Orientation.Horizontal;
        private void Awake()
        {
            _panel = GetComponent<OptionPanel>();
        }
        public override void OnLeft(UISelector player)
        {
            if (Orientation == Orientation.Horizontal)
            {
                _panel.TryChangeIndex(-1);
            }
        }
        public override void OnRight(UISelector player)
        {
            if (Orientation == Orientation.Horizontal)
            {
                _panel.TryChangeIndex(1);
            }
        }
    
        public override void OnUp(UISelector player)
        {
            if (Orientation == Orientation.Vertical)
            {
                _panel.TryChangeIndex(1);
            }
        }
        public override void OnDown(UISelector player)
        {
            if (Orientation == Orientation.Vertical)
            {
                _panel.TryChangeIndex(-1);
            }
        }
        public override void OnAction(UISelector player)
        {
            _panel.TryChangeIndex(-1);
        }
    }

    public enum Orientation
    {
        Vertical,
        Horizontal,
    }
}