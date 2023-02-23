using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndLevelTrigger : MonoBehaviour
{
    [SerializeField] string scene;

    GameObject canvas;

    private void Start()
    {
        canvas = GameObject.Find("MoteCanvas");
    }
    // Start is called before the first frame update
    void OnTriggerEnter(Collider col) 
    {
        Debug.Log("hi");
        if (col.gameObject.CompareTag("Player"))
        {
            if (canvas.GetComponent<MoteUIController>().CollectedCount >= 10)
                SceneManager.LoadScene(scene);
            else
                Debug.Log("Player only has " + canvas.GetComponent<MoteUIController>().CollectedCount + " motes.");
        }
    }
}
