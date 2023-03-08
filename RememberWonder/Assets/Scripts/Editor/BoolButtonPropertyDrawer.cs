using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(BoolButtonAttribute))]
public class BoolButtonPropertyDrawer : PropertyDrawer
{
    private BoolButtonAttribute _typedAttrib;
    private BoolButtonAttribute TypedAttrib
    {
        get
        {
            _typedAttrib ??= attribute as BoolButtonAttribute;
            return _typedAttrib;
        }
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        if (TypedAttrib.useLabel)
            position = EditorGUI.PrefixLabel(position, label);

        var initColor = GUI.backgroundColor;

        /*//If we have a color corresponding to the current bool state, set bg color to that.
        //  (If we don't have a true color, the bool state doesn't matter.)
        if ((!property.boolValue || TypedAttrib.trueColor is not Color) && TypedAttrib.falseColor is Color fColor)
            GUI.backgroundColor = fColor;

        else if (property.boolValue && TypedAttrib.trueColor is Color tColor)
            GUI.backgroundColor = tColor;*/

        //If we have no text override, use default label text in this button.
        var buttonText = TypedAttrib.textOverride ?? label.text;
        //If true and we have true text, use true text instead.
        if (property.boolValue && TypedAttrib.trueText != null)
            buttonText = TypedAttrib.trueText;

        //When it's clicked, swap the bool value.
        if (GUI.Button(position, buttonText, EditorStyles.miniButton))
        {
            property.boolValue = !property.boolValue;
        }

        GUI.backgroundColor = initColor;

        EditorGUI.EndProperty();
    }
}
