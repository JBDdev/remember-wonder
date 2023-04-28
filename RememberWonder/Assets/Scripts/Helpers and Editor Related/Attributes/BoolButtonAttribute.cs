using UnityEngine;

/// <summary>
/// Replaces the normal bool inspector with a button.
/// </summary>
public class BoolButtonAttribute : PropertyAttribute
{
    //COLOR IS CURRENTLY NON FUNCTIONAL
    public readonly string textOverride;
    public readonly string falseColor;
    public readonly string trueColor;
    public readonly bool useLabel;
    public readonly string trueText;

    public BoolButtonAttribute(string textOverride = null)
    {
        this.textOverride = textOverride;
        this.falseColor = null;
        this.trueColor = null;
        this.useLabel = false;
        this.trueText = null;
    }
    public BoolButtonAttribute(string textOverride, bool useLabel)
    {
        this.textOverride = textOverride;
        this.falseColor = null;
        this.trueColor = null;
        this.useLabel = useLabel;
        this.trueText = null;
    }
    public BoolButtonAttribute(string textOverride, string buttonColor)
    {
        this.textOverride = textOverride;
        this.falseColor = buttonColor;
        this.trueColor = null;
        this.useLabel = false;
        this.trueText = null;
    }
    public BoolButtonAttribute(string textOverride, string buttonColor, bool useLabel)
    {
        this.textOverride = textOverride;
        this.falseColor = buttonColor;
        this.trueColor = null;
        this.useLabel = useLabel;
        this.trueText = null;
    }
    public BoolButtonAttribute(string textOverride, string falseColor, string trueColor)
    {
        this.textOverride = textOverride;
        this.falseColor = falseColor;
        this.trueColor = trueColor;
        this.useLabel = false;
        this.trueText = null;
    }
    public BoolButtonAttribute(string falseText, string trueText, string falseColor, string trueColor, bool useLabel)
    {
        this.textOverride = falseText;
        this.falseColor = falseColor;
        this.trueColor = trueColor;
        this.useLabel = useLabel;
        this.trueText = trueText;
    }
}
