using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushPullObject : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] bool grabbed;
    // Start is called before the first frame update
    void Start()
    {
        grabbed = false;
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
        if (col.gameObject.CompareTag("Player"))
        {
            InputHub.Inst.Gameplay.Interact.performed += OnInteractPerformed;
            player = col.gameObject;
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.transform == player)
        {
            InputHub.Inst.Gameplay.Interact.performed -= OnInteractPerformed;
            player = null;
        }
    }

    void OnInteractPerformed(UnityEngine.InputSystem.InputAction.CallbackContext ctx) 
    {
        if (player != null) 
        {
            if (!grabbed)
                transform.parent = player.transform;
            else
                transform.parent = null;

            grabbed = !grabbed;
        }
    }

}
