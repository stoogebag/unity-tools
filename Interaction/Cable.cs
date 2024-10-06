using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class Cable : MonoBehaviour
{
    public Activateable Child;
    public PoweredState Powered;


    public Renderer renderer;
    public Color UnpoweredColor = Color.gray;
    public Color PoweredColor = Color.yellow;

    public static float PowerUpDuration = 0.5f;
    public static float PowerDownDuration = 0.1f;
    
    public void Toggle()
    {
        
    }

    [Button]
    public void Power()
    {
        Powered = PoweredState.Powering;
        
        renderer.material.DOColor(PoweredColor, PowerUpDuration).OnComplete(() => { OnPowered(); });
    }

    private void OnPowered()
    {
        Powered = PoweredState.Powered;
        if(Child != null)
            Child.OnParentPowered();
    }


    [Button]
    public void UnPower()
    {
        Powered = PoweredState.Unpowering;
        OnUnpowered();
        renderer.material.DOColor(UnpoweredColor, PowerDownDuration).OnComplete(()=>
        {
        });
    }
    
    private void OnUnpowered()
    {
        Powered = PoweredState.Unpowered;
        if(Child != null) Child.OnParentUnpowered();
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