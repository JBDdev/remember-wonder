using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Movement Variables")]
    [SerializeField] float maxSpeed;
    [SerializeField] float accModifier;

    [Header("Jump Controls")]
    [SerializeField] float jumpForce;
    [SerializeField] public bool usedJump = false;
    [SerializeField] public float maxIncline;

    [Header("Child Object References")]
    [SerializeField] GameObject holdLocation;

    [Header("External References")]
    [SerializeField] GameObject cameraFollower;
    [SerializeField] GameObject heldObject;

    //Internal Component References
    Rigidbody rb;

    //Accessors
    public GameObject HoldLocation {get {return holdLocation;}}

    // Start is called before the first frame update
    void Start()
    {
        //Get references to components on the GameObject
        rb = GetComponent<Rigidbody>();

        InputHub.Inst.Gameplay.Jump.performed += OnJumpPerformed;
    }
    private void OnDestroy()
    {
        InputHub.Inst.Gameplay.Jump.performed -= OnJumpPerformed;
    }

    private void OnJumpPerformed(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        print($"Jump performed, did we press or release?: " +
            $"{(InputHub.Inst.Gameplay.Jump.WasPressedThisFrame() ? "Pressed" : "Released")}");

        if (usedJump)
            return;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(new Vector3(0f, jumpForce, 0f));
        usedJump = true;
    }

    void Update()
    {

    }

    void FixedUpdate()
    {
        Vector3 direction = cameraFollower.transform.forward * InputHub.Inst.Gameplay.MoveY.ReadValue<float>();
        direction += cameraFollower.transform.right * InputHub.Inst.Gameplay.MoveX.ReadValue<float>();

        direction.Normalize();


        //Brody's Note to Self: The best way to acheive better velocity clamping would be to scale the force being applied by how close we are to maximum speed.

        // if(rb.velocity.sqrMagnitude < maxSpeed * maxSpeed)
        //     rb.AddForce(direction * accModifier, ForceMode.Acceleration);
        //transform.position += direction * maxSpeed * Time.deltaTime;
 
        rb.AddForce(direction * accModifier, ForceMode.Force);
        //Clamp the output velocity
        rb.velocity = new Vector3(Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed), rb.velocity.y, Mathf.Clamp(rb.velocity.z, -maxSpeed, maxSpeed));
    }

    void OnCollisionEnter(Collision col)
    {
        //Debug.Log("test");
        if (col.GetContact(0).normal.y >= maxIncline)
        {
            //Debug.Log(col.GetContact(0).normal.y);
            usedJump = false;
        }

    }

    void OnCollisionStay(Collision col) 
    {
        if (col.GetContact(0).normal.y >= maxIncline)
        {
            //Debug.Log(col.GetContact(0).normal.y);
            usedJump = false;
        }
    }


}
