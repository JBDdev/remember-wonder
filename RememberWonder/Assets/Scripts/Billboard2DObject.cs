using UnityEngine;

public class Billboard2DObject : MonoBehaviour
{
    [Tooltip("The camera this billboarded sprite will face. If null, uses Camera.main.")]
    [SerializeField] private Camera refCam;
    [Space(5)]
    [SerializeField] private bool lockXRotation;
    [SerializeField] private bool lockYRotation;
    [SerializeField] private bool lockZRotation;

    private void Start()
    {
        if (!refCam) refCam = Camera.main;
    }

    private void Update()
    {
        transform.rotation = refCam.transform.rotation;

        if (lockXRotation || lockYRotation || lockZRotation)
        {
            transform.rotation = Quaternion.Euler(
                lockXRotation ? 0 : transform.rotation.eulerAngles.x,
                lockYRotation ? 0 : transform.rotation.eulerAngles.y,
                lockZRotation ? 0 : transform.rotation.eulerAngles.z);
        }
    }
}