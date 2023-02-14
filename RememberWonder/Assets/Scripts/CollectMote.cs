using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectMote : MonoBehaviour
{
    [SerializeField] private Renderer modelRend;
    [SerializeField] private ParticleSystem collectPSys;
    [SerializeField] private AudioList collectAudio;
    [Space(10)]
    [SerializeField] private Bewildered.UHashSet<TagString> collectorTags;
    private bool collected;

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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!collected && collectorTags.Contains(other.tag))
        {
            collected = true;

            //TODO: Become translucent and uncollectable, or collectable but without increasing number?
            modelRend.enabled = false;
            //TODO: More elaborate animation/sequence upon collection
            if (collectPSys) { collectPSys.Play(); }

            AudioHub.Inst.Play(collectAudio, transform.position);
            MoteCollected?.Invoke(this);
        }
    }
}