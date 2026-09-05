#if ADVENTURE_CREATOR

using System.Collections;
using System.Collections.Generic;
using AC;
using UnityEditor;
using UnityEngine;

public class BindHotspot
{
    
    [MenuItem("GameObject/stooge/Hotspot Init")]
    static void Bind()
    {
      //labels it parent's name
      //GetComponent<>();
      
        var selection = Selection.activeGameObject;
        if (selection.TryGetComponent<Hotspot>(out var hs))
        {
            hs.SetName(selection.transform.parent.name, 0);
        }
    }
}
#endif