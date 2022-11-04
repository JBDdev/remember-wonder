using UnityEngine;

/// <summary>
/// Adds an "apply" button next to/beneath a field. This button will call<br/>
/// "<c>OnApplyClicked(<see cref="UnityEditor.SerializedProperty"/> field)</c>" on the<br/>
/// class instance the field belongs to.<br/><br/>
/// Largely extrapolated from <see href="https://docs.unity3d.com/ScriptReference/PropertyDrawer.html"/><br/>
/// by Patrick Mitchell @ <see href="https://patrickode.github.io/"/>.
/// </summary>
public class ApplyButtonAttribute : PropertyAttribute
{
    public readonly bool newLine;
    public readonly string btnTextOverride;

    public ApplyButtonAttribute(bool newLine = false, string btnTextOverride = null)
    {
        this.newLine = newLine;
        this.btnTextOverride = string.IsNullOrWhiteSpace(btnTextOverride)
            ? "Apply"
            : btnTextOverride;
    }
}
