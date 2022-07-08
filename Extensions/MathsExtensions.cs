using System;

public static class MathsExtensions
{
    public static (float, float) QuadraticFormula(float a, float b, float c)
    {
        var disc = Discriminant(a, b, c);
        return ((-b + (float)Math.Sqrt(disc)) / (2 * a),(-b - (float)Math.Sqrt(disc)) / (2 * a));
    }

    public static float Discriminant(float a, float b, float c)
    {
        return b * b - 4 * a * c;
    }
}
