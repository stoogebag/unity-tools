#if BROCCOLI

using Broccoli.Factory;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegenerateTree : MonoBehaviour
{
    [Button]
    void Regenerate()
    {
        GetComponent<TreeFactory>().ProcessPipelinePreview();

    }
}

#endif