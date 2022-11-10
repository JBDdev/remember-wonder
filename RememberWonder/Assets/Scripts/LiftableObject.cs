using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftableObject : MonoBehaviour
{
    public Transform Target {get; set;}
    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player Character");
    }

    // Update is called once per frame
    void Update()
    {
        if(Target != null)
        {
            transform.parent.position = Target.position;
            if(Input.GetKeyDown(KeyCode.Q))
                this.Target = null;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.CompareTag("Player"))
            if(Input.GetKeyDown(KeyCode.E))
                this.Target = col.gameObject.GetComponent<PlayerMovement>().HoldLocation.transform;
    }
    void OnTriggerStay(Collider col)
    {
        if(col.gameObject.CompareTag("Player"))
            if(Input.GetKeyDown(KeyCode.E))
                this.Target = col.gameObject.GetComponent<PlayerMovement>().HoldLocation.transform;

    }
}
