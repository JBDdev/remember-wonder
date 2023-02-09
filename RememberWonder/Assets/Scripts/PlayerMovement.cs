using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Movement Variables")]
    [SerializeField] float maxSpeed;
    [SerializeField] float accModifier;
    public bool pullingObject = false;
    [Space(5)]
    [SerializeField] Vector3 directionLastFrame;
    [SerializeField] float dirChangeThreshold = 0.01f;
    [SerializeField][Range(0, 1)] float airFriction = 1f;
#if UNITY_EDITOR
    [Space(5)]
    [SerializeField] bool visualizeMoveInput;
#endif

    [Header("Jump Controls")]
    [SerializeField] float jumpForce;
    public bool jumpInProgress = false;
    public float maxIncline;
    public float fallGravMultiplier = 1;

    [Header("Child Object References")]
    [SerializeField] GameObject holdLocation;
    [SerializeField] GameObject characterModel;

    [Header("External References")]
    [SerializeField] GameObject cameraPivot;
    [SerializeField] GameObject heldObject;

    //Internal Component References
    Rigidbody rb;
    CapsuleCollider col;

    [Header("Rotation Controls")]
    [SerializeField] float rotationSpeed;
    [SerializeField] float minRotationDistance;

    //Accessors
    public GameObject HoldLocation { get { return holdLocation; } }
    public PushPullObject PulledObject { get; set; }
    public Vector3 Velocity { get => rb.velocity; }

    void Start()
    {
        //Get references to components on the GameObject
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();

        PulledObject = null;

        InputHub.Inst.Gameplay.Jump.performed += OnJumpPerformed;
        InputHub.Inst.Gameplay.Quit.performed += OnQuitPerformed;
        InputHub.Inst.Gameplay.Grab.performed += OnInteractPerformed;
    }
    private void OnDestroy()
    {
        InputHub.Inst.Gameplay.Jump.performed -= OnJumpPerformed;
        InputHub.Inst.Gameplay.Quit.performed -= OnQuitPerformed;
        InputHub.Inst.Gameplay.Grab.performed -= OnInteractPerformed;
    }

    //---Input Events---//

    private void OnJumpPerformed(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        //print($"Jump performed, did we press or release?: " +
        //$"{(InputHub.Inst.Gameplay.Jump.WasPressedThisFrame() ? "Pressed" : "Released")}");

        if (!IsGrounded())
            return;

        if (pullingObject && !PulledObject.liftable)
            return;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(new Vector3(0f, jumpForce, 0f));
        jumpInProgress = true;
    }

    private void OnQuitPerformed(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        Application.Quit();
    }

    private void OnInteractPerformed(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (!IsGrounded() || !PulledObject)
            return;

        if (!pullingObject)
        {
            pullingObject = true;
            if (!PulledObject.liftable)
                rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        }
        else
        {
            pullingObject = false;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    //---Core Methods---//

    void FixedUpdate()
    {
        var grounded = IsGrounded();

        ApplyMoveForce(grounded);

        //If NOT grounded, fall gravity should be modified, and we're falling (not rising),
        if (!grounded && !Mathf.Approximately(fallGravMultiplier, 1) && rb.velocity.y < 0)
        {
            //Apply extra force based on the multiplier (There's no "gravity scale" for 3D Rigidbodies).
            //Gravity's already applied once by default; if 1.01, apply the extra 0.01
            rb.AddForce(Physics.gravity * (fallGravMultiplier - 1f), ForceMode.Acceleration);
        }
    }

    private void ApplyMoveForce(bool grounded)
    {
        Vector3 direction = InputHub.Inst.Gameplay.Move.ReadValue<Vector2>();
        float percentHeld = direction.magnitude;

        direction =
            Quaternion.LookRotation(Vector3.Cross(cameraPivot.transform.right, Vector3.up))
            * direction.SwapAxes(1, 2);

        ApplyPullRestrictions(ref direction);
        if (direction == Vector3.zero) return;

        //If we are not grounded and moving in a significantly different direction (axis delta > deadzone),
        if (!IsGrounded()
            && (Mathf.Abs(directionLastFrame.x - direction.x) > dirChangeThreshold
            || Mathf.Abs(directionLastFrame.z - direction.z) > dirChangeThreshold))
        {
            rb.velocity = new Vector3(rb.velocity.x * airFriction, rb.velocity.y, rb.velocity.z * airFriction);
        }

        //If both axes are under max speed, apply force in direction.
        if (Mathf.Abs(rb.velocity.x) < maxSpeed * percentHeld && Mathf.Abs(rb.velocity.z) < maxSpeed * percentHeld)
        {
            rb.AddForce(direction * accModifier, ForceMode.Force);

            if (rb.velocity.sqrMagnitude > minRotationDistance)
            {
                RotateCharacterModel(rb.velocity);
            }
        }

        directionLastFrame = direction;
        DrawDebugMovementRays(direction);
    }

    public bool IsGrounded()
    {
        GetCapsuleCastParams(out _, out float radius, out Vector3 point1, out Vector3 point2);

        radius -= 0.02f;
        bool groundedCheck = Physics.CapsuleCast(
            point1, point2,
            radius, Vector3.down,
            out RaycastHit groundHit,
            0.1f);

        if (jumpInProgress && groundedCheck)
        {
            Coroutilities.DoNextFrame(this, () => jumpInProgress = rb.velocity.y > 0);
        }
        return groundedCheck && groundHit.normal.y >= maxIncline;
    }

    private void ApplyPullRestrictions(ref Vector3 restrictedDir)
    {
        if (!pullingObject) return;

        // Restrict axis pulling on certain objects
        if (!PulledObject.usableAxes.Contains("z"))
            restrictedDir.z = 0f;
        if (!PulledObject.usableAxes.Contains("x"))
            restrictedDir.x = 0f;

        // Restrict axis movement via max pull distance

        //X is beyond the negative max distance
        if (restrictedDir.x < 0
            && PulledObject.transform.position.x < PulledObject.defaultPos.x - PulledObject.maxPullDistance)
        {
            restrictedDir.x = 0f;
            rb.velocity = Vector3.zero;
        }
        //X is beyond the positive max distance
        else if (restrictedDir.x > 0
            && PulledObject.transform.position.x > PulledObject.defaultPos.x + PulledObject.maxPullDistance)
        {
            restrictedDir.x = 0f;
            rb.velocity = Vector3.zero;
        }
        //Z is beyond the negative max distance
        if (restrictedDir.z < 0
            && PulledObject.transform.position.z < PulledObject.defaultPos.z - PulledObject.maxPullDistance)
        {
            restrictedDir.z = 0f;
            rb.velocity = Vector3.zero;
        }
        //Z is beyond the positive max distance
        else if (restrictedDir.z > 0
            && PulledObject.transform.position.z > PulledObject.defaultPos.z + PulledObject.maxPullDistance)
        {
            restrictedDir.z = 0f;
            rb.velocity = Vector3.zero;
        }
    }

    private void RotateCharacterModel(Vector3 direction)
    {
        if (direction == Vector3.zero) return;
        characterModel.transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
    }

    //---Helper Methods---//

    public void GetCapsuleCastParams(out float height, out float radius, out Vector3 top, out Vector3 bottom)
    {
        height = col.height * transform.localScale.y;
        radius = col.radius * transform.localScale.y;

        top = transform.position + Vector3.up * height / 2;
        top += Vector3.down * radius;   //Go from tip to center of cap-sphere

        bottom = transform.position + Vector3.down * height / 2;
        bottom += Vector3.up * radius;
    }

    private void DrawDebugMovementRays(Vector3 direction)
    {
#if UNITY_EDITOR
        if (!visualizeMoveInput) return;

        var lightGrey = new Color(0.75f, 0.75f, 0.75f, 0.8f);
        Debug.DrawRay(transform.position, Vector3.Cross(cameraPivot.transform.right, Vector3.up) * 2.5f, lightGrey.Adjust(2, 1));
        Debug.DrawRay(transform.position, cameraPivot.transform.right * 2.5f, lightGrey.Adjust(0, 1));

        Debug.DrawRay(transform.position, rb.velocity, Color.yellow.Adjust(3, 0.6f));
        Debug.DrawRay(transform.position, rb.velocity - Vector3.up * rb.velocity.y, Color.yellow);

        Debug.DrawRay(transform.position, direction, Color.white);
        UtilFunctions.DrawSphere(transform.position + direction, 0.15f, 6, 6, Color.white);
#endif
    }
}