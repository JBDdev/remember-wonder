using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionBlock : MonoBehaviour
{
    [SerializeField] string displayText;
    [SerializeField] GameObject canvas;

    // Start is called before the first frame update
    void Start()
    {
        canvas = GameObject.Find("MoteCanvas");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player Character") 
        {
            other.GetComponent<PlayerMovement>().ReadingDialog = true;
            canvas.GetComponent<MoteUIController>().DisplayTutorialText(displayText);
        }
    }
}
