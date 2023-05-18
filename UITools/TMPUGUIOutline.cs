using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TMPUGUIOutline : MonoBehaviour
{
    [SerializeField]
    private Color32 outlineColor;

    [SerializeField, PropertyRange(0,1)] private float outlineWidth;


    private void OnValidate()
    {
        var text = GetComponent<TextMeshProUGUI>();
        text.outlineColor = outlineColor;

        text.outlineWidth = outlineWidth;
        
        text.SetMaterialDirty();
        text.SetAllDirty();
        text.ForceMeshUpdate();
    }
}
