using Bewildered;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayPrompt : MonoBehaviour
{
    [SerializeField] private GameObject promptObj;
    [SerializeField] private UHashSet<TagString> activatorTags;
    [Space(5)]
    [Tooltip("In units per second. If 0.5, will take 2 seconds to reach a scale of 1.")]
    [SerializeField] private float appearSpeed;
    [SerializeField] private float disappearSpeed;
    [SerializeField] private float overshootAmount;
    [SerializeField] private float overshootBounceSpeed;

    private Vector3 initScale;
    private Coroutine promptCorout;

    public static DisplayPrompt activePromptDisplayer = null;

    private void Start()
    {
        initScale = promptObj.transform.localScale;
    }

    private void OnTriggerEnter(Collider other)
    {
        //If other's an activator, since it just entered, this must not be the active prompt; make this the active prompt and appear.
        if (activatorTags.Contains(other.tag))
        {
            activePromptDisplayer = this;
            Coroutilities.TryStopCoroutine(this, ref promptCorout);
            promptCorout = StartCoroutine(PromptAppear());
        }
    }
    private void OnTriggerStay(Collider other)
    {
        //If other's an activator and there's no other active prompt, make this the active prompt and appear.
        if (activatorTags.Contains(other.tag) && !activePromptDisplayer)
        {
            activePromptDisplayer = this;
            Coroutilities.TryStopCoroutine(this, ref promptCorout);
            promptCorout = StartCoroutine(PromptAppear());
        }
    }
    private void OnTriggerExit(Collider other)
    {
        //If other's an activator and we're the active prompt, disappear and make us not the active prompt.
        if (activatorTags.Contains(other.tag) && activePromptDisplayer == this)
        {
            activePromptDisplayer = null;
            Coroutilities.TryStopCoroutine(this, ref promptCorout);
            promptCorout = StartCoroutine(PromptDisappear());
        }
    }
    private void Update()
    {
        //If there is an active prompt, it's not this, and this prompt is showing, disappear.
        if (activePromptDisplayer && activePromptDisplayer != this && promptObj.activeSelf)
        {
            Coroutilities.TryStopCoroutine(this, ref promptCorout);
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
