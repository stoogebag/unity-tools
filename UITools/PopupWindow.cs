using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using stoogebag;

public abstract class PopupWindow : MonoBehaviour
{
    public virtual bool ValidateOK()
    {
        return true;
    }
    public virtual bool ValidateCancel()
    {
        return true;
    }
    public bool HasOKButton;
    public bool HasCancelButton;

    public string OKText = "OK";
    public string CancelText = "Cancel";
}
