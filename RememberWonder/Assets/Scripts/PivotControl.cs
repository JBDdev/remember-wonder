using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PivotControl : MonoBehaviour
{
    [SerializeField] bool clickAndDrag;
    [SerializeField] bool invertX;
    [SerializeField] bool invertY;
    [SerializeField] float lookSpeed;
    [SerializeField][VectorLabels("Min", "Max")] Vector2 pitchRange;

    private float yaw;
    private float pitch;
    private Quaternion targetRotation;

    void Update()
    {
        //If we rotate on mouse down, and none of the mouse buttons are down, don't rotate.
        if (clickAndDrag && !InputHub.Inst.Gameplay.LookActive.IsPressed())
            return;

        yaw += lookSpeed * InputHub.Inst.Gameplay.LookX.ReadValue<float>() * Time.deltaTime;
        //Axis for yaw is multiplied by -1 if invertX is true
        targetRotation = Quaternion.AngleAxis(yaw, Vector3.up * (invertX ? -1 : 1));

        pitch += lookSpeed * InputHub.Inst.Gameplay.LookY.ReadValue<float>() * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, pitchRange.x, pitchRange.y);
        //Axis for pitch is multiplied by -1 if invertY is true
        targetRotation *= Quaternion.AngleAxis(pitch, Vector3.left * (invertY ? -1 : 1));

        transform.localRotation = targetRotation;
    }
}
