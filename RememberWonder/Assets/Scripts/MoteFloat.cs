using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoteFloat : MonoBehaviour
{
    [SerializeField] private Transform model;
    [Space(5)]
    [SerializeField] private float rotateSpeed;
    [SerializeField] private Vector3 rotateAxis = Vector3.up;
    [Space(5)]
    [SerializeField] private float bobAmount;
    [SerializeField] private float bobSpeed;

    private Vector3 initPos;
    private Vector3 bobPos;
    private float bobProgress;

    private void Start()
    {
        initPos = model.position;
    }

    private void Update()
    {
        model.rotation *= Quaternion.AngleAxis(rotateSpeed * Time.deltaTime, rotateAxis);

        bobProgress += bobSpeed * Time.deltaTime;
        bobPos = initPos + Vector3.up * bobAmount * Mathf.Sin(bobProgress);
        model.position = bobPos;
    }
}
