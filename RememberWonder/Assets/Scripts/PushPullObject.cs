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
            
            InputHub.Inst.Gameplay.Interact.performed += OnInteractPerformed;
            player = col.gameObject;
            player.GetComponent<PlayerMovement>().PulledObject = transform.gameObject;
        }
    }

    private void OnTriggerExit(Collider col)
    {
        //Debug.Log("exited");
        if (col.gameObject.CompareTag("Player"))
        {
            
            InputHub.Inst.Gameplay.Interact.performed -= OnInteractPerformed;
            player.GetComponent<PlayerMovement>().PulledObject = null;
            player = null;
        }
    }

    void OnInteractPerformed(UnityEngine.InputSystem.InputAction.CallbackContext ctx) 
    {
        if (player != null) 
        {
            grabbed = !grabbed;

            //Rigidbody rb = GetComponent<Rigidbody>();
            if (!disableJump)
            {
                if (grabbed)
                    Destroy(rb);
                else
                    rb = transform.gameObject.AddComponent<Rigidbody>();
            }

            if (grabbed)
                transform.parent = player.transform;
            else
                transform.parent = null;

            
        }
        
       
    }

}
