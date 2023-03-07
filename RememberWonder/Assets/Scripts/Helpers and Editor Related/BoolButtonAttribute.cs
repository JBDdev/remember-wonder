using UnityEngine;

/// <summary>
/// Replaces the normal bool inspector with a button.
/// </summary>
public class BoolButtonAttribute : PropertyAttribute
{
    public readonly string textOverride;
    public readonly Color? falseColor;
    public readonly Color? trueColor;

    public BoolButtonAttribute(string textOverride = null)
    {
        this.textOverride = textOverride;
        this.falseColor = null;
        this.trueColor = null;
    }
    public BoolButtonAttribute(string textOverride, Color falseColor)
    {
        this.textOverride = textOverride;
        this.falseColor = falseColor;
        this.trueColor = null;
    }
    public BoolButtonAttribute(string textOverride, Color falseColor, Color trueColor)
    {
        this.textOverride = textOverride;
        this.falseColor = falseColor;
        this.trueColor = trueColor;
    }
}
