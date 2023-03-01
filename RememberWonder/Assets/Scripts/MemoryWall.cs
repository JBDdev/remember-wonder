using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryWall : MonoBehaviour
{
    [SerializeField] private Renderer rend;
    [SerializeField] private GameObject promptObj;
    [Space(10)]
    [Tooltip("If 0, instead requires every mote in the scene. If -1, requires 1 less than that. Etc.")]
    [SerializeField] private int motesToUnlock;
    [SerializeField] private Bewildered.UHashSet<TagString> unlockerTags;

    private int internalMoteTracker;
    private int motesInScene;

    private void OnEnable()
    {
        CollectMote.MoteCollected += OnMoteCollected;
        CollectMote.MoteSpawned += OnMoteSpawned;
    }
    private void OnDisable()
    {
        CollectMote.MoteCollected -= OnMoteCollected;
        CollectMote.MoteSpawned -= OnMoteSpawned;
    }

    private void OnMoteCollected(CollectMote _) => internalMoteTracker++;
    private void OnMoteSpawned(CollectMote _, bool __) => motesInScene++;

    private void OnCollisionEnter(Collision collision)
    {
        if (!unlockerTags.Contains(collision.gameObject.tag)) return;

        //If negative, add to motes in scene; the absolute value is the desired leeway
        if (internalMoteTracker < (motesToUnlock > 0
            ? motesToUnlock
            : motesInScene + motesToUnlock))
        {
            LockedFlash();
        }
        else
        {
            //TODO: More elaborate sequence than just blipping away in a burst of particles?
            gameObject.SetActive(false);
            Destroy(promptObj);
        }
    }

    private void LockedFlash()
    {

    }
}
