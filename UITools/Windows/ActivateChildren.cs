using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using stoogebag.UITools.Windows;

public class ActivateChildren : MonoBehaviour,IWindowAnimation
{
    public List<Window> WindowsToActivate;


    //todo: make this robust in case of some cancellations?
    public async Task<bool> Activate()
    {
        await Task.WhenAll(WindowsToActivate.Select(t => t.Activate()).ToArray());

        return true;
    }

    public async Task<bool> Deactivate()
    {
        await Task.WhenAll(WindowsToActivate.Select(t => t.Deactivate()).ToArray());

        return true;
    }
}
