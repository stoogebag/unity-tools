using System;
using TMPro;
using UnityEngine;

public class AnimateOutlineThickness : MonoBehaviour
{
    public float proportion = 1.05f;
    public float period;

    private float startTime = Single.MinValue;

    [SerializeField] //curve
    private AnimationCurve curve;

    [SerializeField] private bool playOnStart;
    [SerializeField] private TextMeshProUGUI text;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        if(playOnStart) Begin();
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
        var t = Mathf.Repeat(timeSinceStart, period) / period ;

        text.outlineWidth = curve.Evaluate(t)* proportion;
        
    }
}
