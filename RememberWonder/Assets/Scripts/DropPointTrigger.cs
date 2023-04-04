using Bewildered;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropPointTrigger : MonoBehaviour
{
    bool invalidDropPosition;
    public bool InvalidDropPosition { get { return invalidDropPosition; } }

    [SerializeField] Renderer outlineRendCache;
    [SerializeField] Renderer arrowRendCache;
    [Space(5)]
    [SerializeField] UHashSet<TagString> ignoredTags;
    [SerializeField] UHashSet<Collider> collidingObjects;
    [Space(5)]
    [SerializeField] Color32 validPlacement = Color.white;
    [SerializeField] Color32 invalidPlacement = Color.white;
    [SerializeField] bool inheritAlpha = true;

    float outlineInitAlpha;
    float arrowInitAlpha;

    void Start()
    {
        collidingObjects = new();
        outlineInitAlpha = ((Color32)outlineRendCache.material.GetColor("_Color")).a;
        arrowInitAlpha = ((Color32)arrowRendCache.material.color).a;
    }

    void FixedUpdate()
    {
        if (collidingObjects.Count > 0)
            invalidDropPosition = true;
        else
            invalidDropPosition = false;

        if (invalidDropPosition)
        {
            outlineRendCache.material.SetColor("_Color", invalidPlacement.Adjust(3, (byte)(inheritAlpha
                ? outlineInitAlpha
                : invalidPlacement.a)));

            arrowRendCache.material.color = invalidPlacement.Adjust(3, (byte)(inheritAlpha
                ? arrowInitAlpha
                : invalidPlacement.a));
        }
        else
        {
            outlineRendCache.material.SetColor("_Color", validPlacement.Adjust(3, (byte)(inheritAlpha
                ? outlineInitAlpha
                : invalidPlacement.a)));

            arrowRendCache.material.color = validPlacement.Adjust(3, (byte)(inheritAlpha
                ? arrowInitAlpha
                : invalidPlacement.a));
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
