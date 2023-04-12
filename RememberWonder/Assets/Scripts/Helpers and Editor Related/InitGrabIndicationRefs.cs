using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using Bewildered.Editor;

public class InitGrabIndicationRefs : MonoBehaviour
{
#if UNITY_EDITOR
    [Header("EDITOR ONLY")]
    [Header("Self References")]
    [SerializeField] private GrabSparkleController sparkleController;
    [SerializeField] private DisplayPrompt promptController;
    [Header("Affected Grab Object References")]
    [SerializeField] private PushPullObject grabbableOwner;
    [SerializeField] private MeshRenderer sparklingMeshRend;
    [Header("Relative Trigger Offsets")]
    [Space(5)]
    [SerializeField] private bool useSparkleTriggerSizeOffset;
    [SerializeField][EnableIf("useSparkleTriggerSizeOffset")] private Vector3 sparkleTriggerSizeOffset;
    [Space(3)]
    [SerializeField] private bool useSparkleTriggerPosOffset;
    [SerializeField][EnableIf("useSparkleTriggerPosOffset")] private Vector3 sparkleTriggerPosOffset;
    [Space(6)]
    [SerializeField] private bool usePromptTriggerSizeOffset;
    [SerializeField][EnableIf("usePromptTriggerSizeOffset")] private Vector3 promptTriggerSizeOffset;
    [Space(3)]
    [SerializeField] private bool usePromptTriggerPosOffset;
    [SerializeField][EnableIf("usePromptTriggerPosOffset")] private Vector3 promptTriggerPosOffset;

    [SerializeField][HideInInspector] private PushPullObject prevGrabbableOwner;
    [SerializeField][HideInInspector] private MeshRenderer prevSparklingMeshRend;
    //[Space]
    [SerializeField][HideInInspector] private bool prevUseSparkleTriggerSizeOffset;
    [SerializeField][HideInInspector] private Vector3 prevSparkleTriggerSizeOffset;
    [SerializeField][HideInInspector] private bool prevUsePromptTriggerSizeOffset;
    [SerializeField][HideInInspector] private Vector3 prevPromptTriggerSizeOffset;
    //[Space]
    [SerializeField][HideInInspector] private bool prevUseSparkleTriggerPosOffset;
    [SerializeField][HideInInspector] private Vector3 prevSparkleTriggerPosOffset;
    [SerializeField][HideInInspector] private bool prevUsePromptTriggerPosOffset;
    [SerializeField][HideInInspector] private Vector3 prevPromptTriggerPosOffset;

    [Button("Try Get References", EButtonEnableMode.Editor, 15)]
    private void TryGetRefs()
    {
        if (transform.parent)
        {
            var serializedSelf = new UnityEditor.SerializedObject(this);

            if (!grabbableOwner)
                serializedSelf.FindProperty("grabbableOwner").objectReferenceValue = transform.parent.GetComponent<PushPullObject>();
            if (!sparklingMeshRend)
            {
                if (transform.parent.TryGetComponent(out MeshRenderer rendOnParent))
                {
                    serializedSelf.FindProperty("sparklingMeshRend").objectReferenceValue = rendOnParent;
                }
                else
                {
                    for (int i = 0; i < transform.parent.childCount; i++)
                    {
                        if (transform.parent.GetChild(i).TryGetComponent(out MeshRenderer rendOnChild))
                        {
                            serializedSelf.FindProperty("sparklingMeshRend").objectReferenceValue = rendOnChild;
                            break;
                        }
                    }
                }
            }

            serializedSelf.ApplyModifiedProperties();
        }
    }

    [Button("Give Owner This Prompt Reference", EButtonEnableMode.Editor, 3)]
    private void TryGiveOwnerPromptRef()
    {
        if (!promptController)
        {
            Debug.Log($"InitGrabIndicationRefs ( <color=#999>{name}</color> ): No prompt ref to give! Is Prompt Controller assigned?");
            return;
        }
        if (!grabbableOwner)
        {
            Debug.Log($"InitGrabIndicationRefs ( <color=#999>{name}</color> ): No owner to give refs to! Is Grabbable Owner assigned?");
            return;
        }

        var serializedOwner = new UnityEditor.SerializedObject(grabbableOwner);
        var serializedPrompts = serializedOwner.FindProperty("grabPrompts");

        //Praise the lort for Bewildered and their Get/Set extension methods
        //Be warned that if the type of grabPrompts changes, things could maybe get hairy due to this cast (not sure)
        var promptRefs = serializedPrompts.GetValue<Bewildered.UHashSet<DisplayPrompt>>();
        if (promptRefs != null)
        {
            promptRefs.Add(promptController);
            serializedPrompts.SetValue(promptRefs);
        }

        UnityEditor.EditorGUIUtility.PingObject(grabbableOwner);
        serializedOwner.ApplyModifiedProperties();
    }

    [Button("Apply Refs/Offsets", EButtonEnableMode.Editor, 3)]
    private void TryApply()
    {
        ApplyToSparkles();
        ApplyToPrompt();

        CheckOffsetsSetPrevious(true);
    }

    /// <summary>
    /// <i>Currently unused.</i><br/>
    /// Checks whether any of the trigger offsets have changed, THEN optionally<br/>
    /// sets the previous values to the current ones (for next time), THEN returns the check result.
    /// </summary>
    private bool CheckOffsetsSetPrevious(bool shouldSet)
    {
        //Refs are the same?
        bool anyChanges = prevGrabbableOwner == grabbableOwner && prevSparklingMeshRend == sparklingMeshRend;
        //Sparkle size is the same?
        anyChanges = anyChanges && prevUseSparkleTriggerSizeOffset == useSparkleTriggerSizeOffset;
        anyChanges = anyChanges && prevSparkleTriggerSizeOffset == sparkleTriggerSizeOffset;
        //Sparkle pos is the same?
        anyChanges = anyChanges && prevUseSparkleTriggerPosOffset == useSparkleTriggerPosOffset;
        anyChanges = anyChanges && prevSparkleTriggerPosOffset == sparkleTriggerPosOffset;
        //Prompt size is the same?
        anyChanges = anyChanges && prevUsePromptTriggerSizeOffset == usePromptTriggerSizeOffset;
        anyChanges = anyChanges && prevPromptTriggerSizeOffset == promptTriggerSizeOffset;
        //Prompt pos is the same?
        anyChanges = anyChanges && prevUsePromptTriggerPosOffset == usePromptTriggerPosOffset;
        anyChanges = anyChanges && prevPromptTriggerPosOffset == promptTriggerPosOffset;

        if (shouldSet)
        {
            //Refs
            prevGrabbableOwner = grabbableOwner;
            prevSparklingMeshRend = sparklingMeshRend;
            //Sparkle size
            prevUseSparkleTriggerSizeOffset = useSparkleTriggerSizeOffset;
            prevSparkleTriggerSizeOffset = sparkleTriggerSizeOffset;
            //Sparkle pos
            prevUseSparkleTriggerPosOffset = useSparkleTriggerPosOffset;
            prevSparkleTriggerPosOffset = sparkleTriggerPosOffset;
            //Prompt size
            prevUsePromptTriggerSizeOffset = usePromptTriggerSizeOffset;
            prevPromptTriggerSizeOffset = promptTriggerSizeOffset;
            //Prompt pos
            prevUsePromptTriggerPosOffset = usePromptTriggerPosOffset;
            prevPromptTriggerPosOffset = promptTriggerPosOffset;
        }

        return anyChanges;
    }

    private void ApplyToSparkles()
    {
        if (!sparkleController) return;

        var serializedSparkleController = new UnityEditor.SerializedObject(sparkleController);
        var serializedSparklePSystem = new UnityEditor.SerializedObject(serializedSparkleController.
            FindProperty("sparkleSystem").objectReferenceValue);

        if (grabbableOwner)
            serializedSparkleController.FindProperty("grabbableSparkleOwner").objectReferenceValue = grabbableOwner;

        if (sparklingMeshRend)
        {
            serializedSparkleController.FindProperty("sparklingMeshRend").objectReferenceValue = sparklingMeshRend;
            serializedSparklePSystem.FindProperty("ShapeModule").
                FindPropertyRelative("m_MeshRenderer").objectReferenceValue = sparklingMeshRend;
        }

        serializedSparklePSystem.ApplyModifiedProperties();
        serializedSparkleController.ApplyModifiedProperties();

        sparkleController.InitTrigger(
            useSparkleTriggerSizeOffset ? sparkleTriggerSizeOffset : null,
            useSparkleTriggerPosOffset ? sparkleTriggerPosOffset : null);
    }

    private void ApplyToPrompt()
    {
        if (!promptController) return;

        var serializedPromptController = new UnityEditor.SerializedObject(promptController);

        if (grabbableOwner)
            serializedPromptController.FindProperty("grabbablePromptOwner").objectReferenceValue = grabbableOwner;

        SetPromptTriggerSize();

        serializedPromptController.ApplyModifiedProperties();
    }

    private void SetPromptTriggerSize()
    {
        if (!usePromptTriggerSizeOffset || !sparklingMeshRend) return;

        var serializedPromptTransform = new UnityEditor.SerializedObject(promptController.transform);

        Vector3 adjustedScale = UtilFunctions.InverseScale(sparklingMeshRend.bounds.size, transform.lossyScale);
        adjustedScale += UtilFunctions.InverseScale(promptTriggerSizeOffset, transform.lossyScale);
        //Go through every axis of the adjusted scale,
        for (int i = 0; i < 3; i++)
        {
            //and if it's zero or less, make it very small instead.
            //  (Negative scales are technically valid here, but likely not expected/desired.)
            adjustedScale[i] = adjustedScale[i] <= 0 ? 1E-5f : adjustedScale[i];
        }

        Vector3 adjustedPosition = transform.InverseTransformPoint(sparklingMeshRend.bounds.center);
        adjustedPosition += UtilFunctions.InverseScale(promptTriggerPosOffset, transform.lossyScale);

        serializedPromptTransform.FindProperty("m_LocalScale").vector3Value = adjustedScale;
        serializedPromptTransform.FindProperty("m_LocalPosition").vector3Value = adjustedPosition;

        serializedPromptTransform.ApplyModifiedProperties();
    }
#endif
}
