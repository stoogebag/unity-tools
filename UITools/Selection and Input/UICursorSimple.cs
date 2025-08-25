#if UNIRX


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using stoogebag;
using stoogebag.Extensions;
using stoogebag.Input;
using stoogebag.Networking.MatchSetup;
using stoogebag.UITools.ElementBindingComponents;
using stoogebag.UITools.Selection_and_Input;
using UnityEngine.EventSystems;
using UnityEngine.UI;


//note: probably you should make this raycastTarget=false! 
[RequireComponent(typeof(Image))]
public class UICursorSimple : UIInteractorBase
{
    private Image im;
    public float cursorSpeed = 300f;

    public MenuButton hovered;

    [SerializeField] private EditorBool hideSystemCursor = EditorBool.Build;

    private void Awake()
    {
        im = GetComponent<Image>();
        
        if(hideSystemCursor == EditorBool.Always) Cursor.visible = false;
        if(hideSystemCursor == EditorBool.Editor && Application.isEditor) Cursor.visible = false;
        if(hideSystemCursor == EditorBool.Build && !Application.isEditor) Cursor.visible = false;
    }

    void LateUpdate()
    {
        HandlePosition();
    }

    private void HandlePosition()
    {
        {
            var mousePos = Input.mousePosition;
            transform.position = mousePos;
        }
    }
}


public enum EditorBool
{
    Always,
    Editor,
    Build,
    Never,
}


#endif