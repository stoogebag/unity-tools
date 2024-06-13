using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Cable : MonoBehaviour
{
    public Activateable Child;
    public PoweredState Powered;


    public Renderer renderer;
    public Color UnpoweredColor = Color.gray;
    public Color PoweredColor = Color.yellow;

    public static float PowerupDuration = 0.5f;
    
    public void Toggle()
    {
        
    }

    public void Power()
    {
        Powered = PoweredState.Powering;
        
        renderer.material.DOColor(PoweredColor, PowerupDuration).OnComplete(() => { OnPowered(); });
    }

    private void OnPowered()
    {
        Powered = PoweredState.Powered;
        Child.OnParentPowered();
    }


    public void UnPower()
    {
        Powered = PoweredState.Unpowering;
        OnUnpowered();
        renderer.material.DOColor(UnpoweredColor, PowerupDuration).OnComplete(()=>
        {
        });
    }
    
    private void OnUnpowered()
    {
        Powered = PoweredState.Unpowered;
        Child.OnParentUnpowered();
    }

}

public enum PoweredState
{
    Unpowered,
    Powering,
    Powered,
    Unpowering
}

public class Activateable : MonoBehaviour
{
    public List<Cable> Cables;
    public virtual void OnParentUnpowered(){}
    public virtual void OnParentPowered(){}
}