using UnityEngine;

/// <summary>
/// Replaces the normal bool inspector with a button.
/// </summary>
public class BoolButtonAttribute : PropertyAttribute
{
    public readonly string textOverride;

    public BoolButtonAttribute(string textOverride = null)
    {
        this.textOverride = textOverride == null ? "" : textOverride;
    }
}
