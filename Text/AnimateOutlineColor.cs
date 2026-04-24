using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class AnimateOutlineColor : MonoBehaviour
{
    public float proportion = 1.05f;
    public float period = 1f;

    private float startTime = Single.MinValue;
    

    [SerializeField] private bool PlayOnStart = true;
    private TextMeshProUGUI tmp;
    
    
    [GradientUsage(true)]   
    [SerializeField]
    private Gradient gradient;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        
        
        if(PlayOnStart)
            Begin();
    }

    private void Begin()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (startTime == Single.MinValue) return;

        var timeSinceStart = Time.time - startTime;
        var t = Mathf.Repeat(timeSinceStart, period)/period;
        print(t);
        tmp.outlineColor = gradient.Evaluate(t);
    }
}
