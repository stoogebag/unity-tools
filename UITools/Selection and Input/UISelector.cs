using System;
using System.Collections.Generic;
using UniRx;

public class UISelector : UIInteractorBase
{
    public float cooldown = 0.3f;

    private CompositeDisposable _disposable = new();
    

    public SelectableUIElement InitialSelectedElement;
    private DateTime lastInputTime = DateTime.MinValue;

    public List<InputSchemeBase> Inputs = new();
    
    private void Start()
    {
        foreach (var i in Inputs)
        {
            Bind(i);
        }

        //Initialise();
    }

    public void Initialise()
    {
        Select(InitialSelectedElement);
    }

    public void ClearBindings()
    {
        Select(null);
        _disposable.Clear();
    }
    
    public void Bind(InputSchemeBase input, bool clearBindings = false)
    {
        if(clearBindings) ClearBindings();

        if (input == null) return;
        
        input.ActionButtonValue.Subscribe(val =>
        {
            print(val);
            if (val)
            {
                _selectedElement?.OnAction(this);
                lastInputTime = DateTime.UtcNow;
            }
        }).AddTo(_disposable);
        
        input.UpButtonValue.Subscribe(val =>
        {
            if (val)
            {
                if (_selectedElement == null) return;
                _selectedElement.OnUp(this);
                if(_selectedElement.UpNeighbour != null) Select(_selectedElement.UpNeighbour);
                lastInputTime = DateTime.UtcNow;
            }
        }).AddTo(_disposable);
        
        input.DownButtonValue.Subscribe(val =>
        {
            if (val)
            {
                if (_selectedElement == null) return;
                _selectedElement.OnDown(this);
                if(_selectedElement.DownNeighbour != null) Select(_selectedElement.DownNeighbour);
                lastInputTime = DateTime.UtcNow;
            }
        }).AddTo(_disposable);
        input.LeftButtonValue.Subscribe(val =>
        {
            if (val)
            {
                if (_selectedElement == null) return;
                _selectedElement.OnLeft(this);
                if(_selectedElement.LeftNeighbour != null) Select(_selectedElement.LeftNeighbour);
                lastInputTime = DateTime.UtcNow;
            }
        }).AddTo(_disposable);
        input.RightButtonValue.Subscribe(val =>
        {
            if (val)
            {
                if (_selectedElement == null) return;
                _selectedElement.OnRight(this);
                if(_selectedElement.RightNeighbour != null) Select(_selectedElement.RightNeighbour);
                lastInputTime = DateTime.UtcNow;
            }
        }).AddTo(_disposable);
    }

    private void OnDestroy()
    {
        _disposable.Clear();
    }


}