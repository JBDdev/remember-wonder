using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject[] initialMenuOptions;
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject instructionsMenu;
    [SerializeField] GameObject settingsMenu;
    [SerializeField] AudioMixer mixer;

    [SerializeField] int mainMenuSelection;
    [SerializeField] float menuInputThreshold;

    [Header("Settings Menu Shenanigans")]
    [SerializeField] GameObject[] windowOptions;
    [SerializeField] int submenuSelection;
    [SerializeField] int windowSelection;
    [SerializeField] GameObject bgmSlider;
    [SerializeField] GameObject sfxSlider;
    [SerializeField] GameObject cameraSlider;
    [SerializeField] GameObject[] exitOptions;
    [SerializeField] int exitSelection;
    [SerializeField] GameObject[] highlightableElements;

    //
    int windowSetting;
    float bgmVolume;
    float sfxVolume;
    float cameraSens;

    bool viewingInstructions;

    // Start is called before the first frame update
    void Start()
    {
        mainMenuSelection = 0;
        initialMenuOptions[0].GetComponent<Image>().enabled = true;
        InputHub.Inst.Gameplay.MenuNav.performed += ChangeSelection;
        InputHub.Inst.Gameplay.Jump.performed += Select;
        viewingInstructions = false;
        LoadPlayerSettings();

    }

    void Select(UnityEngine.InputSystem.InputAction.CallbackContext ctx) 
    {
        if (!viewingInstructions)
        {
            switch (mainMenuSelection)
            {
                case 0:
                    InputHub.Inst.Gameplay.MenuNav.performed -= ChangeSelection;
                    InputHub.Inst.Gameplay.Jump.performed -= Select;
                    SceneManager.LoadScene(1);
                    break;
                case 1:
                    LoadInstructions();
                    break;
                case 2:
                    LoadSettings();
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

    void LoadSettings()
    {
        mainMenu.SetActive(false);
        InputHub.Inst.Gameplay.MenuNav.performed -= ChangeSelection;
        InputHub.Inst.Gameplay.Jump.performed -= Select;

        InputHub.Inst.Gameplay.Jump.performed += ConfirmSettings;
        InputHub.Inst.Gameplay.MenuNav.performed += SubmenuSelection;

        LoadPlayerSettings();
        settingsMenu.SetActive(true);
    }
    void UnloadSettings()
    {
        settingsMenu.SetActive(false);
        InputHub.Inst.Gameplay.Jump.performed -= ConfirmSettings;
        InputHub.Inst.Gameplay.MenuNav.performed -= SubmenuSelection;

        InputHub.Inst.Gameplay.MenuNav.performed += ChangeSelection;
        InputHub.Inst.Gameplay.Jump.performed += Select;

        foreach (GameObject element in highlightableElements)
            element.GetComponent<Image>().color = Color.white;

        mainMenu.SetActive(true);
    }

    void ChangeSelection(UnityEngine.InputSystem.InputAction.CallbackContext ctx) 
    {
        Vector2 input = InputHub.Inst.Gameplay.MenuNav.ReadValue<Vector2>();

        if (input.y > menuInputThreshold)
            mainMenuSelection--;
        else if (input.y < -menuInputThreshold)
            mainMenuSelection++;

        if (mainMenuSelection < 0)
            mainMenuSelection = 3;
        else if (mainMenuSelection > 3)
            mainMenuSelection = 0;

        foreach (GameObject option in initialMenuOptions)
            option.GetComponent<Image>().enabled = false;

        initialMenuOptions[mainMenuSelection].GetComponent<Image>().enabled = true;

    }

    void LoadPlayerSettings() 
    {
        windowSetting = PlayerPrefs.GetInt("windowSetting");
        bgmVolume = PlayerPrefs.GetFloat("bgmVolume");
        sfxVolume = PlayerPrefs.GetFloat("sfxVolume");
        cameraSens = PlayerPrefs.GetFloat("cameraSens");

        //This call might be able to be removed later
        InitMissingPrefValues();

        //Window Settings

        if (windowSetting == 0)
        {
            windowOptions[0].GetComponent<Image>().color = Color.cyan;
            windowOptions[0].GetComponent<Image>().enabled = true;

            windowOptions[1].GetComponent<Image>().color = Color.white;
            windowOptions[1].GetComponent<Image>().enabled = false;
        }
        else
        {
            windowOptions[0].GetComponent<Image>().color = Color.white;
            windowOptions[0].GetComponent<Image>().enabled = false;

            windowOptions[1].GetComponent<Image>().color = Color.cyan;
            windowOptions[1].GetComponent<Image>().enabled = true;
        }

        submenuSelection = windowSetting;
        //Sliders
        bgmSlider.transform.GetChild(0).GetComponent<Slider>().value = bgmVolume;
        sfxSlider.transform.GetChild(0).GetComponent<Slider>().value = sfxVolume;
        cameraSlider.transform.GetChild(0).GetComponent<Slider>().value = cameraSens;

        submenuSelection = 0;
    }

    void InitMissingPrefValues() 
    {
        switch (windowSetting)
        {
            case -1:
                windowSetting = 0;
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                PlayerPrefs.SetInt("windowSetting", 0);
                break;
            case 0:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
            case 1:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
        }

        if (bgmVolume == -1f)
        {
            bgmVolume = 0.5f;
            PlayerPrefs.SetFloat("bgmVolume", bgmVolume);
        }

        if (sfxVolume == -1f)
        {
            sfxVolume = 0.5f;
            PlayerPrefs.SetFloat("sfxVolume", sfxVolume);
        }

        if (cameraSens == -1f)
        {
            cameraSens = 0.5f;
            PlayerPrefs.SetFloat("cameraSens", sfxVolume);
        }

        PlayerPrefs.Save();
    }

    void ApplyNewSettings()
    {
        if (windowSelection == 0)
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        else
            Screen.fullScreenMode = FullScreenMode.Windowed;

        //Update Volume Here
        float bgmValue = bgmSlider.transform.GetChild(0).GetComponent<Slider>().value;
        bgmValue = 30 - (30 * bgmValue);
        mixer.SetFloat("bgmVol", -bgmValue);

        float sfxValue = sfxSlider.transform.GetChild(0).GetComponent<Slider>().value;
        sfxValue = 30 - (30 * sfxValue);
        mixer.SetFloat("sfxVol", -sfxValue);

        //Update Camera Sensitvity Here

        PlayerPrefs.SetInt("windowSetting", windowSelection);
        PlayerPrefs.SetFloat("bgmVolume", bgmSlider.transform.GetChild(0).GetComponent<Slider>().value);
        PlayerPrefs.SetFloat("sfxVolume", sfxSlider.transform.GetChild(0).GetComponent<Slider>().value);
        PlayerPrefs.SetFloat("cameraSens", cameraSlider.transform.GetChild(0).GetComponent<Slider>().value);
    }

    void ConfirmSettings(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (submenuSelection < 4)
            return;

        if (exitSelection == 0)
            ApplyNewSettings();

        UnloadSettings();
    }

    void SubmenuSelection(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        Vector2 input = InputHub.Inst.Gameplay.MenuNav.ReadValue<Vector2>();

        //Handle Horizontal Input
        switch (submenuSelection)
        {
            case 0:
                if (input.x > menuInputThreshold)
                {
                    windowSelection++;
                    if (windowSelection > 1)
                        windowSelection = 0;
                }
                else if (input.x < -menuInputThreshold)
                {
                    windowSelection--;
                    if (windowSelection < 0)
                        windowSelection = 1;
                }
                break;
            case 1:
                if (input.x > menuInputThreshold)
                    bgmSlider.transform.GetChild(0).GetComponent<Slider>().value += 0.1f;
                else if (input.x < -menuInputThreshold)
                    bgmSlider.transform.GetChild(0).GetComponent<Slider>().value -= 0.1f;
                break;
            case 2:
                if (input.x > menuInputThreshold)
                    sfxSlider.transform.GetChild(0).GetComponent<Slider>().value += 0.1f;
                else if (input.x < -menuInputThreshold)
                    sfxSlider.transform.GetChild(0).GetComponent<Slider>().value -= 0.1f;
                break;
            case 3:
                if (input.x > menuInputThreshold)
                    cameraSlider.transform.GetChild(0).GetComponent<Slider>().value += 0.1f;
                else if (input.x < -menuInputThreshold)
                    cameraSlider.transform.GetChild(0).GetComponent<Slider>().value -= 0.1f;
                break;
            case 4:
                if (input.x > menuInputThreshold)
                {
                    exitSelection++;
                    if (exitSelection > 1)
                        exitSelection = 0;
                }
                else if (input.x < -menuInputThreshold)
                {
                    exitSelection--;
                    if (exitSelection < 0)
                        exitSelection = 1;
                }
                break;
        }

        //Handle Vertical Input

        if (input.y > menuInputThreshold)
        {
            submenuSelection--;
            if (submenuSelection < 0)
                submenuSelection = 4;
        }
        else if (input.y < -menuInputThreshold)
        {
            submenuSelection++;
            if (submenuSelection > 4)
                submenuSelection = 0;
        }

        //Highlight the correct element and reset all the others

        foreach (GameObject element in highlightableElements)
            element.GetComponent<Image>().color = Color.white;

        if (windowSelection == 0)
        {
            highlightableElements[0].GetComponent<Image>().enabled = true;
            highlightableElements[1].GetComponent<Image>().enabled = false;
        }
        else if (windowSelection == 1)
        {
            highlightableElements[0].GetComponent<Image>().enabled = false;
            highlightableElements[1].GetComponent<Image>().enabled = true;
        }

        switch (submenuSelection)
        {
            case 0:
                if (windowSelection == 0)
                    highlightableElements[0].GetComponent<Image>().color = Color.cyan;
                else
                    highlightableElements[1].GetComponent<Image>().color = Color.cyan;
                break;
            case 1:
                highlightableElements[2].GetComponent<Image>().color = Color.cyan;
                break;
            case 2:
                highlightableElements[3].GetComponent<Image>().color = Color.cyan;
                break;
            case 3:
                highlightableElements[4].GetComponent<Image>().color = Color.cyan;
                break;
            case 4:
                if (exitSelection == 0)
                    highlightableElements[5].GetComponent<Image>().color = Color.cyan;
                else
                    highlightableElements[6].GetComponent<Image>().color = Color.cyan;
                break;
        }

    }
}
