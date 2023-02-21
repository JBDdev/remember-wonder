using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropPointTrigger : MonoBehaviour
{
    bool invalidDropPosition;
    public bool InvalidDropPosition { get { return invalidDropPosition; } }

    [SerializeField] List<GameObject> collidingObjects;

    // Start is called before the first frame update
    void Start()
    {
        collidingObjects = new List<GameObject>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (collidingObjects.Count > 0)
            invalidDropPosition = true;
        else
            invalidDropPosition = false;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.transform.tag != "Player") 
        {
            collidingObjects.Add(col.gameObject);
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.transform.tag != "Player")
        {
            do
            {
                collidingObjects.Remove(col.gameObject);
            } while (collidingObjects.Contains(col.gameObject));
        }
    }
}
