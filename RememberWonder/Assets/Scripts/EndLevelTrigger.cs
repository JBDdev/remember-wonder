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

    /*MoteUIController moteUI;

    private void Start()
    {
        moteUI = GameObject.Find("MoteCanvas").GetComponent<MoteUIController>();
    }*/

    void OnTriggerEnter(Collider col)
    {
        if (!triggererTags.Contains(col.gameObject.tag)) return;

        //Run the call to clear the UI
        //Run the call to stop player input

        //Run the call to pull up end screen
        //Spawn in the greyed out puzzle pieces (or just have them pre-added in advance)
        //Loop thru and fill in each piece + play audio
        //Elephant backflip?
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
