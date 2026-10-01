using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ActivationSource))]
public class InteractableButton : Interactable
{
    [SerializeField] private string prompt = "Press";
    [SerializeField] private bool startActive;

    private ActivationSource _state;
    private PressInteraction _interaction;

    public ActivationSource State => _state;

    private void Awake()
    {
        _state = GetComponent<ActivationSource>();
        if (_state != null)
        {
            _state.SetActive(startActive);
        }

        _interaction = new PressInteraction(this, prompt);
    }

    public override void OfferInteractions(IInteractor interactor, List<IInteraction> into)
    {
        if (_interaction != null)
        {
            into.Add(_interaction);
        }
    }

    public void Toggle()
    {
        if (_state != null)
        {
            _state.SetActive(!_state.IsActive.Value);
        }
    }
}

public sealed class PressInteraction : IInteraction
{
    private readonly InteractableButton _button;
    private readonly string _prompt;

    public PressInteraction(InteractableButton button, string prompt)
    {
        _button = button;
        _prompt = prompt;
    }

    public string Text => _prompt;

    public bool CanPerform(IInteractor interactor) => _button != null;

    public void Perform(IInteractor interactor) => _button.Toggle();
}
