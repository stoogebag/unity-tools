using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using stoogebag;

public class StripWhitespace : MonoBehaviour
{

    [Button]
    void StripWhitespaceFromChildNames()
    {
        gameObject.ForAllChildrenRecursive(go=> go.name = (go.name.Trim()));
    }

}
