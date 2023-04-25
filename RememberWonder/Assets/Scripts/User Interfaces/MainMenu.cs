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
    [SerializeField] float inputCooldownTime = 0.15f;

    [Header("Settings Menu Shenanigans")]
    [SerializeField] GameObject[] windowOptions;
    [SerializeField] int submenuSelection;
    [SerializeField] int windowSelection;
    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider sfxSlider;
    [SerializeField] Slider cameraSlider;
    [SerializeField] GameObject[] exitOptions;
    [SerializeField] GameObject[] grabToggleOptions;
    [SerializeField] int exitSelection;
    [SerializeField] int grabSelection;
    [SerializeField] GameObject[] highlightableElements;

    Vector2 moveInputCache;
    Coroutine vertInputCooldown = null;
    Coroutine horzInputCooldown = null;

    //
    int windowSetting;
    float bgmVolume;
    float sfxVolume;
    float cameraSens;
    int holdToGrab;

    bool viewingInstructions;
    bool inSubmenu;

    // Start is called before the first frame update
    void Start()
    {
        mainMenuSelection = 0;
        initialMenuOptions[0].GetComponent<Image>().enabled = true;
        inSubmenu = false;  //InputHub.Inst.UI.Move.performed += ChangeSelection;
        InputHub.Inst.UI.Select.performed += Select;
        viewingInstructions = false;
        LoadPlayerSettings();
    }
    private void OnDestroy()
    {
        InputHub.Inst.UI.Select.performed -= Select;
        InputHub.Inst.UI.Select.performed -= ConfirmSettings;
    }

    private void Update()
    {
        moveInputCache = InputHub.Inst.UI.Move.ReadValue<Vector2>();

        if (!moveInputCache.y.EqualWithinRange(0, menuInputThreshold))
        {
            if (vertInputCooldown != null)
                moveInputCache.y = 0;
            else vertInputCooldown = Coroutilities.DoAfterDelay(this, () => vertInputCooldown = null, inputCooldownTime, true);
        }

        if (!moveInputCache.x.EqualWithinRange(0, menuInputThreshold))
        {
            if (horzInputCooldown != null)
                moveInputCache.x = 0;
            else horzInputCooldown = Coroutilities.DoAfterDelay(this, () => horzInputCooldown = null, inputCooldownTime, true);
        }

        ChangeSelection(moveInputCache);
        SubmenuSelection(moveInputCache);
    }

    void Select(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (!viewingInstructions)
        {
            switch (mainMenuSelection)
            {
                case 0:
                    inSubmenu = false;  //InputHub.Inst.UI.Move.performed -= ChangeSelection;
                    InputHub.Inst.UI.Select.performed -= Select;
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
        //InputHub.Inst.UI.Move.performed -= ChangeSelection;
        InputHub.Inst.UI.Select.performed -= Select;

        InputHub.Inst.UI.Select.performed += ConfirmSettings;
        inSubmenu = true;   //InputHub.Inst.UI.Move.performed += SubmenuSelection;

        LoadPlayerSettings();
        settingsMenu.SetActive(true);
    }
    void UnloadSettings()
    {
        settingsMenu.SetActive(false);
        InputHub.Inst.UI.Select.performed -= ConfirmSettings;
        //InputHub.Inst.UI.Move.performed -= SubmenuSelection;

        inSubmenu = false;  //InputHub.Inst.UI.Move.performed += ChangeSelection;
        InputHub.Inst.UI.Select.performed += Select;

        foreach (GameObject element in highlightableElements)
            element.GetComponent<Image>().color = Color.white;

        mainMenu.SetActive(true);
    }

    void ChangeSelection(Vector2 input)
    {
        if (inSubmenu) return;

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
        holdToGrab = PlayerPrefs.GetInt("holdToLift");

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

        if (holdToGrab == 1)
        {
            grabToggleOptions[0].GetComponent<Image>().color = Color.white;
            grabToggleOptions[0].GetComponent<Image>().enabled = true;

            grabToggleOptions[1].GetComponent<Image>().color = Color.white;
            grabToggleOptions[1].GetComponent<Image>().enabled = false;

            grabSelection = 0;
        }
        else
        {
            grabToggleOptions[0].GetComponent<Image>().color = Color.white;
            grabToggleOptions[0].GetComponent<Image>().enabled = false;

            grabToggleOptions[1].GetComponent<Image>().color = Color.white;
            grabToggleOptions[1].GetComponent<Image>().enabled = true;
            grabSelection = 1;
        }

        submenuSelection = windowSetting;
        //Sliders
        bgmSlider.value = bgmVolume;
        sfxSlider.value = sfxVolume;
        cameraSlider.value = cameraSens;

        SetMixerVolumeViaSlider(bgmSlider, "bgmVol");
        SetMixerVolumeViaSlider(sfxSlider, "sfxVol");

        submenuSelection = 0;
    }

    void InitMissingPrefValues()
    {
        if (!PlayerPrefs.HasKey("windowSetting")) windowSetting = 0;
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


        if (!PlayerPrefs.HasKey("holdToLift")) PlayerPrefs.SetInt("holdToLift", 0);
        PlayerPrefs.SetInt("holdToPull", 0);

        if (!PlayerPrefs.HasKey("bgmVolume"))
        {
            bgmVolume = 0.5f;
            PlayerPrefs.SetFloat("bgmVolume", bgmVolume);
        }

        if (!PlayerPrefs.HasKey("sfxVolume"))
        {
            sfxVolume = 0.5f;
            PlayerPrefs.SetFloat("sfxVolume", sfxVolume);
        }

        if (!PlayerPrefs.HasKey("cameraSens"))
        {
            cameraSens = 0.5f;
            PlayerPrefs.SetFloat("cameraSens", cameraSens);
        }

        PlayerPrefs.Save();
    }

    void ApplyNewSettings()
    {
        if (windowSelection == 0)
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        else
            Screen.fullScreenMode = FullScreenMode.Windowed;

        if (grabSelection == 0)
        {
            PlayerPrefs.SetInt("holdToLift", 1);
            PlayerPrefs.SetInt("holdToPull", 1);
        }
        else
        {
            PlayerPrefs.SetInt("holdToLift", 0);
            PlayerPrefs.SetInt("holdToPull", 0);
        }

        SetMixerVolumeViaSlider(bgmSlider, "bgmVol");
        SetMixerVolumeViaSlider(sfxSlider, "sfxVol");

        bgmVolume = bgmSlider.value;
        sfxVolume = sfxSlider.value;

        PlayerPrefs.SetInt("windowSetting", windowSelection);
        PlayerPrefs.SetFloat("bgmVolume", bgmVolume);
        PlayerPrefs.SetFloat("sfxVolume", sfxVolume);
        PlayerPrefs.SetFloat("cameraSens", cameraSlider.value);
    }
    private void SetMixerVolumeViaSlider(Slider slider, string mixerFloatName)
        => mixer.SetFloat(mixerFloatName, -1 * (30 - (30 * slider.value)));

    void ConfirmSettings(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (submenuSelection < 4)
            return;

        if (exitSelection == 0)
            ApplyNewSettings();
        else
        {
            bgmSlider.value = bgmVolume;
            sfxSlider.value = sfxVolume;
            SetMixerVolumeViaSlider(bgmSlider, "bgmVol");
            SetMixerVolumeViaSlider(sfxSlider, "sfxVol");
        }

        UnloadSettings();
    }

    void SubmenuSelection(Vector2 input)
    {
        if (!inSubmenu) return;

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
                if (input.x > menuInputThreshold) bgmSlider.value += 0.1f;
                else if (input.x < -menuInputThreshold) bgmSlider.value -= 0.1f;
                SetMixerVolumeViaSlider(bgmSlider, "bgmVol");
                break;

            case 2:
                if (input.x > menuInputThreshold) sfxSlider.value += 0.1f;
                else if (input.x < -menuInputThreshold) sfxSlider.value -= 0.1f;
                SetMixerVolumeViaSlider(sfxSlider, "sfxVol");
                break;

            case 3:
                if (input.x > menuInputThreshold) cameraSlider.value += 0.1f;
                else if (input.x < -menuInputThreshold) cameraSlider.value -= 0.1f;
                break;
            case 4:
                if (input.x >= menuInputThreshold)
                {
                    grabSelection++;
                    if (grabSelection > 1)
                        grabSelection = 0;
                }
                else if (input.x < -menuInputThreshold)
                {
                    grabSelection--;
                    if (grabSelection < 0)
                        grabSelection = 1;
                }
                break;

            case 5:
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
                submenuSelection = 5;
        }
        else if (input.y < -menuInputThreshold)
        {
            submenuSelection++;
            if (submenuSelection > 5)
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

        if (grabSelection == 0)
        {
            highlightableElements[5].GetComponent<Image>().enabled = true;
            highlightableElements[6].GetComponent<Image>().enabled = false;
        }
        else
        {
            highlightableElements[5].GetComponent<Image>().enabled = false;
            highlightableElements[6].GetComponent<Image>().enabled = true;
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
                if (grabSelection == 0)
                    highlightableElements[5].GetComponent<Image>().color = Color.cyan;
                else
                    highlightableElements[6].GetComponent<Image>().color = Color.cyan;
                break;
            case 5:
                if (exitSelection == 0)
                    highlightableElements[7].GetComponent<Image>().color = Color.cyan;
                else
                    highlightableElements[8].GetComponent<Image>().color = Color.cyan;
                break;
        }

    }
}
