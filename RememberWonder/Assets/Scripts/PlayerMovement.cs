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
    [SerializeField] bool grounded = true;
    bool groundedCheck;
    [SerializeField] public float maxIncline;

    [Header("Child Object References")]
    [SerializeField] GameObject holdLocation;

    [Header("External References")]
    [SerializeField] GameObject cameraFollower;
    [SerializeField] GameObject heldObject;

    //Internal Component References
    Rigidbody rb;
    CapsuleCollider col;

    //Accessors
    public GameObject HoldLocation { get { return holdLocation; } }

    // Start is called before the first frame update
    void Start()
    {
        //Get references to components on the GameObject
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();

        InputHub.Inst.Gameplay.Jump.performed += OnJumpPerformed;
        InputHub.Inst.Gameplay.Quit.performed += OnQuitPerformed;
    }
    private void OnDestroy()
    {
        InputHub.Inst.Gameplay.Jump.performed -= OnJumpPerformed;
        InputHub.Inst.Gameplay.Quit.performed -= OnQuitPerformed;
    }

    private void OnJumpPerformed(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        //print($"Jump performed, did we press or release?: " +
        //$"{(InputHub.Inst.Gameplay.Jump.WasPressedThisFrame() ? "Pressed" : "Released")}");

        if (!grounded)
            return;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(new Vector3(0f, jumpForce, 0f));
        usedJump = true;
    }

    private void OnQuitPerformed(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        Application.Quit();
    }

    void FixedUpdate()
    {
        //TODO: Only apply force when either move input is performed?
        //  (That's already the case, because direction's set to zero when no input, but still.)
        Vector3 direction = cameraFollower.transform.forward * InputHub.Inst.Gameplay.MoveY.ReadValue<float>();
        direction += cameraFollower.transform.right * InputHub.Inst.Gameplay.MoveX.ReadValue<float>();

        direction.Normalize();

        //Brody's Note to Self: The best way to acheive better velocity clamping would be to scale the force
        //  being applied by how close we are to maximum speed.

        // if(rb.velocity.sqrMagnitude < maxSpeed * maxSpeed)
        //     rb.AddForce(direction * accModifier, ForceMode.Acceleration);
        //transform.position += direction * maxSpeed * Time.deltaTime;

        rb.AddForce(direction * accModifier, ForceMode.Force);
        //Clamp the output velocity
        rb.velocity = new Vector3(
            Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed),
            rb.velocity.y,
            Mathf.Clamp(rb.velocity.z, -maxSpeed, maxSpeed));

        grounded = IsGrounded(col.height * transform.localScale.y, col.radius * transform.localScale.y);
    }

    private bool IsGrounded(float currentColHeight, float currentColRadius)
    {
        Vector3 point1 = transform.position + Vector3.up * currentColHeight / 2;
        point1 += Vector3.down * currentColRadius;

        Vector3 point2 = transform.position + Vector3.down * currentColHeight / 2;
        point2 += Vector3.up * currentColRadius;

        currentColRadius -= 0.02f;
        groundedCheck = Physics.CapsuleCast(
            point1, point2,
            currentColRadius, Vector3.down,
            out RaycastHit groundHit,
            0.1f);

        return groundedCheck && groundHit.normal.y >= maxIncline;
    }
}