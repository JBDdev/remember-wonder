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
    [Header("End Level Screen")]
    [SerializeField] GameObject resultsScreen;
    [SerializeField] float startFillDelay;
    [SerializeField] float delayNextFill;

    GameObject moteCanvas;

    int filledPieces = 0;
    /*MoteUIController moteUI;

    private void Start()
    {
        moteUI = GameObject.Find("MoteCanvas").GetComponent<MoteUIController>();
    }*/

    void OnTriggerEnter(Collider col)
    {
        if (!triggererTags.Contains(col.gameObject.tag)) return;

        Invoke("HandleEndScreen", startFillDelay);
    }

    void HandleEndScreen() 
    {
        //Run the call to clear the UI
        moteCanvas = GameObject.Find("MoteCanvas");
        //Debug.Log(moteCanvas);
        moteCanvas.SetActive(false);

        //Run the call to stop player input
        GameObject.Find("Player Character").gameObject.GetComponent<PlayerMovement>().TogglePause();

        int motesToRemove = 16 - moteCanvas.GetComponent<MoteUIController>().CollectedCount;
        //Add the correct # of piece icons
        for (int i = 0; i < motesToRemove; i++) 
        {           
            resultsScreen.transform.GetChild(2).GetChild(15-i).gameObject.SetActive(false);
        }

        //Run the call to pull up end screen
        resultsScreen.SetActive(true);

        //Loop thru and fill in each piece + play audio
        FillPiece(0);


        while (filledPieces < moteCanvas.GetComponent<MoteUIController>().CollectedCount)
        {
            //Coroutilities.DoAfterDelay(this, () => FillPiece(filledPieces), delayNextFill);
            FillPiece(filledPieces);
        }

        //Assign Press A to Continue and make that prompt show up on screen
        Invoke("EnableInput", resultsScreen.transform.GetChild(2).childCount * delayNextFill);
    }
    void FillPiece(int index) 
    {
        resultsScreen.transform.GetChild(2).GetChild(index).GetComponent<Image>().color = Color.white;
        filledPieces++;
    }

    void EnableInput()
    {
        InputHub.Inst.Gameplay.Jump.performed += OnPressContinue;
        resultsScreen.transform.GetChild(3).gameObject.SetActive(true);
    }

    void OnPressContinue(UnityEngine.InputSystem.InputAction.CallbackContext ctx) 
    {
        /*if (moteUI.CollectedCount >= 10)
        {*/
        InputHub.Inst.Gameplay.Jump.performed -= OnPressContinue;
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
        /*}


        else Debug.Log("Player only has " + moteUI.GetComponent<MoteUIController>().CollectedCount + " motes.");*/
    }
}
