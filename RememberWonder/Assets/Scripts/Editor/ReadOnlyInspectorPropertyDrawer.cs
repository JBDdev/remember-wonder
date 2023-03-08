using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ReadOnlyInspectorAttribute))]
public class ReadOnlyInspectorPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        var initState = GUI.enabled;
        GUI.enabled = false;

        EditorGUI.PropertyField(position, property, label);

        GUI.enabled = initState;
        EditorGUI.EndProperty();
    }
}
