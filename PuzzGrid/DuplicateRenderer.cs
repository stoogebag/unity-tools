using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using stoogebag.Extensions;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering;

public class DuplicateRenderer : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> Target;
    

    [SerializeField, HideInInspector]
    private List<GameObject> _duplicates = new List<GameObject>();

    //[SerializeField] private Vector3 Offset;

    private void Start()
    {
        Bind();
    }


    [Button]
    void Bind()
    {
        Clear();
        foreach (var go in Target)
        {
            var renderers = go.GetComponentsInDescendants<Renderer>(true);
            foreach (var r in renderers)
            {
                
                
                
                var clone = Instantiate(r, r.transform);

                TransformTransform(go.transform, transform.parent, clone.transform);
                var comps = clone.GetComponents<Component>();
                for (var i = comps.Length - 1; i >= 0; i--)
                {
                    if(comps[i] is Transform || comps[i] is MeshRenderer || comps[i] is MeshFilter)
                        continue;
                    DestroyImmediate(comps[i]);
                }
                
                // r.transform.ObserveEveryValueChanged(t => t.position)
                //     .Subscribe(pos => clone.transform.position = pos + Offset)
                //     .AddTo(clone.gameObject);
                //
                // r.ObserveEveryValueChanged(t=>t.enabled)
                //     .Subscribe(enabled => clone.enabled = enabled)
                //     .AddTo(clone.gameObject);
                
                _duplicates.Add(clone.gameObject);

                
                
            }
        }

    }

    [Button]
    void Clear()
    {
        _duplicates.DestroyAllImmediateAndClear();
    }
    
    
    public void TransformTransform(Transform oldOrigin,  Transform newOrigin, Transform t) 
    {
        var pos = newOrigin.TransformPoint(oldOrigin.InverseTransformPoint(t.position));
        var rot = newOrigin.rotation * Quaternion.Inverse(oldOrigin.rotation) * t.rotation;
        
        t.SetPositionAndRotation(pos, rot);
    }
    
}
