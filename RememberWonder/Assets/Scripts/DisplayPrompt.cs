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
    private Vector3 promptOffset;

    private Coroutine promptCorout;
    private Coroutine followCorout;
    private Coroutine appearBufferCorout;

    private Collider lastKnownTriggerer;

    public static DisplayPrompt activePromptDisplayer = null;
    private static Transform promptContainer = null;
    private static bool somethingIsGrabbed;

    /// <summary>
    /// Invoked whenever this prompt is told to start appearing/disappearing.
    /// <br/>- <see cref="bool"/>: Whether this prompt should be appearing or not.
    /// <br/>- <see cref="Collider"/>: The collider that triggered the state change. Null if not triggered by a collision event.
    /// <br/>- <see cref="bool"/>: Is this prompt currently active (promptObj.activeSelf)?
    /// </summary>
    public System.Action<bool, Collider, bool> PromptStateChange;

    public bool IsActivePrompt { get => activePromptDisplayer == this; }
    public PushPullObject GrabbablePromptOwner { get => grabbablePromptOwner; set => grabbablePromptOwner = value; }

    //--- Setup ---//

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

    private void OnEnable()
    {
        PlayerMovement.GrabStateChange += SetSomethingIsGrabbed;
    }
    private void OnDisable()
    {
        PlayerMovement.GrabStateChange -= SetSomethingIsGrabbed;
    }
    private void SetSomethingIsGrabbed(bool isGrabbed) => somethingIsGrabbed = isGrabbed;

    //--- Core Functions ---//

    private void OnTriggerEnter(Collider other)
    {
        if (somethingIsGrabbed || !activatorTags.Contains(other.tag)) return;

        //If other's an activator, since it just entered, this must not be the active prompt; make this the active prompt and appear.
        TriggerPromptChange(true, true, this, other);
    }

    private void OnTriggerStay(Collider other)
    {
        if (somethingIsGrabbed || !activatorTags.Contains(other.tag) || activePromptDisplayer) return;

        //If other's an activator and there's no other active prompt, make this the active prompt and appear.
        TriggerPromptChange(true, true, this, other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (somethingIsGrabbed || !activatorTags.Contains(other.tag) || !IsActivePrompt) return;

        //If other's an activator and we're the active prompt, disappear and make us not the active prompt.
        TriggerPromptChange(false, true, null, other);
    }

    private void Update()
    {
        //If this is the active prompt...
        if (IsActivePrompt)
        {
            //...but it's not showing (and isn't grabbed, if applicable),
            if (!promptObj.activeSelf && (!grabbablePromptOwner || !grabbablePromptOwner.IsGrabbed))
            {
                //appear without setting (no need, it's already set).
                TriggerPromptChange(true, false);
            }

            //...and it's owner is currently grabbed, disappear, but remain the active prompt.
            //  Only run once by checking if it's already disappeared or on its way.
            else if (grabbablePromptOwner && grabbablePromptOwner.IsGrabbed
                && promptObj.activeSelf && promptCorout == null)
            {
                TriggerPromptChange(false, false);
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
        else if (activePromptDisplayer && promptObj.activeSelf)
        {
            TriggerPromptChange(false, false);
        }
    }

    private void TriggerPromptChange(bool shouldAppear, bool setActivePrompt,
        DisplayPrompt newActivePrompt = null, Collider triggerer = null)
    {
        if (triggerer) lastKnownTriggerer = triggerer;

        /*print($"<color=#0F0>{transform.parent.parent.name} Prompt change triggered. " +
            $"Appearing: {shouldAppear}. Is active prompt: {IsActivePrompt}. " +
            $"Prompt obj active: {promptObj.activeSelf}. " +
            $"Triggerer is \"{triggerer}\"</color>");*/

        //If we should appear but we're already appearing/appeared, no need to do anything.
        //  Preventing disappear redundancy is harder, has side effects, and *should* be unnecessary. Should.
        if (shouldAppear && IsActivePrompt && promptObj.activeSelf)
            return;

        //PromptStateChange?.Invoke(shouldAppear, triggerer, promptObj.activeSelf);

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
                        () =>
                        {
                            if (!triggerer) return;
                            promptObj.transform.position = triggerer.transform.position + offsetFromTriggerer;
                        },
                        () => !IsActivePrompt);
                    break;
            }

            promptCorout = StartCoroutine(PromptAppear());
            //Wait a little to let any false positives settle down before triggering a state change.
            if (appearBufferCorout == null)
            {
                appearBufferCorout = Coroutilities.DoAfterDelayFrames(this,
                        () =>
                        {
                            PromptStateChange?.Invoke(true, triggerer, promptObj.activeSelf);
                            appearBufferCorout = null;
                        },
                        3);
            }
        }
        else
        {
            Coroutilities.TryStopCoroutine(this, ref followCorout);
            Coroutilities.TryStopCoroutine(this, ref appearBufferCorout);

            promptCorout = StartCoroutine(PromptDisappear());
            PromptStateChange?.Invoke(false, triggerer, promptObj.activeSelf);
        }
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
