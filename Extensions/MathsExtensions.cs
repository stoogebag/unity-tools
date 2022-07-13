using System;

public static class MathsExtensions
{
    public static (float, float) QuadraticFormula(float a, float b, float c)
    {
        if (a == 0)
        {
            if (b == 0) return (float.NaN, float.NaN);
            else return (-c / b, -c/b);
        }
        
        var disc = Discriminant(a, b, c);
        return ((-b + (float)Math.Sqrt(disc)) / (2 * a),(-b - (float)Math.Sqrt(disc)) / (2 * a));
    }

    public static float Discriminant(float a, float b, float c)
    {
        return b * b - 4 * a * c;
    }
}
