using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Movement Variables")]
    [SerializeField] float maxSpeed;
    [SerializeField] float accModifier;
    public bool pullingObject = false;
#if UNITY_EDITOR
    [SerializeField][ReadOnlyInspector] private PushPullObject _PulledObjPreview = null;
#endif
    [Space(5)]
    [SerializeField] Vector3 directionLastFrame;
    [SerializeField] float dirChangeThreshold = 0.01f;
#if UNITY_EDITOR
    [Space(5)]
    [SerializeField] bool visualizeMoveInput;
#endif

    [Header("Jump Controls")]
    [SerializeField] float jumpForce;
    public bool jumpInProgress = false;
    public float maxIncline;
    public float fallGravMultiplier = 1;
    [SerializeField] private LayerMask groundLayers = ~0;

    [Header("Self Component References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private CapsuleCollider primaryCol;
    [SerializeField] private CapsuleCollider secondaryCol;

    [Header("Child Object References")]
    [SerializeField] Transform pickUpPivot;
    [SerializeField] GameObject characterModel;
    [SerializeField] DropPointTrigger dropLocation;
    [SerializeField] GameObject cameraPivot;
    [SerializeField] Transform shadow;
    [SerializeField] float shadowFloorOffset;
    [SerializeField] LayerMask shadowLayerMask = ~0;

    [Header("External References")]
    [SerializeField] GameObject heldObject;
    [SerializeField] PushPullObject pushPullObject;
    [SerializeField] Animator anim;

    [Header("Rotation Controls")]
    [SerializeField] float rotationSpeed;
    [SerializeField] float minRotationDistance;

    [Header("Player SFX")]
    [SerializeField] AudioList landingSFX;
    [SerializeField] SourceSettings landingSettings;

    bool paused;
    bool landed = false;

    /// <summary>
    /// Invoked whenever this we start or stop grabbing something..
    /// <br/>- <see cref="bool"/>: True if we just grabbed something. False if just stopped.
    /// </summary>
    public static Action<bool> GrabStateChange;

    //Accessors
    public Transform PickUpPivot { get { return pickUpPivot; } }
    public GameObject CharacterModel { get { return characterModel; } }
    public DropPointTrigger DropLocation { get { return dropLocation; } }

    public PushPullObject PulledObject { get { return pushPullObject; } set { pushPullObject = value; } }
    public Vector3 Velocity { get => rb.velocity; }
    public Collider PrimaryCollider { get => primaryCol; }
    public Collider SecondaryCollider { get => secondaryCol; }


    void Start()
    {
        //Get references to components on the GameObject
        anim = transform.GetComponentInChildren<Animator>();

        paused = false;

        PulledObject = null;

        InputHub.Inst.Gameplay.Jump.performed += OnJumpPerformed;
        InputHub.Inst.Gameplay.Grab.performed += OnInteractPerformed;
    }
    private void OnDestroy()
    {
        InputHub.Inst.Gameplay.Jump.performed -= OnJumpPerformed;
        InputHub.Inst.Gameplay.Grab.performed -= OnInteractPerformed;
    }

    //---Input Events---//

    private void OnJumpPerformed(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        //print($"Jump performed, did we press or release?: " +
        //$"{(InputHub.Inst.Gameplay.Jump.WasPressedThisFrame() ? "Pressed" : "Released")}");

        if (!IsGrounded() || jumpInProgress)
            return;

        if (pullingObject && !PulledObject.liftable)
            return;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(new Vector3(0f, jumpForce, 0f));
        jumpInProgress = true;
        anim.SetBool("Jumped", true);
    }

    public void TogglePause()
    {
        if (paused)
        {
            paused = false;
            InputHub.Inst.Gameplay.Jump.performed += OnJumpPerformed;
            InputHub.Inst.Gameplay.Grab.performed += OnInteractPerformed;
            if (IsGrounded())
            {
                jumpInProgress = false;
            }
            rb.isKinematic = false;
            rb.freezeRotation = true;
        }
        else
        {
            paused = true;
            InputHub.Inst.Gameplay.Jump.performed -= OnJumpPerformed;
            InputHub.Inst.Gameplay.Grab.performed -= OnInteractPerformed;
            rb.isKinematic = true;
            rb.freezeRotation = false;
        }
    }

    private void OnInteractPerformed(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        //If a PulledObject hasn't been registered, don't do anything. (See PushPullObject)
        if (!PulledObject) return;

        if (!pullingObject)
        {
            pullingObject = true;

            if (PulledObject.liftable)
            {
                dropLocation.gameObject.SetActive(true);
            }
            else
            {
                rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
            }

            //If X or Z are not allowed, make ALL force unable to move the player in that axis
            if (PulledObject.GrabMoveMultipliers.x <= 0)
                rb.constraints |= RigidbodyConstraints.FreezePositionX;
            if (PulledObject.GrabMoveMultipliers.z <= 0)
                rb.constraints |= RigidbodyConstraints.FreezePositionZ;
        }
        else
        {
            if (PulledObject.liftable && dropLocation.InvalidDropPosition)
                return;

            pullingObject = false;
            rb.constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezeRotation;
        }

        GrabStateChange?.Invoke(pullingObject);
    }

    //---Core Methods---//

    private void Update()
    {
#if UNITY_EDITOR
        _PulledObjPreview = PulledObject;
#endif

        //Update Drop Shadow 
        RaycastHit hit;
        if (shadow && Physics.Raycast(
            transform.position.Adjust(1, -0.5f, true),
            Vector3.down,
            out hit,
            Mathf.Infinity,
            shadowLayerMask,
            QueryTriggerInteraction.Ignore))
        {
            //float yPos = hit.collider.bounds.center.y + hit.collider.bounds.extents.y;
            shadow.position = new Vector3(hit.point.x, hit.point.y + shadowFloorOffset, hit.point.z);
        }
    }
    void FixedUpdate()
    {
        var grounded = IsGrounded();

        if (!grounded) landed = false;

        anim.SetBool("Jumped", jumpInProgress);
        if (grounded && !landed && rb.velocity.y < 0)
        {
            landed = true;
            AudioHub.Inst.Play(landingSFX, landingSettings, transform.position);
        }

        //Debug.Log(anim.GetAnimatorTransitionInfo(0).IsUserName("Landing"));

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

        direction = Quaternion.LookRotation(Vector3.Cross(cameraPivot.transform.right, Vector3.up)) * direction.SwapAxes(1, 2);

        anim.SetFloat("Walk Speed", direction.sqrMagnitude);

        if (direction.sqrMagnitude > minRotationDistance * minRotationDistance)
            RotateCharacterModel(direction.normalized);

        ApplyPullRestrictions(ref direction, grounded);
        if (direction == Vector3.zero) return;

        float percentHeld = direction.magnitude;

        //If we are not grounded, inputting significantly, and in a significantly different direction (axis delta > deadzone),
        if (!grounded
            && direction.sqrMagnitude > minRotationDistance * minRotationDistance
            && (Mathf.Abs(directionLastFrame.x - direction.x) > dirChangeThreshold
            || Mathf.Abs(directionLastFrame.z - direction.z) > dirChangeThreshold))
        {
            //Make velocity point in the direction of input; input direction with the XZ magnitude of velocity, with the Y component of velocity
            rb.velocity = (direction * Vector3.ProjectOnPlane(rb.velocity, Vector3.up).magnitude).Adjust(1, rb.velocity.y);
        }

        //If both axes are under max speed, apply force in direction.
        if (Mathf.Abs(rb.velocity.x) < maxSpeed * percentHeld && Mathf.Abs(rb.velocity.z) < maxSpeed * percentHeld)
        {
            rb.AddForce(direction * accModifier, ForceMode.Force);
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
            0.1f,
            groundLayers,
            QueryTriggerInteraction.Ignore);

        if (jumpInProgress && groundedCheck)
        {
            Coroutine c = Coroutilities.DoNextFrame(this, () => jumpInProgress = rb.velocity.y > 0);
        }
        return groundedCheck && groundHit.normal.y >= maxIncline;
    }

    private void ApplyPullRestrictions(ref Vector3 restrictedDir, bool grounded)
    {
        //If not pulling, unrestrict all move axes and bail out
        if (!pullingObject)
        {
            rb.constraints &= ~(RigidbodyConstraints.FreezePosition);
            return;
        }
        //Otherwise, if this object isn't liftable, we shouldn't be able to pull it if we're not grounded
        //  We can bail out ONLY because we've already zeroed everything
        if (!grounded && !PulledObject.liftable)
        {
            ZeroDirAndVelocity(ref restrictedDir, 3);
            return;
        }

        restrictedDir.x *= PulledObject.GrabMoveMultipliers.x;
        restrictedDir.z *= PulledObject.GrabMoveMultipliers.z;

        if (BeyondMaxDistance(restrictedDir, 0))    //X is beyond max distance
            ZeroDirAndVelocity(ref restrictedDir, 0);

        if (BeyondMaxDistance(restrictedDir, 2))    //Z is beyond max distance
            ZeroDirAndVelocity(ref restrictedDir, 2);
    }
    #region Restriction Helper Functions
    /// <param name="axis">012, XYZ. Input any other int to zero out ALL axes.</param>
    private void ZeroDirAndVelocity(ref Vector3 dir, int axis)
    {
        switch (axis)
        {
            case 0: dir.x = 0f; break;
            case 1: dir.y = 0f; break;
            case 2: dir.z = 0f; break;
            default:
                dir = Vector3.zero;
                break;
        }

        rb.velocity = Vector3.zero;
    }
    /// <param name="axis">012, XYZ. Clamped using <see cref="Mathf.Clamp(int, int, int)"/>.</param>
    private bool BeyondMaxDistance(Vector3 direction, int axis)
    {
        axis = Mathf.Clamp(axis, 0, 2);

        return
            //Beyond negative max
            (direction[axis] < 0 && PulledObject.transform.position[axis] < PulledObject.defaultPos[axis] - PulledObject.maxPullDistance)
            //Beyond positive max
            || (direction[axis] > 0 && PulledObject.transform.position[axis] > PulledObject.defaultPos[axis] + PulledObject.maxPullDistance);
    }
    #endregion

    private void RotateCharacterModel(Vector3 direction)
    {
        if (direction.sqrMagnitude <= Mathf.Epsilon) return;
        characterModel.transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
    }

    //---Helper Methods---//

    public void GetCapsuleCastParams(out float height, out float radius, out Vector3 top, out Vector3 bottom)
    {
        height = primaryCol.height * transform.localScale.y;
        radius = primaryCol.radius * transform.localScale.y;

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