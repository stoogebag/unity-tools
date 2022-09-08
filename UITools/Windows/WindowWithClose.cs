using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using stoogebag;
using UniRx;
using UnityEngine.UI;

public class WindowWithClose : Window
{
    public Button Close;
    
   void Start()
    {
        //base.Start();
        Close = gameObject.FirstOrDefault<Button>("Close");
    }

    public override async Task Activate()
    {
        await base.Activate();
        if(Close == null) Close = gameObject.FirstOrDefault<Button>("Close");

        Close.OnClickAsObservable().Subscribe(a => Deactivate()).AddTo(_disposable);
    }

    public override async Task Deactivate()
    {
        _disposable.Clear();
        await base.Deactivate();
    }

    void Update()
    {
        
    }
}
