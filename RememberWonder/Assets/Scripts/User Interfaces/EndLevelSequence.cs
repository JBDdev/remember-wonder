using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevelSequence : MonoBehaviour
{
    [SerializeField] GameObject creditsText;
    [SerializeField] float scrollSpeed;
    // Start is called before the first frame update
    void Start()
    {
        InputHub.Inst.Gameplay.Jump.performed += LoadTitleScreen;
    }

    // Update is called once per frame
    void Update()
    {
        creditsText.transform.position += new Vector3(0, scrollSpeed, 0) * Time.deltaTime;
    }

    void LoadTitleScreen(UnityEngine.InputSystem.InputAction.CallbackContext ctx) 
    {
        InputHub.Inst.Gameplay.Jump.performed -= LoadTitleScreen;
        SceneManager.LoadScene(0);
    }
}
