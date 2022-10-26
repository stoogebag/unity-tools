using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public abstract class SelectableUIElement : MonoBehaviour
{
    public UIInteractorBase Owner { get; private set; }
    public List<UISelector> Owners { get; }

    public TemporaryEffectBase[] SelectionEffects;
    public CompositeDisposable _selectedDisposable = new CompositeDisposable();
    
    public SelectableUIElement LeftNeighbour;
    public SelectableUIElement RightNeighbour;
    public SelectableUIElement UpNeighbour;
    public SelectableUIElement DownNeighbour;

    public bool CanInteract(UIInteractorBase interactor)
    {
        return Owner == interactor; //bc: is this good enough?
    }

    private void Start()
    {
        SelectionEffects = GetComponents<TemporaryEffectBase>();
    }

    public virtual void OnSelected(UIInteractorBase player)
    {
        SelectionEffects = GetComponents<TemporaryEffectBase>();
//        print($"selected {name}");
        foreach (var effect in SelectionEffects)
        {
            effect.Activate();
            _selectedDisposable.Add(Disposable.Create(() => effect.Deactivate()));
        }
    }

    public virtual void OnDeselected(UIInteractorBase player)
    {
        _selectedDisposable.Clear();
    }

    public virtual void OnUp   (UISelector player) { }
    public virtual void OnDown (UISelector player) { }
    public virtual void OnLeft (UISelector player) { }
    public virtual void OnRight(UISelector player) { }
    
    public virtual void OnAction(UISelector player) { }


    public void Bind(UIInteractorBase interactor)
    {
        Owner = interactor; 
    }
}