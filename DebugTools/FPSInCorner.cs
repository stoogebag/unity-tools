using UnityEngine;

public class FPSInCorner : TextInCorner
{
    public override string GetText()
    {
        return (1/Time.unscaledDeltaTime).ToString() + " FPS";
    }
}