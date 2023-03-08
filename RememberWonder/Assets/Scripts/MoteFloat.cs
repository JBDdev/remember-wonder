using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoteFloat : MonoBehaviour
{
    [SerializeField] private Transform model;
    [SerializeField] private Transform shadow;
    [SerializeField] private float shadowFloorOffset;
    [Space(5)]
    [SerializeField] private float rotateSpeed;
    [SerializeField] private Vector3 rotateAxis = Vector3.up;
    [SerializeField] private Vector3 shadowRotateAxis = Vector3.forward;
    [Space(5)]
    [SerializeField] private float bobAmount;
    [SerializeField] private float bobSpeed;

    private Vector3 initPos;
    private Vector3 bobPos;
    private float bobProgress;

    private void Start()
    {
        initPos = model.localPosition;

        //Raycast to place mote shadow on floor
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
            shadow.position = new Vector3(hit.point.x, hit.point.y + shadowFloorOffset, hit.point.z);
    }

    private void Update()
    {
        model.rotation *= Quaternion.AngleAxis(rotateSpeed * Time.deltaTime, rotateAxis);
        shadow.rotation *= Quaternion.AngleAxis(rotateSpeed * Time.deltaTime, shadowRotateAxis);

        bobProgress += bobSpeed * Time.deltaTime;
        bobPos = initPos + Vector3.up * bobAmount * Mathf.Sin(bobProgress);
        model.localPosition = bobPos;
    }
}
