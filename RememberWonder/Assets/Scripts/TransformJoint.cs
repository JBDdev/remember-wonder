using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformJoint : MonoBehaviour
{
    [System.Flags]
    public enum SetTiming
    {
        Never = 0,
        OnStart = 1,
        OnEnable = 2,
        OnDisable = 4,
    }

    public Rigidbody affectedRigidbody;
    public Transform connectedTransform;
    [Space(5)]
    public bool matchPosition;
    public bool matchRotation;
    public Vector3 positionOffset = Vector3.zero;
    public Quaternion rotationOffset = Quaternion.identity;
    [Space(5)]
    public SetTiming autoSetPositionOffset;
    public SetTiming autoSetRotationOffset;

    private void Start() => SetOffset(SetTiming.OnStart);
    private void OnEnable() => SetOffset(SetTiming.OnEnable);
    private void OnDisable() => SetOffset(SetTiming.OnDisable);

    private void SetOffset(SetTiming flagToCheck)
    {
        var positionReference = connectedTransform ? connectedTransform.position : Vector3.zero;
        var rotationReference = connectedTransform ? connectedTransform.rotation : Quaternion.identity;

        if (autoSetPositionOffset.HasFlag(flagToCheck))
            positionOffset = transform.position - positionReference;

        if (autoSetRotationOffset.HasFlag(flagToCheck))
            rotationOffset = Quaternion.RotateTowards(rotationReference, transform.rotation, Mathf.Infinity);
    }

    private void Update()
    {
        if (!connectedTransform) return;

        if (affectedRigidbody)
        {
            affectedRigidbody.MovePosition(connectedTransform.position + positionOffset);
            affectedRigidbody.MoveRotation(rotationOffset * connectedTransform.rotation);
        }
        else
        {
            transform.position = connectedTransform.position + positionOffset;
            transform.rotation = rotationOffset * connectedTransform.rotation;
        }
    }
}