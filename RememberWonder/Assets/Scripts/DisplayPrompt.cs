using Bewildered;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayPrompt : MonoBehaviour
{
    enum PromptPositionType { Default, TriggererStatic, TriggererFollow }

    [SerializeField] private BoxCollider triggerCol;
    [SerializeField] private GameObject promptObj;
    [SerializeField] private PushPullObject grabbablePromptOwner;
    [Space(10)]
    [SerializeField] private PromptPositionType positionType;
    [Tooltip("Unused if position type does not involve the triggerer.")]
    [SerializeField] private Vector3 offsetFromTriggerer;
    [SerializeField] private UHashSet<TagString> activatorTags;
    [Space(5)]
    [Tooltip("In units per second. If 0.5, will take 2 seconds to reach a scale of 1.")]
    [SerializeField] private float appearSpeed;
    [SerializeField] private float disappearSpeed;
    [SerializeField] private float overshootAmount;
    [SerializeField] private float overshootBounceSpeed;

    private Vector3 initScale;
    private Coroutine promptCorout;
    private Coroutine followCorout;
    private Vector3 promptOffset;

    private Collider lastKnownTriggerer;

    public static DisplayPrompt activePromptDisplayer = null;
    private static Transform promptContainer = null;

    /// <summary>
    /// Invoked whenever this prompt is told to start appearing/disappearing.
    /// <br/>- <see cref="bool"/>: Whether this prompt should be appearing or not.
    /// <br/>- <see cref="Collider"/>: The collider that triggered the state change. Null if not triggered by a collision event.
    /// </summary>
    public System.Action<bool, Collider> PromptStateChange;

    public PushPullObject GrabbablePromptOwner { get => grabbablePromptOwner; set => grabbablePromptOwner = value; }

    private void Start()
    {
        initScale = promptObj.transform.localScale;

        promptObj.transform.localScale = Vector3.zero;
        promptObj.SetActive(false);
        promptObj.name += $" ( {(transform.parent.parent ? transform.parent.parent.name : transform.parent.name)} )";

        if (!promptContainer) promptContainer = new GameObject("Prompt Container").transform;

        promptOffset = promptObj.transform.localPosition;
        ((RectTransform)promptObj.transform).SetParent(promptContainer.transform, false);

        if (positionType == PromptPositionType.Default)
            Coroutilities.DoUntil(this, () => promptObj.transform.position = transform.position + promptOffset, () => !Application.isPlaying);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!activatorTags.Contains(other.tag)) return;

        //If other's an activator, since it just entered, this must not be the active prompt; make this the active prompt and appear.
        TriggerPromptChange(true, true, this, other);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!activatorTags.Contains(other.tag) || activePromptDisplayer) return;

        //If other's an activator and there's no other active prompt, make this the active prompt and appear.
        TriggerPromptChange(true, true, this, other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!activatorTags.Contains(other.tag) || activePromptDisplayer != this) return;

        //If other's an activator and we're the active prompt, disappear and make us not the active prompt.
        TriggerPromptChange(false, true, null, other);
    }

    private void Update()
    {
        //If this is the active prompt...
        if (activePromptDisplayer == this)
        {
            //...but it's not showing (and isn't grabbed, if applicable),
            if (!promptObj.activeSelf && (!grabbablePromptOwner || !grabbablePromptOwner.IsGrabbed))
            {
                //appear without setting (no need, it's already set).
                TriggerPromptChange(true, false);
            }
            //...and we're showing...
            else if (promptObj.activeSelf)
            {
                var triggerGlobalCenter = transform.TransformPoint(triggerCol.center);
                var sqrDistanceToTriggerExtent = transform.TransformVector(triggerCol.size / 2).sqrMagnitude;

                //...and the last known triggerer is farther away from the trigger's center than the trigger's extents,
                if (Vector3.Distance(lastKnownTriggerer.ClosestPoint(triggerGlobalCenter), triggerGlobalCenter) >= sqrDistanceToTriggerExtent)
                {
                    //disappear and make this not the active prompt.
                    TriggerPromptChange(false, true, null);
                }
            }
        }

        //If there is an active prompt, it's not this, and this prompt is showing, disappear.
        if (activePromptDisplayer && activePromptDisplayer != this && promptObj.activeSelf)
        {
            TriggerPromptChange(false, false);
        }
        //If the grabbable object the prompt's tied to is grabbed, disappear the prompt, but keep it as the active prompt.
        else if (grabbablePromptOwner && grabbablePromptOwner.IsGrabbed)
        {
            TriggerPromptChange(false, false);
        }
    }

    private void TriggerPromptChange(bool shouldAppear, bool setActivePrompt,
        DisplayPrompt newActivePrompt = null, Collider triggerer = null)
    {
        if (triggerer) lastKnownTriggerer = triggerer;

        //If we should appear but we're already appearing/appeared, no need to do anything.
        //  Preventing disappear redundancy is harder, has side effects, and *should* be unnecessary. Should.
        if (shouldAppear && activePromptDisplayer == this && promptObj.activeSelf)
            return;

        if (setActivePrompt)
            activePromptDisplayer = newActivePrompt;

        Coroutilities.TryStopCoroutine(this, ref promptCorout);
        if (shouldAppear)
        {
            switch (positionType)
            {
                default:
                case PromptPositionType.Default:
                    break;

                case PromptPositionType.TriggererStatic:
                    promptObj.transform.position = triggerer.transform.position + offsetFromTriggerer;
                    break;

                case PromptPositionType.TriggererFollow:
                    followCorout = Coroutilities.DoUntil(this,
                        () => promptObj.transform.position = triggerer.transform.position + offsetFromTriggerer,
                        () => activePromptDisplayer != this);
                    break;
            }

            promptCorout = StartCoroutine(PromptAppear());
        }
        else
        {
            Coroutilities.TryStopCoroutine(this, ref followCorout);
            promptCorout = StartCoroutine(PromptDisappear());
        }

        PromptStateChange?.Invoke(shouldAppear, triggerer);
    }

    private IEnumerator PromptAppear()
    {
        promptObj.SetActive(true);

        while (promptObj.transform.localScale != initScale * overshootAmount)
        {
            promptObj.transform.localScale = Vector3.MoveTowards(
                promptObj.transform.localScale,
                initScale * overshootAmount,
                appearSpeed * Time.deltaTime);

            yield return null;
        }
        while (promptObj.transform.localScale != initScale)
        {
            promptObj.transform.localScale = Vector3.MoveTowards(
                promptObj.transform.localScale,
                initScale,
                overshootBounceSpeed * Time.deltaTime);

            yield return null;
        }

        promptCorout = null;
    }
    private IEnumerator PromptDisappear()
    {
        while (promptObj.transform.localScale != Vector3.zero)
        {
            promptObj.transform.localScale = Vector3.MoveTowards(
                promptObj.transform.localScale,
                Vector3.zero,
                disappearSpeed * Time.deltaTime);

            yield return null;
        }

        promptObj.SetActive(false);
        promptCorout = null;
    }
}
