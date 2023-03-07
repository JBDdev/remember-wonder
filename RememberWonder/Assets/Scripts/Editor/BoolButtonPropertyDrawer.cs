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

        var initColor = GUI.backgroundColor;

        //If we have a color corresponding to the current bool state, set bg color to that.
        if (!property.boolValue && TypedAttrib.falseColor is Color fColor)
            GUI.backgroundColor = fColor;
        else if (property.boolValue && TypedAttrib.trueColor is Color tColor)
            GUI.backgroundColor = tColor;

        //If we have no text override, use default label text in this button. When it's clicked, swap the bool value.
        if (GUI.Button(position, TypedAttrib.textOverride != null ? TypedAttrib.textOverride : label.text, EditorStyles.miniButton))
        {
            property.boolValue = !property.boolValue;
        }

        GUI.backgroundColor = initColor;

        EditorGUI.EndProperty();
    }
}
