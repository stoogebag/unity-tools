using System.Collections;
using System.Collections.Generic;
using Animancer;
using Sirenix.OdinInspector;
using stoogebag.Extensions;
using UnityEditor.Animations;
using UnityEngine;

public class BindAnimator : MonoBehaviour
{
    private Animator animator;

    private Rigidbody _rigidbody;
    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
        gameObject.TryGetComponentInAncestor<Rigidbody>(out _rigidbody);
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("Speed", _rigidbody.velocity.magnitude);
    }

    public AnimationClip TestClip;

    [Button]
    void PlayTestClip()
    {
        GetComponent<AnimancerComponent>().Play(TestClip);
    }


}
