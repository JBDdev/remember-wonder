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
    [SerializeField] BoxCollider colliderRef;
    [SerializeField] PlayerMovement player;
    [Space(5)]
    [SerializeField][Range(0, 5)] float maxRaiseDistance;
    [SerializeField] float raiseYOffset = 0.01f;
    [SerializeField] LayerMask ignoredLayers = 0;
    [SerializeField] UHashSet<TagString> ignoredTags;
    [SerializeField] UHashSet<Collider> collidingObjects;
    [Space(5)]
    [SerializeField] Color32 validPlacement = Color.white;
    [SerializeField] Color32 invalidPlacement = Color.white;
    [SerializeField] bool inheritAlpha = true;

    float outlineInitAlpha;
    float arrowInitAlpha;
    Vector3 initLocalPos;

    Vector3 maxRaisePoint;
    Vector3 colliderHalfExtents;
    RaycastHit[] raiseCheckHitBuffer = new RaycastHit[10];

    void Start()
    {
        collidingObjects = new();
        outlineInitAlpha = ((Color32)outlineRendCache.material.GetColor("_Color")).a;
        arrowInitAlpha = ((Color32)arrowRendCache.material.color).a;
        initLocalPos = transform.localPosition;

        colliderHalfExtents = UtilFunctions.InverseScale(colliderRef.size, transform.lossyScale) / 2;
    }

    private void Update()
    {
        //Reset position of the drop indicator.
        transform.localPosition = initLocalPos;

        //Cast a box from the max raise point down to the default drop point.
        maxRaisePoint = transform.position.Adjust(1, maxRaiseDistance, true);
        int hitCount = Physics.BoxCastNonAlloc(
            maxRaisePoint,
            colliderHalfExtents,
            Vector3.down,
            raiseCheckHitBuffer,
            transform.rotation,
            maxRaisePoint.y - (transform.TransformPoint(initLocalPos).y - colliderHalfExtents.y),
            ~ignoredLayers,
            QueryTriggerInteraction.Ignore);

        //Go through every hit,
        for (int i = 0; i < hitCount; i++)
        {
            //And if it's not an ignored tag, it's not the currently lifted object, and isn't a bogus on-startup
            //hit (see https://docs.unity3d.com/ScriptReference/Physics.BoxCastAll.html),
            if (!ignoredTags.Contains(raiseCheckHitBuffer[i].collider.tag)
                && player.PulledObject.gameObject != raiseCheckHitBuffer[i].collider.gameObject
                && !IsBogusHit(raiseCheckHitBuffer[i]))
            {
                //Move the drop point indicator up to that hit's y location.
                transform.position = transform.position.Adjust(1, raiseCheckHitBuffer[i].point.y + colliderHalfExtents.y + raiseYOffset);
                break;
            }
        }
    }
    private bool IsBogusHit(RaycastHit hit) => hit.point == Vector3.zero && hit.distance == 0;

    void FixedUpdate()
    {
        invalidDropPosition = collidingObjects.Count > 0;

        if (invalidDropPosition)
            SetColors(invalidPlacement);
        else
            SetColors(validPlacement);
    }

    private void SetColors(Color32 newColor)
    {
        //The manual cast to byte is needed because otherwise, it will be read as an int and
        //then implicitly cast to a float, thus making the compiler think we're targetting a method
        //than the one we want (by implicitly casting the color32 to a normal color, instead of
        //just leaving the color alone and casting the int to a byte >:c)
        outlineRendCache.material.SetColor("_Color", newColor.Adjust(3, (byte)(inheritAlpha
            ? outlineInitAlpha
            : invalidPlacement.a)));

        arrowRendCache.material.color = newColor.Adjust(3, (byte)(inheritAlpha
            ? arrowInitAlpha
            : invalidPlacement.a));
    }

    private void OnTriggerEnter(Collider col)
    {
        if (ignoredTags.Contains(col.transform.tag) || ignoredLayers.Includes(col.gameObject.layer)) return;

        collidingObjects.Add(col);
    }

    private void OnTriggerExit(Collider col)
    {
        if (ignoredTags.Contains(col.transform.tag) || ignoredLayers.Includes(col.gameObject.layer)) return;

        collidingObjects.Remove(col);
    }
}
