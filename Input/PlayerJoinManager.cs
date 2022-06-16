#if INCONTROL_EXISTS

using System;
using System.Collections.Generic;
using System.Linq;
using InControl;
using stoogebag;
using UniRx;
using UnityEngine;

public abstract class PlayerJoinManager : MonoBehaviour
{
    private List<PlayerBase> _connectedPlayers = new List<PlayerBase>();

    public int MaxPlayers = 4;

    public event Action<PlayerBase> PlayerConnected;

    public IObservable<PlayerBase> PlayerConnectedObservable =>
        Observable.FromEvent<PlayerBase>(h => PlayerConnected += h, h => PlayerConnected -= h);

    public event Action<PlayerBase> PlayerDisconnected;

    public IObservable<PlayerBase> PlayerDisconnectedObservable =>
        Observable.FromEvent<PlayerBase>(h => PlayerDisconnected += h, h => PlayerDisconnected -= h);

    void Update()
    {
        var controllers = InputManager.Devices.Where(t => t.DeviceClass == InputDeviceClass.Controller);

        foreach (var controller in controllers)
        {
            CheckForControllerInput(controller);
        }

        CheckKeyboard();
    }

    private void CheckKeyboard()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryConnect(null, true);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TryDisconnectKeyboard();
        }
    }

    void CheckForControllerInput(InputDevice controller)
    {
        var a = controller.GetControl(InputNames.aButton);
        if (a.WasPressed)
        {
            TryConnect(controller, false);
        }

        var b = controller.GetControl(InputNames.bButton);
        if (b.WasPressed)
        {
            TryDisconnect(controller);
        }
    }

    private void TryDisconnectKeyboard()
    {
        var index = _connectedPlayers.IndexOfFirst(t => t?.IsKeyboard == true);
        if (index == -1) return;

        DisconnectPlayer(_connectedPlayers[index]);
    }

    private void TryDisconnect(InputDevice controller)
    {
        var index = _connectedPlayers.IndexOfFirst(t => t?.Controller?.GUID == controller.GUID);
        if (index == -1) return;
        DisconnectPlayer(_connectedPlayers[index]);
    }


    private void TryConnect(InputDevice controller, bool isKeyboard)
    {
        if (controller != null && isKeyboard) throw new Exception("you can't be both");
        
        if (_connectedPlayers.Count() == MaxPlayers) return; //game full.
        if (_connectedPlayers.Any(t => t.IsKeyboard)) return; //we exist already

        var player = new PlayerBase();
        player.Controller = controller;
        player.IsKeyboard = isKeyboard;

        player.PlayerName = GetNewPlayerName();
        player.Color = GetNewPlayerColor();
        player.PlayerNumber = GetNewPlayerNumber();
        player.IsLocal = true;
        ConnectPlayer(player);
    }

    private void ConnectPlayer(PlayerBase player)
    {
        _connectedPlayers.Add(player);
        PlayerConnected?.Invoke(player);
    }

    private void DisconnectPlayer(PlayerBase player)
    {
        _connectedPlayers.Remove(player);
        PlayerDisconnected?.Invoke(player);
    }

    public abstract string GetNewPlayerName();
    public abstract Color GetNewPlayerColor();

    public int GetNewPlayerNumber()
    {
        for (int i = 1;true; i++)
        {
            if (_connectedPlayers.Any(t => t.PlayerNumber == i)) continue;
            else return i;
        }
    }

    public PlayerBase[] GetConnectedPlayers()
    {
        var max = _connectedPlayers.Max(t => t.PlayerNumber) - 1;
        var array = new PlayerBase[max];
        
        foreach (var p in _connectedPlayers)
        {
            array[p.PlayerNumber - 1] = p;
        }
        return array;
        
    }

}

public class PlayerBase
{
    public string PlayerName { get; set; }
    public int PlayerNumber { get; set; }
    public bool IsKeyboard { get; set; }
    public InputDevice Controller { get; set; }
    public Color Color { get; set; }
    public bool IsLocal { get; set; }
    
}

#endif