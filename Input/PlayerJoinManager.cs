#if INCONTROL_EXISTS

using System;
using System.Collections.Generic;
using System.Linq;
using FishNet;
using InControl;
using ReactUnity.Helpers;
using stoogebag;
using stoogebag.Extensions;
using stoogebag.Input;
using stoogebag.Networking.MatchSetup;
using stoogebag.Utils;
using UniRx;
using UnityEngine;

public class PlayerJoinManager : Singleton<PlayerJoinManager>
{
    private Dictionary<string,PlayerInfo> _connectedPlayers = new Dictionary<string,PlayerInfo>();

    private Dictionary<string, InputSchemeBase> _connectedInputs = new();

    public int MaxPlayers = 4;

    public event Action<PlayerInfo> PlayerConnected;

    public IObservable<PlayerInfo> PlayerConnectedObservable =>
        Observable.FromEvent<PlayerInfo>(h => PlayerConnected += h, h => PlayerConnected -= h);

    public event Action<PlayerInfo> PlayerDisconnected;

    public IObservable<PlayerInfo> PlayerDisconnectedObservable =>
        Observable.FromEvent<PlayerInfo>(h => PlayerDisconnected += h, h => PlayerDisconnected -= h);


    void Update()
    {
        if (!Application.isFocused) return;
        
        var controllers = InputManager.Devices?.Where(t => t.DeviceClass == InputDeviceClass.Controller);

        if (controllers != null)
        {
            foreach (var controller in controllers)
            {
                CheckForControllerInput(controller);
            }
        }

        CheckKeyboard();
    }

    
    
    private void CheckKeyboard()
    {
        foreach (var bindings in InputSchemesManager.Instance.AvailableKeyboardBindings)
        {
            if (Input.GetKeyDown(bindings.StartKey))
            {
                TryConnect(bindings);
            }
            if (Input.GetKeyDown(bindings.CancelKey))
            {
                var playerInfo = GetPlayer(bindings);
                if(playerInfo != null) TryDisconnect(playerInfo);
            }
        }
    }


    void CheckForControllerInput(InputDevice controller)
    {
        var a = controller.GetControl(InputNames.aButton);

        foreach (var bindings in InputSchemesManager.Instance.AvailableControllerBindings)
        {
            foreach (var type in bindings.ButtonStart)
            {
                var ctrl = controller.GetControl(type);
                if (ctrl.WasPressed)
                {
                    TryConnect(controller,bindings);
                }
            }
            foreach (var type in bindings.ButtonCancel)
            {
                var ctrl = controller.GetControl(type);
                if (ctrl.WasPressed)
                {
                    var player = GetPlayer(controller, bindings);
                    
                    TryDisconnect(player);
                }
            }

        }
    }


    private PlayerInfo GetPlayer(InputDevice controller, ControllerBindings bindings)
    {
        return _connectedPlayers.Values.FirstOrDefault(t =>
        {
            var input = t.Input as ControllerInputs;
            if (input == null) return false;
            return input.Bindings == bindings && input.controller == controller;
        });
    }
    
    private PlayerInfo GetPlayer(KeyBindings bindings)
    {
        return _connectedPlayers.Values.FirstOrDefault(t => (t.Input as KeyboardInputs)?.Bindings?.Contains(bindings) == true);
    }

    private void TryDisconnect(PlayerInfo p)
    {
        if (p == null) return;
        if(_connectedPlayers.Keys.Contains(p.ID)) DisconnectPlayer(p);
    }

    
    
    private void TryConnect(InputDevice controller, ControllerBindings bindings)
    {
        if (GetPlayer(controller, bindings) != null) return;

        var input = gameObject.AddComponent<ControllerInputs>();
        input.Bind(controller, bindings);
        
        var player = new PlayerInfo
        {
            Input =  input,
            connectionID = GetConnectionID(),
        };
        ConnectPlayer(player);
    }
    private void TryConnect(KeyBindings bindings)
    {
        if (GetPlayer(bindings) != null) return;

        var input = gameObject.AddComponent<KeyboardInputs>();
        input.Bind(bindings);
        
        var player = new PlayerInfo
        {
            Input =  input,
            connectionID = GetConnectionID(),
        };
        ConnectPlayer(player);
    }

    


    public int GetConnectionID()
    {
        return InstanceFinder.ClientManager.Connection.ClientId;
    }

    private void ConnectPlayer(PlayerInfo player)
    {
        player.ComputeID();
        _connectedPlayers.Add(player.ID, player);
        _connectedInputs.Add(player.ID, player.Input);

        InputSchemesManager.Instance.ConnectedInputs[player.Input.GetID()] = player.Input;
        
        
        PlayerConnected?.Invoke(player);
    }

    private void DisconnectPlayer(PlayerInfo player)
    {
        _connectedPlayers.Remove(player.ID);
        _connectedInputs.Remove(player.ID);
        
        if(player.Input != null) Destroy(player.Input);
        PlayerDisconnected?.Invoke(player);
    }

    public InputSchemeBase GetInput(string id) => _connectedInputs.TryGetOrDefault(id);

    
    
    // public virtual string GetNewPlayerName()
    // {
    //     
    // }
    //
    // public virtual Color GetNewPlayerColor()
    // {
    //     
    // }
    //
    // public int GetNewPlayerNumber()
    // {
    //     for (int i = 1;true; i++)
    //     {
    //         if (_connectedPlayers.Any(t => t.PlayerNumber == i)) continue;
    //         else return i;
    //     }
    // }

    // public PlayerInfo[] GetConnectedPlayers()
    // {
    //     var max = _connectedPlayers.Max(t => t.PlayerNumber) - 1;
    //     var array = new PlayerInfo[max];
    //     
    //     foreach (var p in _connectedPlayers)
    //     {
    //         array[p.PlayerNumber - 1] = p;
    //     }
    //     return array;
    //     
    // }

}

public static class InputNames
{
    public static readonly InputControlType aButton = InputControlType.Action1;
    public static readonly InputControlType bButton = InputControlType.Action2;
    public static readonly InputControlType xButton = InputControlType.Action3;
    public static readonly InputControlType yButton = InputControlType.Action4;
}

#endif