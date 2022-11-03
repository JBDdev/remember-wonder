using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] float lookSpeed;

    // Start is called before the first frame update
    void LateUpdate() 
    {
        transform.position = player.transform.position;

        //Debug.Log(Input.GetAxis("Mouse X"));
        float deltaRotation = lookSpeed * Input.GetAxis("Mouse X") * Time.deltaTime;
        transform.Rotate(0, deltaRotation, 0);

    }
}
