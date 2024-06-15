using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

public class PatrolRoute : MonoBehaviour
{
    public SharedGameObjectList PatrolPoints;
    
    [Button]
    void BindPoints()
    {
        PatrolPoints = new List<GameObject>();
        foreach (Transform child in transform)
        {
            PatrolPoints.Value.Add(child.gameObject);
        }
    }

    private void Reset()
    {
        BindPoints();
    }

    private void OnDrawGizmos()
    {
        for (var i = 0; i < PatrolPoints.Value.Count; i++)
        {
            var p1 = PatrolPoints.Value[i];
            var p2 = PatrolPoints.Value[(i + 1) % PatrolPoints.Value.Count];
            Gizmos.DrawLine(p1.transform.position, p2.transform.position);
            Gizmos.DrawWireSphere(p1.transform.position, 1);
        }
    }
}
