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
        if (surfaceAhead)
            FindSurfaceInDirection(transform, true);
        else if (surfaceBehind)
            FindSurfaceInDirection(transform, false);
        else if (surfaceCamera)
            FindSurfaceInDirection(SceneView.lastActiveSceneView.camera.transform, true);

        surfaceBehind = false;
        surfaceAhead = false;
        surfaceCamera = false;
    });

    private void FindSurfaceInDirection(Transform referencePoint, bool forward)
    {
        var direction = forward ? referencePoint.forward : -referencePoint.forward;
        Ray surfaceFinderRay = new Ray(referencePoint.position - direction * 0.01f, direction);

        if (Physics.Raycast(surfaceFinderRay, out var hit, maxDistanceFromSurface, surfaceLayers, triggerInteraction))
        {
            Undo.RegisterCompleteObjectUndo(transform, $"Move \"{name}\" to hit point {hit.point} on surface \"{hit.transform.name}\"");
            transform.position = hit.point + hit.normal * offsetFromSurface;
            transform.forward = faceTowardSurface ? -hit.normal : hit.normal;
            Undo.IncrementCurrentGroup();

            if (parentToSurface)
            {
                Undo.SetTransformParent(transform, hit.transform, $"Parent \"{name}\" to surface \"{hit.transform.name}\"");
                EditorGUIUtility.PingObject(transform);
            }
        }
        else Debug.Log($"PutOnSurface ( <color=#999>{name}</color> ): No surface found!");
    }
#endif
}