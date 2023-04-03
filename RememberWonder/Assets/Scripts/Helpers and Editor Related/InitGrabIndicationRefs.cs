using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

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
    [SerializeField] private Vector3 sparkleTriggerSizeOffset;
    [Space(3)]
    [SerializeField] private bool useSparkleTriggerPosOffset;
    [SerializeField] private Vector3 sparkleTriggerPosOffset;
    [Space(6)]
    [SerializeField] private bool usePromptTriggerSizeOffset;
    [SerializeField] private Vector3 promptTriggerSizeOffset;
    [Space(3)]
    [SerializeField] private bool usePromptTriggerPosOffset;
    [SerializeField] private Vector3 promptTriggerPosOffset;

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
                serializedSelf.FindProperty("sparklingMeshRend").objectReferenceValue = transform.parent.GetComponent<MeshRenderer>();

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

        serializedOwner.FindProperty("grabPrompt").objectReferenceValue = promptController;
        UnityEditor.EditorGUIUtility.PingObject(grabbableOwner);

        serializedOwner.ApplyModifiedProperties();
    }

    [Button("Apply Offsets to Triggers", EButtonEnableMode.Editor, 3)]
    private void TryApply()
    {
        //If refs are the same,
        if (CheckOffsetsSetPrevious(false))
        {
            Debug.Log($"InitGrabIndicationRefs ( <color=#999>{name}</color> ): Nothing to apply!");
            return;
        }

        ApplyToSparkles();
        ApplyToPrompt();

        CheckOffsetsSetPrevious(true);
    }

    /// <summary>
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

        if (grabbableOwner)
            serializedSparkleController.FindProperty("grabbableSparkleOwner").objectReferenceValue = grabbableOwner;

        if (sparklingMeshRend)
            serializedSparkleController.FindProperty("sparklingMeshRend").objectReferenceValue = sparklingMeshRend;

        sparkleController.InitTrigger(useSparkleTriggerSizeOffset ? sparkleTriggerSizeOffset : null);

        serializedSparkleController.ApplyModifiedProperties();
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

        Vector3 adjustedPosition = transform.InverseTransformPoint(sparklingMeshRend.bounds.center);
        adjustedPosition += UtilFunctions.InverseScale(promptTriggerPosOffset, transform.lossyScale);

        serializedPromptTransform.FindProperty("m_LocalScale").vector3Value = adjustedScale;
        serializedPromptTransform.FindProperty("m_LocalPosition").vector3Value = adjustedPosition;

        serializedPromptTransform.ApplyModifiedProperties();
    }
#endif
}
