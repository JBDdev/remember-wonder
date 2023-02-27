using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevelSequence : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        InputHub.Inst.Gameplay.Jump.performed += LoadTitleScreen;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LoadTitleScreen(UnityEngine.InputSystem.InputAction.CallbackContext ctx) 
    {
        InputHub.Inst.Gameplay.Jump.performed -= LoadTitleScreen;
        SceneManager.LoadScene(0);
    }
}
