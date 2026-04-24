using System;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectableStateListener : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler,
    ISelectHandler, IDeselectHandler
{
    public enum SelectionState { Normal, Highlighted, Pressed, Selected, Disabled }
    
    public IReadOnlyReactiveProperty<SelectionState> CurrentState => _currentState;
    readonly ReactiveProperty<SelectionState> _currentState =
        new ReactiveProperty<SelectionState>(SelectionState.Normal);

    Button button;
    bool isPointerInside;
    bool isPointerDown;
    bool hasSelection;

    void Awake()
    {
        button = GetComponent<Button>();
    }

    void Update()
    {
        // Track interactable changes
        EvaluateState();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerInside = true;
        EvaluateState();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerInside = false;
        EvaluateState();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPointerDown = true;
        EvaluateState();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPointerDown = false;
        EvaluateState();
    }

    public void OnSelect(BaseEventData eventData)
    {
        hasSelection = true;
        EvaluateState();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        hasSelection = false;
        EvaluateState();
    }

    private void OnDisable()
    {
        hasSelection = false; 
        EvaluateState();
    }

    void EvaluateState()
    {
        if (button == null) return;

        SelectionState newState;

        if(!button.IsActive()) 
            newState = SelectionState.Disabled;
        else if (!button.interactable)
            newState = SelectionState.Disabled;
        else if (isPointerDown)
            newState = SelectionState.Pressed;
        else if (hasSelection)
            newState = SelectionState.Selected;
        else if (isPointerInside)
            newState = SelectionState.Highlighted;
        else
            newState = SelectionState.Normal;

        if (newState == CurrentState.Value) return;

        _currentState.Value = newState;
    }
}
