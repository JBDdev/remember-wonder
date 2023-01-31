using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PushPullObject : MonoBehaviour
{
    [SerializeField] PlayerMovement player;
    [SerializeField] bool grabbed;
    public string[] usableAxes;
    public float maxPullDistance;
    public bool disableJump;
    public Vector3 defaultPos;

    Rigidbody rb;
    Renderer[] childRendsCache;

    void Start()
    {
        grabbed = false;
        defaultPos = transform.position;

        rb = GetComponent<Rigidbody>();
        childRendsCache = GetComponentsInChildren<Renderer>();
    }

    private void OnTriggerEnter(Collider col)
    {
        if (!col.gameObject.CompareTag("Player")) return;

        player = col.gameObject.GetComponent<PlayerMovement>();

        if (player.transform.position.y >= transform.position.y + 1.5f) 
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

        InputHub.Inst.Gameplay.Interact.performed += OnInteractPerformed;

        UpdateChildRends(rend => rend.material.color = Color.grey);
    }

    private void OnTriggerExit(Collider col)
    {
        if (!col.gameObject.CompareTag("Player")) return;

        //We should have a ref to player; we get one when they enter.
        //  If we don't, this function fired twice or something.
        if (!player || player.pullingObject) return;

        InputHub.Inst.Gameplay.Interact.performed -= OnInteractPerformed;

        player.PulledObject = null;
        player = null;

        UpdateChildRends(rend => rend.material.color = Color.white);
    }

    void OnInteractPerformed(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (!player || !player.IsGrounded()) return;

        grabbed = !grabbed;

        //Rigidbody rb = GetComponent<Rigidbody>();
        if (!disableJump)
        {
            if (grabbed)
                Destroy(rb);
            else
            {
                rb = transform.gameObject.AddComponent<Rigidbody>();
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            }
        }

        if (grabbed)
            transform.parent = player.transform;
        else
            transform.parent = null;
    }

    private void UpdateChildRends(System.Action<Renderer> updateFunc, bool refreshCache = false)
    {
        if (refreshCache) childRendsCache = GetComponentsInChildren<Renderer>();

        foreach (Renderer rend in childRendsCache)
            updateFunc(rend);
    }
}
