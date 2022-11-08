using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Movement Variables")]
    [SerializeField] float maxSpeed;
    [SerializeField] float accModifier;

    //TODO: Evaluate utility of Input System Package

    [Header("Jump Controls")]
    [SerializeField] float jumpForce;
    [SerializeField] public bool usedJump = false;
    [SerializeField] public float maxIncline;

    [Header("External References")]
    [SerializeField] GameObject cameraFollower;

    //Internal Component References
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        //Get references to components on the GameObject
        rb = GetComponent<Rigidbody>();
    }

    void Update() 
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 direction = (cameraFollower.transform.forward * Input.GetAxis("Vertical")) + (cameraFollower.transform.right * Input.GetAxis("Horizontal"));

        direction.Normalize();

        rb.AddForce(direction * accModifier, ForceMode.Acceleration);
        //transform.position += direction * maxSpeed * Time.deltaTime;

        //Clamp the output velocity
        rb.velocity = new Vector3(Mathf.Clamp(rb.velocity.x, -maxSpeed, maxSpeed), rb.velocity.y, Mathf.Clamp(rb.velocity.z, -maxSpeed, maxSpeed));

        //Jump handling
        if (Input.GetAxis("Jump") > 0 && !usedJump) 
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(new Vector3(0f, jumpForce, 0f));
            usedJump = true;
        }

    }

    void OnCollisionEnter(Collision col)
    {
        //Debug.Log("test");
        if (col.GetContact(0).normal.y >= maxIncline)
        {
            //Debug.Log(col.GetContact(0).normal.y);
            usedJump = false;
        }

    }
}
