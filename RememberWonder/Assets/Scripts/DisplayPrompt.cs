using Bewildered;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayPrompt : MonoBehaviour
{
    enum PromptPositionType { Default, TriggererStatic, TriggererFollow }

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

    public static DisplayPrompt activePromptDisplayer = null;
    private static Transform promptContainer = null;

    public PushPullObject GrabbablePromptOwner { get => grabbablePromptOwner; set => grabbablePromptOwner = value; }

    private void Start()
    {
        initScale = promptObj.transform.localScale;

        promptObj.transform.localScale = Vector3.zero;
        promptObj.SetActive(false);

        if (positionType != PromptPositionType.Default) return;

        if (!promptContainer) promptContainer = new GameObject("Prompt Container").transform;

        promptOffset = promptObj.transform.localPosition;
        ((RectTransform)promptObj.transform).SetParent(promptContainer.transform, false);

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
        //If this is the active prompt, but it's not showing and isn't grabbed, appear.
        if (activePromptDisplayer == this && !promptObj.activeSelf
            && grabbablePromptOwner && !grabbablePromptOwner.IsGrabbed)
        {
            TriggerPromptChange(true, false);
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
        if (setActivePrompt) activePromptDisplayer = newActivePrompt;

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

        yield return null;
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
    }
}
