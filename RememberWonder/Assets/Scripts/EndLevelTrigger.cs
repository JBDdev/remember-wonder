using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndLevelTrigger : MonoBehaviour
{
    [SerializeField] string scene;
    [Tooltip("If negative, will move that many scenes ahead in the build order.")]
    [SerializeField] int sceneIndex;
    [SerializeField] Bewildered.UHashSet<TagString> triggererTags;
    [Space(5f)]
    [SerializeField] GameObject moteCanvas;
    [Header("End Level Screen")]
    [SerializeField] GameObject resultsScreen;
    [SerializeField] float startFillDelay;
    [SerializeField] float delayNextFill;
    [Header("Audio")]
    [SerializeField] AudioList collect;
    [SerializeField] AudioList pullUpMenu;
    [SerializeField] SourceSettings sourceSettings;

    bool fillingPieces = false;
    bool loadingScene = false;
    int filledPieces = 0;
    float fillTimer = 0;
    int motesToRemove = 0;

    private void Update()
    {
        if (fillingPieces)
        {
            fillTimer += Time.deltaTime;
            if (fillTimer >= delayNextFill)
            {
                fillTimer = 0;
                FillPiece();
            }

            if (filledPieces >= moteCanvas.GetComponent<MoteUIController>().CollectedCount)
            {
                fillingPieces = false;
                EnableInput();
            }
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (!triggererTags.Contains(col.gameObject.tag)) return;

        Invoke("HandleEndScreen", startFillDelay);
    }

    void HandleEndScreen()
    {
        //Run the call to clear the UI
        //Debug.Log(moteCanvas);
        moteCanvas.SetActive(false);

        AudioHub.Inst.Play(pullUpMenu, sourceSettings);

        //Run the call to stop player input
        GameObject.Find("Player Character").gameObject.GetComponent<PlayerMovement>().TogglePause();

        motesToRemove = 16 - moteCanvas.GetComponent<MoteUIController>().TotalCount;
        //Add the correct # of piece icons
        for (int i = 0; i < motesToRemove; i++)
        {
            resultsScreen.transform.GetChild(2).GetChild(15 - i).gameObject.SetActive(false);
        }
        //Run the call to pull up end screen
        resultsScreen.SetActive(true);

        fillingPieces = true;
    }

    //This code is called every time a piece needs to be filled in
    void FillPiece()
    {
        resultsScreen.transform.GetChild(2).GetChild(filledPieces).GetComponent<Image>().color = Color.white;
        AudioHub.Inst.Play(collect, sourceSettings);
        filledPieces++;
    }

    void EnableInput()
    {
        InputHub.Inst.Gameplay.Jump.performed += OnPressContinue;
        resultsScreen.transform.GetChild(3).gameObject.SetActive(true);
    }

    void OnPressContinue(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        if (loadingScene) return;
        loadingScene = true;

        InputHub.Inst.Gameplay.Jump.performed -= OnPressContinue;

        GameObject.Find("Player Character").gameObject.GetComponent<PlayerMovement>().TogglePause();
        if (string.IsNullOrWhiteSpace(scene))
        {
            SceneManager.LoadScene(sceneIndex >= 0
                ? sceneIndex
                : SceneManager.GetActiveScene().buildIndex + -sceneIndex);
        }
        else
        {
            SceneManager.LoadScene(scene);
        }
    }
}
