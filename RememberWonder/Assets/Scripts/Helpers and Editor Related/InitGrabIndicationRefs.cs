using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitGrabIndicationRefs : MonoBehaviour
{
#if UNITY_EDITOR
    [Header("Editor Only")]
    [SerializeField] GrabSparkleController sparkleController;
    [SerializeField] DisplayPrompt promptController;
    [Space(10)]
    [SerializeField] PushPullObject grabbableOwner;
    [SerializeField] MeshRenderer sparklingMeshRend;
    [Space(10)]
    [SerializeField][BoolButton("Get")] private bool getRefs = false;
    [Space(5)]
    [SerializeField][BoolButton("Apply")] private bool applyRefsToTriggers = false;

    private void OnValidate() => ValidationUtility.DoOnDelayCall(this, () =>
    {
        TryGetRefs();
        TryApplyRefs();
    });

    private void TryGetRefs()
    {
        if (!getRefs) return;

        if (transform.parent)
        {
            if (!grabbableOwner) transform.parent.TryGetComponent(out grabbableOwner);
            if (!sparklingMeshRend) transform.parent.TryGetComponent(out sparklingMeshRend);
        }

        getRefs = false;
    }

    private void TryApplyRefs()
    {
        if (!applyRefsToTriggers) return;

        if (grabbableOwner)
        {
            sparkleController.GrabbableSparkleOwner = grabbableOwner;
            promptController.GrabbablePromptOwner = grabbableOwner;
        }
        if (sparklingMeshRend)
        {
            sparkleController.SparklingMeshRend = sparklingMeshRend;
        }

        applyRefsToTriggers = false;
    }
#endif
}
