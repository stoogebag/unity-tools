using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using stoogebag;
using UniRx;
using UnityEngine.UI;

public class ConnectionUI : MonoBehaviour
{
    
    [InitialiseOnAwake]
    private Button Host;
    
    [InitialiseOnAwake]
    public Button Join;

    private void Awake()
    {
        Host = this.GetChild<Button>("Host");
        Join = this.GetChild<Button>("Join");

        Host.OnClickAsObservable().Subscribe(u =>
        {
            NetworkConnectManager.Instance.StartServer();
            NetworkConnectManager.Instance.TryConnect();
        });
        
        Join.OnClickAsObservable().Subscribe(u =>
        {
            NetworkConnectManager.Instance.TryConnect();

            //var textInput = TextInputPopup.Create();

        });

    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
