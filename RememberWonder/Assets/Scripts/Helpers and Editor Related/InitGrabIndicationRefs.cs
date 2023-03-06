using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitGrabIndicationRefs : MonoBehaviour
{
#if UNITY_EDITOR
    [Header("Editor Only")]
    [SerializeField] private GrabSparkleController sparkleController;
    [SerializeField] private DisplayPrompt promptController;
    [Space(10)]
    [SerializeField] private PushPullObject grabbableOwner;
    [SerializeField] private MeshRenderer sparklingMeshRend;
    [Space(5)]
    [SerializeField] private bool useSparkleTriggerSizeOffset;
    [SerializeField] private Vector3 sparkleTriggerSizeOffset;
    [SerializeField] private bool usePromptTriggerSizeOffset;
    [SerializeField] private Vector3 promptTriggerSizeOffset;
    [Space(10)]
    [SerializeField][BoolButton] private bool getRefs = false;
    [Space(5)]
    [SerializeField][BoolButton] private bool applyToTriggers = false;

    [SerializeField][HideInInspector] private PushPullObject prevGrabbableOwner;
    [SerializeField][HideInInspector] private MeshRenderer prevSparklingMeshRend;
    [SerializeField][HideInInspector] private bool prevUseSparkleTriggerSizeOffset;
    [SerializeField][HideInInspector] private Vector3 prevSparkleTriggerSizeOffset;
    [SerializeField][HideInInspector] private bool prevUsePromptTriggerSizeOffset;
    [SerializeField][HideInInspector] private Vector3 prevPromptTriggerSizeOffset;

    private void OnValidate() => ValidationUtility.DoOnDelayCall(this, () =>
    {
        TryGetRefs();
        TryApply();

        getRefs = false;
        applyToTriggers = false;
    });

    private void TryGetRefs()
    {
        if (!getRefs) return;

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

    private void TryApply()
    {
        if (!applyToTriggers) return;

        if (prevGrabbableOwner == grabbableOwner
            && prevSparklingMeshRend == sparklingMeshRend
            && prevUseSparkleTriggerSizeOffset == useSparkleTriggerSizeOffset
            && prevSparkleTriggerSizeOffset == sparkleTriggerSizeOffset
            && prevUsePromptTriggerSizeOffset == usePromptTriggerSizeOffset
            && prevPromptTriggerSizeOffset == promptTriggerSizeOffset)
        {
            Debug.Log($"InitGrabIndicationRefs ( <color=#999>{name}</color> ): Nothing to apply!");
            return;
        }

        ApplyToSparkles();
        ApplyToPrompt();

        prevGrabbableOwner = grabbableOwner;
        prevSparklingMeshRend = sparklingMeshRend;
        prevSparkleTriggerSizeOffset = sparkleTriggerSizeOffset;
        prevPromptTriggerSizeOffset = promptTriggerSizeOffset;
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

        SetTriggerSizes();

        serializedPromptController.ApplyModifiedProperties();
    }

    private void SetTriggerSizes()
    {
        if (!usePromptTriggerSizeOffset || !sparklingMeshRend) return;

        var serializedPromptTransform = new UnityEditor.SerializedObject(promptController.transform);

        Vector3 adjustedScale = UtilFunctions.InverseScale(sparklingMeshRend.bounds.size, transform.lossyScale);
        adjustedScale += UtilFunctions.InverseScale(promptTriggerSizeOffset, transform.lossyScale);

        Vector3 adjustedPosition = transform.InverseTransformPoint(sparklingMeshRend.bounds.center);

        serializedPromptTransform.FindProperty("m_LocalScale").vector3Value = adjustedScale;
        serializedPromptTransform.FindProperty("m_LocalPosition").vector3Value = adjustedPosition;

        serializedPromptTransform.ApplyModifiedProperties();
    }
#endif
}
