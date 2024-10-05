using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class HoudiniBakeManager : MonoBehaviour
{
    [Button]
    void BakeAll()
    {
        foreach (var baker in FindObjectsOfType<HoudiniBaker>())
        {
            baker.Bake();
        }
    }
}
