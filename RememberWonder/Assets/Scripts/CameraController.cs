using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] Transform verticalPivot;
    [SerializeField] Camera ownedCam;
    [Space(5)]
    [SerializeField] bool rotateOnMouseDown;
    [SerializeField] bool invertX;
    [SerializeField] bool invertY;
    [SerializeField] float lookSpeed;
    [SerializeField][VectorLabels("Min", "Max")] Vector2 pitchRange;
    [Space(5)]
    [SerializeField] float avoidRadius;
    [SerializeField] LayerMask avoidLayers = ~0;
    [SerializeField] float camToTargetSpeed = 0.01f;

    private float yaw;
    private float pitch;

    private Vector3 baseCamOffset;
    private float baseCamDistance;
    private Vector3 target;
    private Coroutine targetFollowCorout;

    private void Start()
    {
        if (ownedCam)
        {
            baseCamOffset = ownedCam.transform.localPosition;
            baseCamDistance = baseCamOffset.magnitude;
        }
        else Debug.LogWarning("Owned cam isn't assigned; camera will not zoom in and out near walls");
    }
    private void OnEnable()
    {
        targetFollowCorout = StartCoroutine(GoToTarget());
    }
    private void OnDisable()
    {
        StopCoroutine(targetFollowCorout);
    }

    void LateUpdate()
    {
        transform.position = player.transform.position;
        target = GetFarthestCamOffset();

        RotateCamera();
    }

    private IEnumerator GoToTarget()
    {
        while (true)
        {
            if (ownedCam)
                ownedCam.transform.localPosition = Vector3.MoveTowards(
                    ownedCam.transform.localPosition,
                    target,
                    camToTargetSpeed);

            yield return null;
        }
    }

    private Vector3 GetFarthestCamOffset()
    {
        if (ownedCam && Physics.SphereCast(
            transform.position,
            avoidRadius,
            ownedCam.transform.position - transform.position,
            out var hit,
            baseCamDistance,
            avoidLayers))
        {
            return new Vector3(baseCamOffset.x, baseCamOffset.y, -hit.distance);
        }

        return baseCamOffset;
    }

    private void RotateCamera()
    {
        //If we rotate on mouse down, and none of the mouse buttons are down, don't rotate.
        if (rotateOnMouseDown && !(Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2)))
            return;

        yaw += lookSpeed * Input.GetAxis("Mouse X") * Time.deltaTime;
        //Axis for yaw is multiplied by -1 if invertX is true
        transform.localRotation = Quaternion.AngleAxis(yaw, Vector3.up * (invertX ? -1 : 1));

        if (!verticalPivot) return;

        pitch += lookSpeed * Input.GetAxis("Mouse Y") * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, pitchRange.x, pitchRange.y);
        //Axis for pitch is multiplied by -1 if invertY is true
        verticalPivot.localRotation = Quaternion.AngleAxis(pitch, Vector3.left * (invertY ? -1 : 1));
    }
}
