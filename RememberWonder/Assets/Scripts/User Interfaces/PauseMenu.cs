using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject[] initialMenuOptions;
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject instructionsMenu;
    [SerializeField] GameObject settingsMenu;
    [SerializeField] GameObject player;
    [SerializeField] AudioMixer mixer;

    [SerializeField] GameObject moteUI;
    [SerializeField] GameObject pauseMenu;

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
    [SerializeField] GameObject[] grabToggleOptions;
    [SerializeField] GameObject[] exitOptions;
    [SerializeField] int exitSelection;
    [SerializeField] int grabSelection;
    [SerializeField] GameObject[] highlightableElements;

    Vector2 moveInputCache;
    Coroutine vertInputCooldown = null;
    Coroutine horzInputCooldown = null;

    //Settings values
    int windowSetting;
    float bgmVolume;
    float sfxVolume;
    float cameraSens;
    int holdToGrab;

    bool paused;
    bool inSubmenu;

    bool viewingInstructions;
    bool viewingSettings;

    float bgmVolumeAtStart;
    float sfxVolumeAtStart;

    // Start is called before the first frame update
    void Start()
    {
        mainMenuSelection = 0;
        paused = false;
        viewingInstructions = false;
        viewingSettings = false;
        InputHub.Inst.Gameplay.Pause.performed += PauseInput;
        pauseMenu.SetActive(false);
        player = GameObject.FindGameObjectWithTag("Player");

        mixer.GetFloat("bgmVol", out bgmVolumeAtStart);
        mixer.GetFloat("sfxVol", out sfxVolumeAtStart);

        LoadPlayerSettings();
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

    void PauseInput(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        TogglePauseMenu();
    }
    void TogglePauseMenu()
    {
        if (viewingSettings)
            return;

        if (paused)
        {
            inSubmenu = false;  //InputHub.Inst.UI.Move.performed -= ChangeSelection;
            InputHub.Inst.UI.Select.performed -= Select;
            if (viewingInstructions)
                UnloadInstructions();

            if (viewingSettings)
                UnloadSettings();

            pauseMenu.SetActive(false);
            paused = false;
            Time.timeScale = 1f;
            foreach (GameObject option in initialMenuOptions)
                option.GetComponent<Image>().enabled = false;

            moteUI.SetActive(true);
        }
        else
        {
            inSubmenu = false;  //InputHub.Inst.UI.Move.performed += ChangeSelection;
            InputHub.Inst.UI.Select.performed += Select;
            pauseMenu.SetActive(true);
            initialMenuOptions[mainMenuSelection].GetComponent<Image>().enabled = true;
            paused = true;
            Time.timeScale = 0f;
            moteUI.SetActive(false);
        }

        player.GetComponent<PlayerMovement>().TogglePause();
    }

    void ChangeSelection(Vector2 input)
    {
        if (!paused || inSubmenu) return;

        if (input.y > menuInputThreshold)
            mainMenuSelection--;
        else if (input.y < -menuInputThreshold)
            mainMenuSelection++;

        if (mainMenuSelection < 0)
            mainMenuSelection = 4;
        else if (mainMenuSelection > 4)
            mainMenuSelection = 0;

        foreach (GameObject option in initialMenuOptions)
            option.GetComponent<Image>().enabled = false;

        initialMenuOptions[mainMenuSelection].GetComponent<Image>().enabled = true;
    }

    void Select(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (viewingInstructions)
            UnloadInstructions();
        else if (viewingSettings)
        {
            UnloadSettings();
        }
        else
        {
            //This block covers the default pause menu
            switch (mainMenuSelection)
            {
                case 0:
                    TogglePauseMenu();
                    break;
                case 1:
                    LoadInstructions();
                    break;
                case 2:
                    LoadSettings();
                    break;
                case 3:
                    InputHub.Inst.Gameplay.Pause.performed -= PauseInput;
                    inSubmenu = false;  //InputHub.Inst.UI.Move.performed -= ChangeSelection;
                    InputHub.Inst.UI.Select.performed -= Select;
                    Time.timeScale = 1f;
                    SceneManager.LoadScene(0);
                    break;
            }
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
        InputHub.Inst.Gameplay.Pause.performed -= PauseInput;
        //InputHub.Inst.UI.Move.performed -= ChangeSelection;
        InputHub.Inst.UI.Select.performed -= Select;

        InputHub.Inst.UI.Select.performed += ConfirmSettings;
        inSubmenu = true;   //InputHub.Inst.UI.Move.performed += SubmenuSelection;

        LoadPlayerSettings();
        settingsMenu.SetActive(true);
        viewingSettings = true;
    }
    void UnloadSettings()
    {
        settingsMenu.SetActive(false);
        InputHub.Inst.UI.Select.performed -= ConfirmSettings;
        //InputHub.Inst.UI.Move.performed -= SubmenuSelection;

        InputHub.Inst.Gameplay.Pause.performed += PauseInput;
        inSubmenu = false;   //InputHub.Inst.UI.Move.performed += ChangeSelection;
        InputHub.Inst.UI.Select.performed += Select;

        foreach (GameObject element in highlightableElements)
            element.GetComponent<Image>().color = Color.white;

        mainMenu.SetActive(true);
        viewingSettings = false;
    }

    //Settings Menu Functionality
    #region Settings Menu Nonsense
    //ReadPlayerSettings
    void LoadPlayerSettings()
    {
        windowSetting = PlayerPrefs.GetInt("windowSetting");
        bgmVolume = PlayerPrefs.GetFloat("bgmVolume");
        sfxVolume = PlayerPrefs.GetFloat("sfxVolume");
        cameraSens = PlayerPrefs.GetFloat("cameraSens");
        holdToGrab = PlayerPrefs.GetInt("holdToLift");

        //This call might be able to be removed later
        InitMissingPrefValues();

        //Button Settings

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
        }
        else
        {
            grabToggleOptions[0].GetComponent<Image>().color = Color.white;
            grabToggleOptions[0].GetComponent<Image>().enabled = false;

            grabToggleOptions[1].GetComponent<Image>().color = Color.white;
            grabToggleOptions[1].GetComponent<Image>().enabled = true;
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

    //WriteNewSettings
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


        if(!PlayerPrefs.HasKey("holdToLift")) PlayerPrefs.SetInt("holdToLift", 0);
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
        if (!paused || !inSubmenu) return;

        //Handle Horizontal Input
        switch (submenuSelection)
        {
            case 0:
                if (input.x >= menuInputThreshold)
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
        else
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
    #endregion
}