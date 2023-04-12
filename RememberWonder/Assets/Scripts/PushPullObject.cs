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
    [SerializeField] private Bewildered.UHashSet<DisplayPrompt> grabPrompts;
    public bool liftable;
    [SerializeField][NaughtyAttributes.ShowIf("liftable")] private TagString liftedTag;
    [SerializeField][NaughtyAttributes.ShowIf("liftable")] PhysicMaterial physMat;
    [NaughtyAttributes.HideIf("liftable")] public float maxPullDistance;
    [SerializeField] private Vector3 grabMoveMultipliers = Vector3.one;
    [Header("Audio")]
    [SerializeField] private AudioList liftAudio;
    [SerializeField] private SourceSettings liftAudioSettings;
    [Space(5)]
    [SerializeField] private AudioList putDownAudio;
    [SerializeField] private SourceSettings putDownAudioSettings;
#if UNITY_EDITOR
    [Header("EDITOR ONLY")]
    [SerializeField] bool printRegistrationResults;
#endif

    Rigidbody rb;

    string initTag;    //QUICK AND DIRTY FIX for camera collision, Delete later?
    Transform parentBeforeGrab;
    float initMass;

    public Vector3 GrabMoveMultipliers { get => grabMoveMultipliers; }
    public bool IsGrabbed { get => grabbed; }

    private void OnEnable()
    {
        foreach (var prompt in grabPrompts) prompt.PromptStateChange += OnGrabPromptStateChange;

        if (grabPrompts.Count < 1)
            Debug.LogWarning($"\"{name}\" does not have a grab prompt reference; it won't be able to be grabbed.");
    }
    private void OnDisable()
    {
        foreach (var prompt in grabPrompts) prompt.PromptStateChange -= OnGrabPromptStateChange;
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
        if (!player) return;

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

    private void OnGrabPromptStateChange(DisplayPrompt promptThatChanged, bool appearing, Collider changeTriggerer)
    {
#if UNITY_EDITOR
        if (printRegistrationResults)
        {
            print($"{name}: <color={(appearing ? "#0F0" : "#FF0")}>Prompt state changed @ {Time.timeAsDouble}!</color> " +
                $"<color=#777>appearing: {appearing}, changeTriggerer: {(changeTriggerer ? changeTriggerer : "null")}, " +
                $"\n\t{(appearing ? $"Registering? {!!changeTriggerer}" : $"Deregistering? {!promptThatChanged.IsActivePrompt}")}");
        }
#endif

        if (appearing && changeTriggerer)
        {
            Register(changeTriggerer);
        }
        //Don't deregister if the prompt has disappeared, but is still the active prompt.
        else if (!appearing && !promptThatChanged.IsActivePrompt)
        {
            Deregister();
        }
    }

    private void Register(Collider registeredCollider)
    {
        //Try to get a reference to the player from the collider. Only move on if we succeed.
        //  If we already have a player ref, no need to re-register.
        if (player || !registeredCollider.TryGetComponent(out player)) return;

        player.PulledObject = this;

        if (liftable && !string.IsNullOrEmpty(liftedTag)) tag = liftedTag;

        InputHub.Inst.Gameplay.Grab.performed += OnInteractPerformedWhileRegistered;
    }

    private void Deregister()
    {
        //If we don't have a reference to the player, we don't need to "deregister" anything.
        if (!player || player.pullingObject) return;

        InputHub.Inst.Gameplay.Grab.performed -= OnInteractPerformedWhileRegistered;

        player.PulledObject = null;
        player = null;

        if (liftable) tag = initTag;
    }

#if UNITY_EDITOR
    [NaughtyAttributes.Button(topPadding: 5)]
    private void PingFirstPrompt()
    {
        var firstPrompt = GetComponentInChildren<InitGrabIndicationRefs>();
        if (firstPrompt)
        {
            UnityEditor.EditorGUIUtility.PingObject(firstPrompt);
        }
    }
#endif
}
