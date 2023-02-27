using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SourceSettings))]
public class SourceSettingsPropertyDrawer : PropertyDrawer
{
    bool unfolded = false;
    int numberOfVars = 12;
    float lineSpacing = 2;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var editedHeight = base.GetPropertyHeight(property, label);
        if (unfolded)
        {
            editedHeight += lineSpacing;
            editedHeight *= numberOfVars + 1;   //Plus one for the header/foldout itself
            editedHeight -= lineSpacing;        //No need extra line spacing on the bottom line
        }
        return editedHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        var oneLinePos = position;
        oneLinePos.height = unfolded
            ? (position.height - lineSpacing * numberOfVars) / (numberOfVars + 1)
            : oneLinePos.height;

        //To save unfolded state per-instance of SourceSettings, we can apparently use isExpanded on one of the properties
        //in the foldout, even if said property doesn't expand. See http://answers.unity.com/answers/1160771/view.html
        var propInFoldout = property.FindPropertyRelative("loop");

        propInFoldout.isExpanded = EditorGUI.Foldout(oneLinePos, propInFoldout.isExpanded, label, true);
        unfolded = propInFoldout.isExpanded;
        if (!unfolded) return;

        // Draw fields; don't indent because minibuttons are immune to indentation(?!?!?!?)
        DrawFauxNullable(ref oneLinePos, property, "loop", "useLoop");
        DrawFauxNullable(ref oneLinePos, property, "priority", "usePriority");
        DrawFauxNullable(ref oneLinePos, property, "volume", "useVolume");
        DrawFauxNullable(ref oneLinePos, property, "pitch", "usePitch");
        DrawFauxNullable(ref oneLinePos, property, "stereoPan", "useStereoPan");
        DrawFauxNullable(ref oneLinePos, property, "spatialBlend", "useSpatialBlend");
        DrawFauxNullable(ref oneLinePos, property, "reverbZoneMix", "useReverbZoneMix");
        DrawFauxNullable(ref oneLinePos, property, "dopplerLevel", "useDopplerLevel");
        DrawFauxNullable(ref oneLinePos, property, "spread", "useSpread");
        DrawFauxNullable(ref oneLinePos, property, "volumeRolloff", "useVolumeRolloff");
        DrawFauxNullable(ref oneLinePos, property, "minDistance", "useMinDistance");
        DrawFauxNullable(ref oneLinePos, property, "maxDistance", "useMaxDistance");

        EditorGUI.EndProperty();
    }

    private void DrawFauxNullable(ref Rect position, SerializedProperty baseProp, string propName, string toggleName)
    {
        position.y += position.height + lineSpacing;

        var buttonRect = position;
        var indentRect = position;
        buttonRect.width = 25;
        indentRect.x += buttonRect.width + 3;
        indentRect.width -= buttonRect.width + 3;

        var toggleProp = baseProp.FindPropertyRelative(toggleName);

        var buttStyle = EditorStyles.miniButton;
        var initColor = GUI.backgroundColor;
        GUI.backgroundColor = toggleProp.boolValue
            ? new Color(0.741f, 0.569f, 0.396f)
            : new Color(0.478f, 0.541f, 0.659f);
        buttStyle.padding.left = 1;
        buttStyle.padding.right = 1;
        buttStyle.alignment = TextAnchor.MiddleCenter;

        if (GUI.Button(buttonRect, toggleProp.boolValue ? "On" : "Off", buttStyle))
        {
            toggleProp.boolValue = !toggleProp.boolValue;
        }
        GUI.backgroundColor = initColor;

        bool prevEnableState = GUI.enabled;
        var prevLabelWidth = EditorGUIUtility.labelWidth;
        GUI.enabled = toggleProp.boolValue;

        EditorGUIUtility.labelWidth = indentRect.width / 3;
        EditorGUI.PropertyField(indentRect, baseProp.FindPropertyRelative(propName));

        EditorGUIUtility.labelWidth = prevLabelWidth;
        GUI.enabled = prevEnableState;
    }
}