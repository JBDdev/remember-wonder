using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject[] initialMenuOptions;
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject instructionsMenu;
    [SerializeField] GameObject settingsMenu;

    [SerializeField] int selection;

    bool viewingInstructions;
    // Start is called before the first frame update
    void Start()
    {
        selection = 0;
        initialMenuOptions[0].GetComponent<Image>().enabled = true;
        InputHub.Inst.Gameplay.Move.performed += ChangeSelection;
        InputHub.Inst.Gameplay.Jump.performed += Select;
        viewingInstructions = false;
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }

    void Select(UnityEngine.InputSystem.InputAction.CallbackContext ctx) 
    {
        if (!viewingInstructions)
        {
            switch (selection)
            {
                case 0:
                    InputHub.Inst.Gameplay.Move.performed -= ChangeSelection;
                    InputHub.Inst.Gameplay.Jump.performed -= Select;
                    SceneManager.LoadScene(1);
                    break;
                case 1:
                    LoadInstructions();
                    break;
                case 2:
                    Debug.Log("Settings Menu Not Implemented Yet! This will be yoinked from the demo scene later!");
                    break;
                case 3:
                    Application.Quit();
                    break;
            }
        }
        else 
        {
            UnloadInstructions();
        }
    }

    void LoadInstructions() 
    {
        mainMenu.SetActive(false);
        instructionsMenu.SetActive(true);
        viewingInstructions = true;
    }

    void UnloadInstructions() 
    {
        instructionsMenu.SetActive(false);
        mainMenu.SetActive(true);
        viewingInstructions = false;
    }

    void ChangeSelection(UnityEngine.InputSystem.InputAction.CallbackContext ctx) 
    {
        Vector2 input = InputHub.Inst.Gameplay.Move.ReadValue<Vector2>();

        if (input.y > 0)
            selection--;
        else if (input.y < 0)
            selection++;

        if (selection < 0)
            selection = 2;
        else if (selection > 3)
            selection = 0;

        foreach (GameObject option in initialMenuOptions)
            option.GetComponent<Image>().enabled = false;

        initialMenuOptions[selection].GetComponent<Image>().enabled = true;

    }
}
