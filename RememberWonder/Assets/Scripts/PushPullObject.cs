using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PushPullObject : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] bool grabbed;
    public string[] usableAxes;
    public float maxPullDistance;
    public bool disableJump;

    public Vector3 defaultPos;

    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        grabbed = false;
        defaultPos = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        
    }

    private void OnTriggerEnter(Collider col)
    {
        //Debug.Log("entered");
        if (col.gameObject.CompareTag("Player"))
        {
            Debug.Log("Grounded: " + col.gameObject.GetComponent<PlayerMovement>().ForceGroundedUpdate());
            if (col.gameObject.GetComponent<PlayerMovement>().PulledObject != null || col.gameObject.GetComponent<PlayerMovement>().pullingObject || !col.gameObject.GetComponent<PlayerMovement>().ForceGroundedUpdate())
                return;
            InputHub.Inst.Gameplay.Interact.performed += OnInteractPerformed;
            player = col.gameObject;
            player.GetComponent<PlayerMovement>().PulledObject = transform.gameObject;
            foreach (Renderer r in GetComponentsInChildren<Renderer>()) 
            {
                r.material.color = Color.gray; 
            }
        }
    }

    private void OnTriggerExit(Collider col)
    {
        //Debug.Log("exited");
        if (col.gameObject.CompareTag("Player"))
        {
            if (col.gameObject.GetComponent<PlayerMovement>().pullingObject)
                return;
            InputHub.Inst.Gameplay.Interact.performed -= OnInteractPerformed;
            player.GetComponent<PlayerMovement>().PulledObject = null;
            player = null;
            foreach (Renderer r in GetComponentsInChildren<Renderer>())
            {
                r.material.color = Color.white;
            }
        }
    }

    void OnInteractPerformed(UnityEngine.InputSystem.InputAction.CallbackContext ctx) 
    {
        if (player != null) 
        {
            if(!player.GetComponent<PlayerMovement>().ForceGroundedUpdate())
                return;

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
        
       
    }

}
