using Bewildered;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropPointTrigger : MonoBehaviour
{
    bool invalidDropPosition;
    public bool InvalidDropPosition { get { return invalidDropPosition; } }

    [SerializeField] UHashSet<TagString> ignoredTags;
    [SerializeField] UHashSet<Collider> collidingObjects;
    [SerializeField] Vector4 validPlacement;
    [SerializeField] Vector4 invalidPlacement;

    void Start()
    {
        collidingObjects = new();
    }

    void FixedUpdate()
    {
        if (collidingObjects.Count > 0)
            invalidDropPosition = true;
        else
            invalidDropPosition = false;

        if (invalidDropPosition)
        {
            transform.GetComponent<Renderer>().material.SetColor("_EmissionColor", invalidPlacement);
            transform.GetChild(0).GetComponent<Renderer>().material.color = invalidPlacement;
        }
        else
        {
            transform.GetComponent<Renderer>().material.SetColor("_EmissionColor", validPlacement);
            transform.GetChild(0).GetComponent<Renderer>().material.color = validPlacement;
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (ignoredTags.Contains(col.transform.tag)) return;

        collidingObjects.Add(col);
    }

    private void OnTriggerExit(Collider col)
    {
        if (ignoredTags.Contains(col.transform.tag)) return;

        collidingObjects.Remove(col);
    }
}
