using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PushPullObject : MonoBehaviour
{
    [SerializeField][ReadOnlyInspector] private PlayerMovement player;
    [SerializeField][ReadOnlyInspector] private bool grabbed;
    [ReadOnlyInspector] public Vector3 defaultPos;
    [SerializeField] private DisplayPrompt grabPrompt;
    public bool liftable;
    [SerializeField] private Vector3 grabMoveMultipliers = Vector3.one;
    public float maxPullDistance;
    [SerializeField] PhysicMaterial physMat;
    [Header("Audio")]
    [SerializeField] private AudioList liftAudio;
    [SerializeField] private SourceSettings liftAudioSettings;
    [Space(5)]
    [SerializeField] private AudioList putDownAudio;
    [SerializeField] private SourceSettings putDownAudioSettings;

    Rigidbody rb;

    string initTag;    //QUICK AND DIRTY FIX for camera collision, Delete later?
    Transform parentBeforeGrab;
    float initMass;

    public Vector3 GrabMoveMultipliers { get => grabMoveMultipliers; }
    public bool IsGrabbed { get => grabbed; }

    private void OnEnable()
    {
        if (grabPrompt) grabPrompt.PromptStateChange += OnGrabPromptStateChange;
        else Debug.LogWarning($"\"{name}\" does not have a grab prompt reference; it won't be able to be grabbed.");
    }
    private void OnDisable()
    {
        if (grabPrompt) grabPrompt.PromptStateChange -= OnGrabPromptStateChange;
    }

    void Start()
    {
        grabbed = false;
        defaultPos = transform.position;
        initTag = tag;

        rb = GetComponent<Rigidbody>();
        if (rb) initMass = rb.mass;
    }

    void OnInteractPerformedWhileRegistered(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        //If we don't have a player reference, bail out.
        //If this isn't liftable and the player's not grounded, bail out.
        if (!player || (!liftable && !player.IsGrounded())) return;

        if (liftable && player.DropLocation.InvalidDropPosition) return;

        grabbed = !grabbed;

        if (liftable)
        {
            if (grabbed)
            {
                Destroy(rb);
                transform.rotation = player.CharacterModel.transform.rotation;
                foreach (BoxCollider b in transform.GetComponents<BoxCollider>())
                    b.material = physMat;
            }

            else
            {
                if (player.DropLocation.InvalidDropPosition)
                    return;

                rb = transform.gameObject.AddComponent<Rigidbody>();
                rb.mass = initMass;
                foreach (BoxCollider b in transform.GetComponents<BoxCollider>())
                    b.material = null;
            }
        }

        if (grabbed)
        {
            if (liftable)
            {
                transform.position = player.PickUpPivot.position;

                parentBeforeGrab = transform.parent;
                transform.parent = player.PickUpPivot;

                AudioHub.Inst.Play(liftAudio, liftAudioSettings, transform.position);
            }
            else
            {
                parentBeforeGrab = transform.parent;
                transform.parent = player.transform;
            }
        }
        else
        {
            transform.parent = parentBeforeGrab;

            player.DropLocation.gameObject.SetActive(false);
            if (liftable)
            {
                AudioHub.Inst.Play(putDownAudio, putDownAudioSettings, transform.position);
                transform.position = player.DropLocation.transform.position;
            }
        }
    }

    private void OnGrabPromptStateChange(bool appearing, Collider changeTriggerer, bool currentActive)
    {
        if (appearing && changeTriggerer)
        {
            Register(changeTriggerer);
        }
        else if (!appearing)
        {
            Deregister();
        }
    }

    private void Register(Collider registeredCollider)
    {
        //Try to get a reference to the player from the collider. Only move on if we succeed.
        //  If we already have a player ref, no need to re-register.
        if (player || !registeredCollider.TryGetComponent(out player)) return;

        /*//If the player's already grabbing an object, abort registration (but don't nullify player? why not?).
        if (player.PulledObject || player.pullingObject || !player.IsGrounded()) return;*/

        player.PulledObject = this;

        // !!! QUICK AND DIRTY FIX for camera collision; delete later? !!!
        tag = "Player";

        InputHub.Inst.Gameplay.Grab.performed += OnInteractPerformedWhileRegistered;
    }

    private void Deregister()
    {
        //If we don't have a reference to the player, we don't need to "deregister" anything.
        if (!player || player.pullingObject) return;

        InputHub.Inst.Gameplay.Grab.performed -= OnInteractPerformedWhileRegistered;

        player.PulledObject = null;
        player = null;

        //!!! QUICK AND DIRTY FIX for camera collision; delete later? !!!
        tag = initTag;
    }
}
