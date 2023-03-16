using System.Collections;
using System.Collections.Generic;
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

    /*MoteUIController moteUI;

    private void Start()
    {
        moteUI = GameObject.Find("MoteCanvas").GetComponent<MoteUIController>();
    }*/

    void OnTriggerEnter(Collider col)
    {
        if (!triggererTags.Contains(col.gameObject.tag)) return;

        //Run the call to clear the UI
        GameObject.Find("MoteCanvas").gameObject.SetActive(false);
        //Run the call to stop player input
        GameObject.Find("Player Character").gameObject.GetComponent<PlayerMovement>().TogglePause();

        //Run the call to pull up end screen
        resultsScreen.SetActive(true);
        //Loop thru and fill in each piece + play audio
        //Assign Press A to Continue and make that prompt show up on screen

        
    }

    void OnPressContinue(UnityEngine.InputSystem.InputAction.CallbackContext ctx) 
    {
        /*if (moteUI.CollectedCount >= 10)
        {*/
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
