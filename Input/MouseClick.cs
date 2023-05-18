using System;
using stoogebag.Extensions;
using UniRx;
using UnityEngine;

public class MouseClick : MonoBehaviour
{
    [SerializeField] private float raycastDistance = 2000;

    private void OnMouseEnter()
    {
    }

    private void OnMouseOver()
    {
        var lm = gameObject.GetLayerMask();
        var hit = (Camera.main.Raycast(Input.mousePosition.WithZ(0), lm, raycastDistance));
        
        if(hit.collider != null) MouseOver?.Invoke(transform.InverseTransformPoint(hit.point));
    }

    public event Action<Vector3> MouseOver;
    public IObservable<Vector3> MouseOverObservable => Observable.FromEvent<Vector3>(h => MouseOver += h, h => MouseOver -= h);
    
    
}