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

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        if (GUI.Button(position, TypedAttrib.textOverride, EditorStyles.miniButton))
        {
            property.boolValue = !property.boolValue;
        }

        EditorGUI.EndProperty();
    }
}
