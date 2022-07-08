using System;
using System.Linq;
using UnityEngine;

public class FPSInCorner : TextInCorner
{
    public int cacheSize = 5;
    public bool lockOnLowest = true;

    private int index = 0;
    public float waitTime = 5;
    float lowestSeen = float.MaxValue;
    private float[] _cache;

    private void Awake()
    {
        _cache = new float[cacheSize];
    }

    public override string GetText()
    {
        
        
        if (Time.time < waitTime) return "wait plz";
        var val = 1/Time.unscaledDeltaTime;
     
        _cache[index] = val;   var num = val;
        index = (index + 1) % cacheSize;
        if(lockOnLowest){
            lowestSeen =  Math.Min(lowestSeen, val);
            num = lowestSeen;
        }
        else
        {
            num = _cache.Sum(t => t) / cacheSize;
        }
    
        return (num).ToString() + " FPS";
    }
}