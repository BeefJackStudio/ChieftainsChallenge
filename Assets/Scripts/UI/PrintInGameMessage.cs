using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PrintInGameMessage : MonoBehaviour {

    private Animation myAnim;
    private Text myText;

    //----------------------------------------------------------------------------------------------------------------------------------------------

    void OnEnable() {
        myAnim = gameObject.GetComponent<Animation>();
        myText = gameObject.GetComponent<Text>();

        NotificationsManager.StartPowerShot += NotificationsManager_StartPowerShot;
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------

    void OnDisable() {
        myAnim = gameObject.GetComponent<Animation>();

        NotificationsManager.StartPowerShot -= NotificationsManager_StartPowerShot;
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------

    private void NotificationsManager_StartPowerShot() {
        myText.text = "Power Shot";
        myAnim.Play();
    }
}
