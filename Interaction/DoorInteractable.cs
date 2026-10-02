using System.Collections.Generic;
using UnityEngine;

public class DoorInteractable : Interactable
{
    [SerializeField] private Door door;
    [SerializeField] private string requiredKey = "";

    private DoorInteraction _interaction;

    private void Awake()
    {
        if (door == null)
        {
            door = GetComponent<Door>();
        }

        _interaction = new DoorInteraction(door, requiredKey);
    }

    public override void OfferInteractions(IInteractor interactor, List<IInteraction> into)
    {
        if (_interaction != null)
        {
            into.Add(_interaction);
        }
    }
}

public sealed class DoorInteraction : IInteraction
{
    private readonly Door _door;
    private readonly string _requiredKey;

    public DoorInteraction(Door door, string requiredKey)
    {
        _door = door;
        _requiredKey = requiredKey;
    }

    public string Text => _door != null && _door.IsOpen ? "Close" : "Open";

    public bool CanPerform(IInteractor interactor)
    {
        if (_door == null || !_door.CanOperate(interactor))
        {
            return false;
        }

        return string.IsNullOrEmpty(_requiredKey) || interactor.HasKey(_requiredKey);
    }

    public void Perform(IInteractor interactor) => _door.Toggle();
}
