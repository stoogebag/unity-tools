using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using stoogebag.UITools.Windows;

public class ActivateChildren : MonoBehaviour,IWindowAnimation
{
    public List<Window> WindowsToActivate;


    //todo: make this robust in case of some cancellations?
    public async UniTask<bool> Activate()
    {
        await UniTask.WhenAll(Enumerable.Select(WindowsToActivate, t => t.Activate()).ToArray());

        return true;
    }

    public async UniTask<bool> Deactivate()
    {
        await UniTask.WhenAll(WindowsToActivate.Select(t => t.Deactivate()).ToArray());

        return true;
    }
}
