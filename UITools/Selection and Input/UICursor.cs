#if INCONTROL_EXISTS


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InControl;
using UnityEngine;
using stoogebag;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UICursor :  UIInteractorBase
{
    private PlayerInfo _player;
    public InputSchemeBase input;

    public InputDevice controller ;
    public bool isMouse;

    private Image im;
    public float cursorSpeed = 300f;

    public MenuButton hovered;
    
    private void Awake()
    {
        im = GetComponent<Image>();
    }

    private void Update()
    {
        bool buttonDown;

        if (isMouse) buttonDown = Input.GetMouseButtonDown(0);
        else buttonDown = controller?.Action1.WasPressed ?? false;
        
        PointerEventData pointerData = new PointerEventData (EventSystem.current)
        {
            pointerId = -1,
        };
         
        pointerData.position = transform.position;
        List<RaycastResult> results = new List<RaycastResult>(); //todo: should i get rid of this? probably not important.
        EventSystem.current.RaycastAll(pointerData, results);
        
        var but = results.Select(t => t.gameObject.GetComponent<SelectableUIElement>()).WhereNotNull().FirstOrDefault();
        if (but != null)
        {
            if (but.CanInteract(this))
            {
                Select(but);
            }
            // if (but.Owner == null || but.Owner == _player)
            // {
            //
            //     if (buttonDown) Click(but);
            //     else Hover(but);
            // }
            // else Hover(null);
        }
        else
        {
            Select(null);
        }
    }

    // private void Click(MenuButton but)
    // {
    //     but.Click();
    // }
    //
    // private void Hover(MenuButton but)
    // {
    //     if (hovered == but) return;
    //
    //     hovered?.UnHover();
    //     hovered = but;
    //     hovered?.Hover();
    //
    //
    // }

    void LateUpdate()
    {
        HandlePosition();
    }

    private void HandlePosition()
    {
        if (isMouse)
        {
            var mousePos = Input.mousePosition;
            transform.position = mousePos;
        }
        else if (controller != null)
        {
            var valX = 0f;
            var valY = 0f;
            valX += input.GetHorizontal();
            valY += input.GetVertical();

            var vec = new Vector3(valX * Time.deltaTime * cursorSpeed,
                valY * Time.deltaTime * cursorSpeed,
                0);

            transform.position += vec;
        }
    }

    public void Bind(PlayerInfo player)
    {
        _player = player;
        input = player?.Input;
        controller = (input as ControllerInputs)?.controller;
        isMouse = (input is KeyboardInputs);
        im.color = player.Color;
    }

    public void Unbind()
    {
        _player = null;
        
    }
}


#endif