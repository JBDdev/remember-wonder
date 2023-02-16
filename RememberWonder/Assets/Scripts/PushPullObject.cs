using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PushPullObject : MonoBehaviour
{
    [SerializeField] PlayerMovement player;
    [SerializeField] bool grabbed;
    [SerializeField] Vector3 grabMoveMultipliers = Vector3.one;
    public float maxPullDistance;
    public bool liftable;
    [SerializeField] AudioList liftAudio;
    [SerializeField] AudioList putDownAudio;
    [SerializeField] SourceSettings audioSettings;
    public Vector3 defaultPos;

    Rigidbody rb;
    Renderer[] childRendsCache;
    string initTag;    //QUICK AND DIRTY FIX for camera collision, Delete later?

    public Vector3 GrabMoveMultipliers { get => grabMoveMultipliers; }

    void Start()
    {
        grabbed = false;
        defaultPos = transform.position;
        initTag = tag;

        rb = GetComponent<Rigidbody>();
        childRendsCache = GetComponentsInChildren<Renderer>();
    }

    private void Update()
    {
        // If we are too far away, deregister
        if (liftable && player != null)
        {
            //Debug.Log();
            if (!grabbed && (transform.position - player.transform.position).sqrMagnitude > 3f)
            {
                Deregister();
            }

        }
    }
    private void OnTriggerEnter(Collider col)
    {
        if (!col.gameObject.CompareTag("Player")) return;

        player = col.gameObject.GetComponent<PlayerMovement>();
        Register();
    }

    private void OnTriggerExit(Collider col)
    {
        if (!col.gameObject.CompareTag("Player")) return;
        Deregister();
    }

    void OnInteractPerformed(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (player == null || (!player.IsGrounded() && !liftable)) return;

        grabbed = !grabbed;

        //Rigidbody rb = GetComponent<Rigidbody>();
        if (liftable)
        {
            if (grabbed)
                Destroy(rb);
            else
            {
                rb = transform.gameObject.AddComponent<Rigidbody>();
                //rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            }
        }

        if (grabbed)
        {
            if (liftable)
            {
                transform.position = player.transform.GetChild(0).GetChild(0).transform.position;
                transform.parent = player.transform.GetChild(0).GetChild(0).transform;

                AudioHub.Inst.Play(liftAudio, audioSettings, transform.position);
            }

            else
                transform.parent = player.transform;
        }
        else
        {
            transform.parent = null;
            if (liftable) AudioHub.Inst.Play(putDownAudio, audioSettings, transform.position);
        }
    }

    private void UpdateChildRends(System.Action<Renderer> updateFunc, bool refreshCache = false)
    {
        if (refreshCache) childRendsCache = GetComponentsInChildren<Renderer>();

        foreach (Renderer rend in childRendsCache)
            updateFunc(rend);
    }

    //Breakout of OnTriggerEnter / OnTriggerExit functionality
    private void Register()
    {
        if (!player || player.transform.position.y >= transform.position.y + 1.5f)
        {
            player = null;
            return;
        }

        //Debug.Log(transform.position.y + 1.5f);
        //Debug.Log(player.gameObject.transform.position.y);
        //Debug.Log(player.gameObject.transform.position.y >= transform.position.y + 1.5f);

        //Debug.Log("Grounded: " + player.IsGrounded());

        if (player.PulledObject || player.pullingObject || !player.IsGrounded())
            return;
        player.PulledObject = this;

        //QUICK AND DIRTY FIX for camera collision; delete later?
        tag = "Player";

        InputHub.Inst.Gameplay.Grab.performed += OnInteractPerformed;

        UpdateChildRends(rend => rend.material.color = Color.grey);
    }
    private void Deregister()
    {
        //We should have a ref to player; we get one when they enter.
        //  If we don't, this function fired twice or something.
        if (!player || player.pullingObject) return;

        InputHub.Inst.Gameplay.Grab.performed -= OnInteractPerformed;

        player.PulledObject = null;
        player = null;

        //QUICK AND DIRTY FIX for camera collision; delete later?
        tag = initTag;

        UpdateChildRends(rend => rend.material.color = Color.white);
    }
}
