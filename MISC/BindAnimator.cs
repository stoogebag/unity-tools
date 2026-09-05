#if ANIMANCER

using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using stoogebag.Extensions;
using UnityEngine;

public class BindAnimator : MonoBehaviour
{
    private Animator animator;
    public float multiplier = 1f;

    //private Rigidbody2D _rigidbody;
    // Start is called before the first frame update
    void Awake()
    {
        animator= gameObject.FirstOrDefault<Animator>();
     //   gameObject.TryGetComponentInAncestor<Rigidbody2D>(out _rigidbody);
    }


    private Vector3 oldPos;

    [SerializeField]
    private GameObject model;
    
    void LateUpdate()
    {
        var pos = transform.position;
        var velocity = (pos - oldPos)/Time.deltaTime;
        
        animator?.SetFloat("Speed", velocity.magnitude * multiplier);
        oldPos = pos;

    }


}
#endif