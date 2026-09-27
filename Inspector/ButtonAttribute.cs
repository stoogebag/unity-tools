using System;

[AttributeUsage(AttributeTargets.Method)]
public sealed class ButtonAttribute : Attribute
{
    public readonly string Label;
    public readonly UnityIcon Icon = UnityIcon.None;

    public ButtonAttribute(string label = null) => Label = label;

    public ButtonAttribute(UnityIcon icon, string label = null)
    {
        Icon = icon;
        Label = label;
    }
}

[AttributeUsage(AttributeTargets.Method)]
public sealed class ButtonGroupAttribute : Attribute
{
    public readonly string Name;

    public ButtonGroupAttribute(string name = null) => Name = name;
}
