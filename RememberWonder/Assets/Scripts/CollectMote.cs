using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectMote : MonoBehaviour
{
    [SerializeField][TagSelector] private string[] collectorTags;

    /// <summary>
    /// Called when a mote is collected.<br/>
    /// Argument = the mote instance that was collected.
    /// </summary>
    public static Action<CollectMote> MoteCollected;

    private void OnTriggerEnter(Collider other)
    {
        if (Array.Exists(collectorTags, tag => other.CompareTag(tag)))
        {
            MoteCollected?.Invoke(this);
            //TODO: Become translucent and uncollectable, or collectable but without increasing number?
            Destroy(gameObject);
        }
    }
}