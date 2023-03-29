using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectMote : MonoBehaviour
{
    [SerializeField] private Renderer modelRend;
    [SerializeField] private GameObject shadowObj;
    [SerializeField] private ParticleSystem collectPSys;
    [Space(5)]
    [SerializeField] private Bewildered.UHashSet<TagString> collectorTags;
    [Header("Audio")]
    [SerializeField] private AudioList idleAudio;
    [SerializeField] private SourceSettings idleAudioSettings;
    [Space(5)]
    [SerializeField] private AudioList collectAudio;
    [SerializeField] private SourceSettings collectAudioSettings;
    private bool collected;

    private AudioSource idleAudioSource = null;

    /// <summary>
    /// Called when a mote is collected.
    /// <br/>- <see cref="CollectMote"/>: The mote instance that was spawned.
    /// <br/>- <see cref="bool"/>: Whether this mote has been collected yet.
    /// </summary>
    public static Action<CollectMote, bool> MoteSpawned;

    /// <summary>
    /// Called when a mote is collected.
    /// <br/>- <see cref="CollectMote"/>: The mote instance that was collected.
    /// </summary>
    public static Action<CollectMote> MoteCollected;

    private void Start()
    {
        //TODO: On scene startup, check saved data to see if this mote's been collected; maybe each mote has an ID?
        MoteSpawned?.Invoke(this, collected);

        if (!collected && idleAudio != AudioList.None)
        {
            Coroutilities.DoAfterDelayFrames(this,
                () => idleAudioSource = AudioHub.Inst.Play(idleAudio, idleAudioSettings, transform.position),
                1);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!collected && collectorTags.Contains(other.tag))
        {
            collected = true;

            //TODO: Become translucent and uncollectable, or collectable but without increasing number?
            modelRend.enabled = false;
            shadowObj.SafeSetActive(false);
            //TODO: More elaborate animation/sequence upon collection
            if (collectPSys) { collectPSys.Play(); }

            AudioHub.Inst.Play(collectAudio, collectAudioSettings, transform.position);
            if (idleAudioSource) idleAudioSource.Stop();

            MoteCollected?.Invoke(this);
        }
    }
}