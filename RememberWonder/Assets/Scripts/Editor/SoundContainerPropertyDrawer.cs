using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SoundContainer))]
public class SoundContainerPropertyDrawer : PropertyDrawer
{
    private bool expanded;
    private int linesInUse = 2;
    private float unexpandedBottomPadding = 3;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var arrayProp = property.FindPropertyRelative("clips");
        expanded = arrayProp.isExpanded;

        var activeArrayItems = expanded
            ? arrayProp.arraySize
            : 0;

        //Two lines are in use unexpanded. When expanded, add lines equal to count, plus two extra lines for
        //the +/- under the array(? shouldn't it only need one? idk, it's not sized right if it's not two) 
        linesInUse = 2 + activeArrayItems + (expanded ? 2 : 0);

        return base.GetPropertyHeight(property, label) * linesInUse + (expanded
            ? 0
            : unexpandedBottomPadding);
    }

    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        var oneLinePos = position;
        oneLinePos.height = (oneLinePos.height - (expanded ? 0 : unexpandedBottomPadding)) / linesInUse;
        EditorGUI.PropertyField(oneLinePos, property.FindPropertyRelative("randomOrder"));

        oneLinePos.y += oneLinePos.height;
        EditorGUI.PropertyField(oneLinePos, property.FindPropertyRelative("clips"));

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}
