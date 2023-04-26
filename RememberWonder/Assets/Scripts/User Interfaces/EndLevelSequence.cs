using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevelSequence : MonoBehaviour
{
    [SerializeField] RectTransform creditsTextObj;
    [SerializeField] float scrollSpeed;
    [SerializeField] float fasterScrollSpeed;

    // Start is called before the first frame update
    void Start()
    {
        InputHub.Inst.UI.Select.performed += LoadTitleScreen;
    }

    // Update is called once per frame
    void Update()
    {
        //Scroll the credits up faster if back button is held
        creditsTextObj.position += Vector3.up
            * (InputHub.Inst.UI.Back.IsPressed() ? fasterScrollSpeed : scrollSpeed)
            * Time.deltaTime;
    }

    void LoadTitleScreen(UnityEngine.InputSystem.InputAction.CallbackContext ctx) 
    {
        InputHub.Inst.UI.Select.performed -= LoadTitleScreen;
        SceneManager.LoadScene(0);
    }
}
