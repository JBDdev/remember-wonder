using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PutOnSurface : MonoBehaviour
{
#if UNITY_EDITOR
    [Header("EDITOR ONLY")]
    [SerializeField] private float offsetFromSurface = 0.001f;
    [SerializeField] private bool faceTowardSurface = true;
    [SerializeField] private bool parentToSurface = true;
    [Space(5)]
    [SerializeField] private float maxDistanceFromSurface = 50f;
    [SerializeField] private LayerMask surfaceLayers = ~0;
    [SerializeField] private QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore;
    [Space(10)]
    [SerializeField][BoolButton("Apply to Surface Ahead")] private bool surfaceAhead;
    [Space(5)]
    [SerializeField][BoolButton("Apply to Surface Behind")] private bool surfaceBehind;
    [Space(5)]
    [SerializeField][BoolButton("Apply to Surface Ahead of Camera")] private bool surfaceCamera;

    private void OnValidate() => ValidationUtility.DoOnDelayCall(this, () =>
    {
        var serializedSelf = new SerializedObject(this);

        if (surfaceAhead)
            FindSurfaceInDirection(transform, true);
        else if (surfaceBehind)
            FindSurfaceInDirection(transform, false);
        else if (surfaceCamera)
            FindSurfaceInDirection(SceneView.lastActiveSceneView.camera.transform, true);

        serializedSelf.FindProperty("surfaceAhead").boolValue = false;
        serializedSelf.FindProperty("surfaceBehind").boolValue = false;
        serializedSelf.FindProperty("surfaceCamera").boolValue = false;
        serializedSelf.ApplyModifiedPropertiesWithoutUndo();
    });

    private void FindSurfaceInDirection(Transform referencePoint, bool forward)
    {
        var direction = forward ? referencePoint.forward : -referencePoint.forward;
        Ray surfaceFinderRay = new Ray(referencePoint.position - direction * 0.01f, direction);

        if (Physics.Raycast(surfaceFinderRay, out var hit, maxDistanceFromSurface, surfaceLayers, triggerInteraction))
        {
            var serializedTransform = new SerializedObject(transform);
            var targetParent = parentToSurface ? hit.transform : transform.parent;

            if (parentToSurface)
            {
                Undo.SetTransformParent(transform, hit.transform, $"Parent \"{name}\" to surface \"{hit.transform.name}\"");
                EditorGUIUtility.PingObject(transform);
            }

            serializedTransform.FindProperty("m_LocalPosition").vector3Value =
                targetParent.InverseTransformPoint(hit.point + hit.normal * offsetFromSurface);

            serializedTransform.FindProperty("m_LocalRotation").quaternionValue =
                Quaternion.Inverse(targetParent.rotation) * Quaternion.LookRotation(faceTowardSurface ? -hit.normal : hit.normal);

            serializedTransform.ApplyModifiedProperties();
        }
        else Debug.Log($"PutOnSurface ( <color=#999>{name}</color> ): No surface found!");
    }
#endif
}