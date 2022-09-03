using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using stoogebag;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    public PlayerInfo Owner { get; private set; }

    private Button button;
    private Vector3 originalScale;

    public float hoverScaleFactor = 1.4f;
    private Func<bool> _enabledFunc;
    private Action _onClick;

    private void Awake()
    {
        button = GetComponent<Button>();
        originalScale = transform.localScale;
    }

    public void Hover()
    {
        if (_enabledFunc?.Invoke() == false) return;
        
        transform.localScale = hoverScaleFactor * originalScale;
    }
    public void UnHover()
    {
        //print($"unhovered ({name}. player? {Owner.Name})");
        transform.localScale = originalScale;
    }

    public void Click()
    {
        if (_enabledFunc?.Invoke() == false) return;
        
        _onClick?.Invoke();
        button.onClick?.Invoke();
    }

    public void Bind(Action onClick, Func<bool> enabledFunc = null)
    {
        _onClick = onClick;
        _enabledFunc = enabledFunc;
    }
    
    
}
