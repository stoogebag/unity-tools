using UnityEngine;

namespace stoogebag.UITools.Selection_and_Input
{
    public class UIInteractorBase : MonoBehaviour
    {
    
        protected SelectableUIElement _selectedElement;
        protected void Select(SelectableUIElement uiElement)
        {
            if (_selectedElement == uiElement) return;
        
            _selectedElement?.OnDeselected(this);
            _selectedElement = uiElement;
            _selectedElement?.OnSelected(this);
        }
    
    }
}